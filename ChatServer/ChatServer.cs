using System.Net;
using System.Net.Sockets;
using ChatCommon;

namespace ChatServerApp;

/// <summary>
/// TcpListener ile gelen bağlantıları kabul eden ve mesajları dağıtan sohbet sunucusu.
/// Arayüzden bağımsızdır; olup biteni LogMessage ve ClientsChanged olaylarıyla bildirir.
/// </summary>
public class ChatServer
{
    private static readonly TimeSpan JoinTimeout = TimeSpan.FromSeconds(10);

    // Her istemci kendi async döngüsünde çalıştığı için listeye erişim kilitle korunur
    private readonly object _lock = new();
    private readonly Dictionary<string, ClientSession> _clients = new(StringComparer.OrdinalIgnoreCase);

    private TcpListener? _listener;
    private CancellationTokenSource? _cts;

    public bool IsRunning => _listener != null;
    public int Port { get; private set; }

    /// <summary>Günlüğe yazılacak bir olay oldu.</summary>
    public event EventHandler<string>? LogMessage;

    /// <summary>Bağlı istemci listesi veya istemci bilgileri değişti.</summary>
    public event EventHandler? ClientsChanged;

    /// <summary>Bağlı istemcilerin anlık bir kopyası (bağlanma sırasına göre).</summary>
    public IReadOnlyList<ClientSession> GetClients()
    {
        lock (_lock)
        {
            return _clients.Values.OrderBy(c => c.ConnectedAt).ToList();
        }
    }

    // ------------------------------------------------------------------
    // Başlatma / Durdurma
    // ------------------------------------------------------------------

    /// <summary>
    /// Sunucuyu verilen portta başlatır. Port kullanılıyorsa SocketException fırlatır.
    /// </summary>
    public void Start(int port)
    {
        if (IsRunning) return;

        var listener = new TcpListener(IPAddress.Any, port); // Tüm ağ kartlarından bağlantı kabul et
        listener.Start();

        _listener = listener;
        _cts = new CancellationTokenSource();
        Port = port;

        Log($"Sunucu {port} portunda başlatıldı. Bağlantı bekleniyor...");
        _ = AcceptLoopAsync(listener, _cts.Token);
    }

    /// <summary>Tüm istemcilere haber verir, bağlantıları kapatır ve dinlemeyi bırakır.</summary>
    public async Task StopAsync()
    {
        if (_listener == null) return;

        _cts?.Cancel();
        _listener.Stop();
        _listener = null;

        List<ClientSession> all;
        lock (_lock)
        {
            all = _clients.Values.ToList();
            _clients.Clear();
        }

        foreach (var client in all)
        {
            await TrySendAsync(client, ChatMessage.CreateSystem("Sunucu kapatıldı."));
            client.Connection.Dispose();
        }

        Log("Sunucu durduruldu.");
        ClientsChanged?.Invoke(this, EventArgs.Empty);
    }

    // ------------------------------------------------------------------
    // Bağlantı kabul ve istemci döngüsü
    // ------------------------------------------------------------------

