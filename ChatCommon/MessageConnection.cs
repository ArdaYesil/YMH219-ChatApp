using System.Net.Sockets;
using System.Text;

namespace ChatCommon;

/// <summary>
/// Bir TCP bağlantısını sarar ve ChatMessage gönderip almayı kolaylaştırır.
/// Hem sunucu (her istemci için bir tane) hem de istemci aynı sınıfı kullanır.
/// Okuma: StreamReader.ReadLineAsync, yazma: StreamWriter.WriteLineAsync.
/// </summary>
public sealed class MessageConnection : IDisposable
{
    private readonly TcpClient _client;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    // Aynı anda iki yerden yazılırsa satırlar birbirine karışmasın diye
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private bool _disposed;

    /// <summary>Karşı tarafın IP:Port bilgisi.</summary>
    public string RemoteEndPoint { get; }

    public MessageConnection(TcpClient client)
    {
        _client = client;
        NetworkStream stream = client.GetStream();
        var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        _reader = new StreamReader(stream, utf8);
        _writer = new StreamWriter(stream, utf8) { AutoFlush = true, NewLine = "\n" };
        RemoteEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "bilinmiyor";
    }

    /// <summary>Mesajı JSON satırı olarak gönderir.</summary>
    public async Task SendAsync(ChatMessage message)
    {
        await _writeLock.WaitAsync();
        try
        {
            await _writer.WriteLineAsync(message.ToJson());
        }
        finally
        {
            _writeLock.Release();
        }
    }

    /// <summary>
    /// Sıradaki mesajı bekler ve döndürür.
    /// Karşı taraf bağlantıyı kapattıysa null döner. Bozuk satırlar atlanır.
    /// </summary>
    public async Task<ChatMessage?> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            string? line = await _reader.ReadLineAsync(cancellationToken);
            if (line == null) return null; // Bağlantı kapandı

            if (string.IsNullOrWhiteSpace(line)) continue;

            ChatMessage? message = ChatMessage.FromJson(line);
            if (message != null) return message;
        }
    }

    /// <summary>Bağlantıyı kapatır. Birden fazla çağrılması sorun değildir.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _client.Close();
    }
}
