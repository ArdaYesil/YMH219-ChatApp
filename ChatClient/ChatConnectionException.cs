namespace ChatClientApp;

/// <summary>
/// Bağlantı ile ilgili, kullanıcıya doğrudan gösterilebilecek anlaşılır bir hata.
/// Teknik istisnalar (SocketException, IOException...) bu sınıfa çevrilir.
/// </summary>
public class ChatConnectionException : Exception
{
    public ChatConnectionException(string message, Exception? inner = null)
        : base(message, inner)
    {
    }
}
