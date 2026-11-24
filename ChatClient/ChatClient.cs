using System.Net.Sockets;
using ChatCommon;

namespace ChatClientApp;

/// <summary>
/// Sunucuya TcpClient ile bağlanan, mesaj gönderen ve gelen mesajları dinleyen istemci.
/// Arayüzden bağımsızdır; gelen mesajları MessageReceived olayıyla bildirir.
/// </summary>
public class ChatClient
{
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

    private MessageConnection? _connection;
    private CancellationTokenSource? _cts;

    public string Username { get; private set; } = "";
    public bool IsConnected => _connection != null;

    /// <summary>Sunucudan bir mesaj geldi.</summary>
    public event EventHandler<ChatMessage>? MessageReceived;

    /// <summary>Bağlantı kullanıcı istemeden koptu (parametre: sebep).</summary>
    public event EventHandler<string>? ConnectionLost;

    /// <summary>
    /// Sunucuya bağlanır ve kullanıcı adıyla katılır.
    /// Başarısız olursa anlaşılır mesajlı ChatConnectionException fırlatır.
    /// </summary>
    public async Task ConnectAsync(string host, int port, string username)
    {
        if (IsConnected) throw new ChatConnectionException("Zaten bağlısınız.");

        var tcp = new TcpClient();
        MessageConnection? connection = null;
        try
        {
            using var timeout = new CancellationTokenSource(ConnectTimeout);

            await tcp.ConnectAsync(host, port, timeout.Token);
            connection = new MessageConnection(tcp);

            // Katılma isteği gönder ve cevabı bekle
            await connection.SendAsync(ChatMessage.CreateJoin(username));
            ChatMessage? reply = await connection.ReceiveAsync(timeout.Token);

            if (reply == null)
                throw new ChatConnectionException("Sunucu bağlantıyı kapattı.");
            if (reply.Type == MessageType.JoinRejected)
                throw new ChatConnectionException("Sunucu bağlantıyı reddetti: " + reply.Text);
            if (reply.Type != MessageType.JoinAccepted)
                throw new ChatConnectionException("Sunucudan beklenmeyen cevap geldi.");

            _connection = connection;
            _cts = new CancellationTokenSource();
            Username = username;

            _ = ReceiveLoopAsync(connection, _cts.Token); // Arka planda dinlemeye başla
        }
        catch (Exception ex) when (ex is not ChatConnectionException)
        {
            Cleanup(connection, tcp);
            throw new ChatConnectionException(DescribeConnectError(ex, host, port), ex);
        }
        catch
        {
            Cleanup(connection, tcp);
            throw;
        }
    }

    /// <summary>Herkese açık mesaj gönderir.</summary>
    public Task SendChatAsync(string text) =>
        SendAsync(ChatMessage.CreateChat(Username, text));

    /// <summary>Belirli bir kullanıcıya özel mesaj gönderir.</summary>
    public Task SendPrivateAsync(string to, string text) =>
        SendAsync(ChatMessage.CreatePrivate(Username, to, text));

    /// <summary>Sunucuya ayrıldığını bildirir ve bağlantıyı kapatır.</summary>
    public async Task DisconnectAsync()
    {
        MessageConnection? connection = _connection;
        if (connection == null) return;

        _connection = null;
        _cts?.Cancel(); // Okuma döngüsü "kullanıcı kapattı" olarak bitsin

        try
        {
            await connection.SendAsync(ChatMessage.CreateLeave(Username));
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            // Sunucu zaten gitmişse önemli değil
        }

        connection.Dispose();
    }

    // ------------------------------------------------------------------

    private async Task SendAsync(ChatMessage message)
    {
        MessageConnection connection = _connection
            ?? throw new ChatConnectionException("Sunucuya bağlı değilsiniz.");

        try
        {
            await connection.SendAsync(message);
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            throw new ChatConnectionException("Mesaj gönderilemedi: sunucu bağlantısı koptu.", ex);
        }
    }

    /// <summary>Sunucudan gelen mesajları bağlantı kapanana kadar okur.</summary>
    private async Task ReceiveLoopAsync(MessageConnection connection, CancellationToken ct)
    {
        string? lostReason = null;
        try
        {
            while (true)
            {
                ChatMessage? message = await connection.ReceiveAsync(ct);
                if (message == null)
                {
                    lostReason = "Sunucu bağlantıyı kapattı.";
                    break;
                }
                MessageReceived?.Invoke(this, message);
            }
        }
        catch (Exception ex) when (ex is IOException or SocketException
                                       or ObjectDisposedException or OperationCanceledException)
        {
            // Kullanıcı kendisi kestiyse (ct iptal) bu bir hata değildir
            if (!ct.IsCancellationRequested) lostReason = "Sunucu ile bağlantı koptu.";
        }

        if (ct.IsCancellationRequested) return;

        // Bağlantı istenmeden koptu: temizle ve haber ver
        if (_connection == connection) _connection = null;
        connection.Dispose();
        ConnectionLost?.Invoke(this, lostReason ?? "Bağlantı kapandı.");
    }

    private static void Cleanup(MessageConnection? connection, TcpClient tcp)
    {
        connection?.Dispose();
        tcp.Dispose();
    }

    private static string DescribeConnectError(Exception ex, string host, int port) => ex switch
    {
        OperationCanceledException =>
            $"Sunucu {ConnectTimeout.TotalSeconds:0} saniye içinde yanıt vermedi (zaman aşımı).",
        SocketException { SocketErrorCode: SocketError.ConnectionRefused } =>
            $"{host}:{port} adresinde çalışan bir sunucu bulunamadı. Sunucu başlatıldı mı, port doğru mu?",
        SocketException { SocketErrorCode: SocketError.HostNotFound or SocketError.NoData } =>
            $"'{host}' adresi bulunamadı. IP adresini kontrol edin.",
        SocketException se => $"Sunucuya bağlanılamadı: {se.Message}",
        _ => $"Sunucuya bağlanılamadı: {ex.Message}"
    };
}
