namespace ChatCommon;

/// <summary>Sunucu ile istemci arasında gidip gelen mesajların türleri.</summary>
public enum MessageType
{
    Join,          // İstemci → Sunucu: "Bu kullanıcı adıyla katılmak istiyorum"
    JoinAccepted,  // Sunucu → İstemci: Katılım kabul edildi
    JoinRejected,  // Sunucu → İstemci: Katılım reddedildi (Text = sebep)
    Chat,          // Herkese açık sohbet mesajı
    Private,       // Özel mesaj (To = alıcı)
    System,        // Sunucunun bilgi mesajı ("Ali katıldı" gibi)
    UserList,      // Sunucu → İstemci: Çevrimiçi kullanıcı listesi
    Leave          // İstemci → Sunucu: "Ayrılıyorum"
}
