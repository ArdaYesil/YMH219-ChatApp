using ChatCommon;

namespace ChatServerApp;

/// <summary>Sunucuya bağlı bir istemcinin bilgileri ve bağlantısı.</summary>
public class ClientSession
{
    private int _messageCount;

    public string Username { get; }
    public MessageConnection Connection { get; }
    public string EndPoint => Connection.RemoteEndPoint;
    public DateTime ConnectedAt { get; } = DateTime.Now;

    /// <summary>Bu istemcinin gönderdiği mesaj sayısı.</summary>
    public int MessageCount => _messageCount;

    public ClientSession(string username, MessageConnection connection)
    {
        Username = username;
        Connection = connection;
    }

    /// <summary>Mesaj sayacını iş parçacığı güvenli şekilde artırır.</summary>
    public void IncrementMessageCount() => Interlocked.Increment(ref _messageCount);
}
