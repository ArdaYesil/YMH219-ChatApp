using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatCommon;

/// <summary>
/// Ağ üzerinden gönderilen tek bir mesaj. Her mesaj JSON'a çevrilip
/// TEK SATIR olarak gönderilir; karşı taraf satırı okuyup tekrar nesneye çevirir.
/// </summary>
public class ChatMessage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() } // Enum'lar "Chat" gibi okunabilir yazılsın
    };

    public MessageType Type { get; set; }
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Time { get; set; } = DateTime.Now;
    public List<string> Users { get; set; } = new();

    /// <summary>Nesneyi tek satırlık JSON metnine çevirir.</summary>
    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

    /// <summary>JSON satırını nesneye çevirir. Bozuk veri gelirse null döner (uygulama çökmez).</summary>
    public static ChatMessage? FromJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<ChatMessage>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // ---- Kolay oluşturma metotları (factory) ----

    public static ChatMessage CreateJoin(string username) =>
        new() { Type = MessageType.Join, From = username };

    public static ChatMessage CreateJoinAccepted(string username) =>
        new() { Type = MessageType.JoinAccepted, To = username };

    public static ChatMessage CreateJoinRejected(string reason) =>
        new() { Type = MessageType.JoinRejected, Text = reason };

    public static ChatMessage CreateChat(string from, string text) =>
        new() { Type = MessageType.Chat, From = from, Text = text };

    public static ChatMessage CreatePrivate(string from, string to, string text) =>
        new() { Type = MessageType.Private, From = from, To = to, Text = text };

    public static ChatMessage CreateSystem(string text) =>
        new() { Type = MessageType.System, From = "Sunucu", Text = text };

    public static ChatMessage CreateUserList(IEnumerable<string> users) =>
        new() { Type = MessageType.UserList, Users = users.ToList() };

    public static ChatMessage CreateLeave(string username) =>
        new() { Type = MessageType.Leave, From = username };
}