    /// <summary>Yeni bağlantıları sürekli kabul eder; her biri için ayrı bir görev başlatır.</summary>
    private async Task AcceptLoopAsync(TcpListener listener, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            TcpClient tcp;
            try
            {
                tcp = await listener.AcceptTcpClientAsync(ct);
            }
            catch (Exception ex) when (ex is OperationCanceledException or SocketException or ObjectDisposedException)
            {
                break; // Sunucu durduruldu
            }

            _ = HandleClientAsync(tcp, ct);
        }
    }

    /// <summary>
    /// Tek bir istemcinin yaşam döngüsü:
    /// 1) Join mesajını bekle ve kullanıcı adını doğrula
    /// 2) Mesajları okuyup işle
    /// 3) Bağlantı kapanınca listeden çıkar ve herkese duyur
    /// </summary>
    private async Task HandleClientAsync(TcpClient tcp, CancellationToken ct)
    {
        var connection = new MessageConnection(tcp);
        ClientSession? session = null;

        try
        {
            // 1) İlk mesaj belirli süre içinde Join olmalı
            using var joinCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            joinCts.CancelAfter(JoinTimeout);

            ChatMessage? first = await connection.ReceiveAsync(joinCts.Token);
            if (first?.Type != MessageType.Join) return;

            string username = first.From.Trim();
            string? error = ChatProtocol.ValidateUsername(username);

            if (error == null)
            {
                lock (_lock)
                {
                    if (_clients.ContainsKey(username))
                    {
                        error = "Bu kullanıcı adı zaten kullanılıyor.";
                    }
                    else
                    {
                        session = new ClientSession(username, connection);
                        _clients.Add(username, session);
                    }
                }
            }

            if (session == null)
            {
                await connection.SendAsync(ChatMessage.CreateJoinRejected(error!));
                Log($"{connection.RemoteEndPoint} reddedildi: {error}");
                return;
            }

            await connection.SendAsync(ChatMessage.CreateJoinAccepted(username));
            Log($"{username} bağlandı ({connection.RemoteEndPoint}).");
            ClientsChanged?.Invoke(this, EventArgs.Empty);

            await BroadcastAsync(ChatMessage.CreateSystem($"{username} sohbete katıldı."));
            await BroadcastUserListAsync();

            // 2) Mesaj döngüsü
            while (!ct.IsCancellationRequested)
            {
                ChatMessage? message = await connection.ReceiveAsync(ct);
                if (message == null || message.Type == MessageType.Leave) break;

                await ProcessMessageAsync(session, message);
            }
        }
        catch (Exception ex) when (ex is IOException or SocketException
                                       or ObjectDisposedException or OperationCanceledException)
        {
            // Bağlantı koptu, zaman aşımı veya sunucu durduruldu: aşağıda temizlenir
        }
        finally
        {
            connection.Dispose();
            if (session != null)
            {
                await RemoveClientAsync(session, serverStopping: ct.IsCancellationRequested);
            }
        }
    }

    /// <summary>İstemciden gelen sohbet / özel mesajı işler.</summary>
    private async Task ProcessMessageAsync(ClientSession sender, ChatMessage message)
    {
        string text = ChatProtocol.NormalizeText(message.Text);
        if (text.Length == 0) return;

        sender.IncrementMessageCount();
        ClientsChanged?.Invoke(this, EventArgs.Empty);

        switch (message.Type)
        {
            case MessageType.Chat:
                Log($"{sender.Username}: {text}");
                // Gönderen kendi mesajını zaten ekranında gösteriyor, ona tekrar gönderilmez
                await BroadcastAsync(ChatMessage.CreateChat(sender.Username, text), exclude: sender);
                break;

            case MessageType.Private:
                ClientSession? target;
                lock (_lock)
                {
                    _clients.TryGetValue(message.To, out target);
                }

                if (target == null || target == sender)
                {
                    await TrySendAsync(sender, ChatMessage.CreateSystem($"'{message.To}' adlı kullanıcı bulunamadı."));
                    return;
                }

                // Özel mesajın içeriği sunucu günlüğüne yazılmaz
                Log($"{sender.Username} → {target.Username}: özel mesaj iletildi.");
                await TrySendAsync(target, ChatMessage.CreatePrivate(sender.Username, target.Username, text));
                break;
        }
    }

    private async Task RemoveClientAsync(ClientSession session, bool serverStopping)
    {
        bool removed;
        lock (_lock)
        {
            // Yalnızca listedeki kayıt GERÇEKTEN bu oturumsa sil
            removed = _clients.TryGetValue(session.Username, out var current)
                      && current == session
                      && _clients.Remove(session.Username);
        }

        if (!removed || serverStopping) return;

        Log($"{session.Username} ayrıldı.");
        ClientsChanged?.Invoke(this, EventArgs.Empty);
        await BroadcastAsync(ChatMessage.CreateSystem($"{session.Username} sohbetten ayrıldı."));
        await BroadcastUserListAsync();
    }

    // ------------------------------------------------------------------
    // Gönderim yardımcıları
    // ------------------------------------------------------------------

    /// <summary>Mesajı bağlı tüm istemcilere (isteğe bağlı biri hariç) gönderir.</summary>
    private async Task BroadcastAsync(ChatMessage message, ClientSession? exclude = null)
    {
        List<ClientSession> targets;
        lock (_lock)
        {
            targets = _clients.Values.Where(c => c != exclude).ToList();
        }

        await Task.WhenAll(targets.Select(c => TrySendAsync(c, message)));
    }

    private Task BroadcastUserListAsync()
    {
        List<string> names;
        lock (_lock)
        {
            names = _clients.Keys.OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToList();
        }
        return BroadcastAsync(ChatMessage.CreateUserList(names));
    }

    /// <summary>
    /// Gönderir; bağlantı kopmuşsa hatayı yutar. O istemci kendi okuma döngüsünde
    /// kopukluğu fark edip listeden çıkarılacaktır.
    /// </summary>
    private static async Task TrySendAsync(ClientSession client, ChatMessage message)
    {
        try
        {
            await client.Connection.SendAsync(message);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
        }
    }

    private void Log(string text) => LogMessage?.Invoke(this, text);
}
