namespace ChatCommon;

/// <summary>Sunucu ve istemcinin ortak kullandığı sabitler ve kurallar.</summary>
public static class ChatProtocol
{
    public const int DefaultPort = 5000;
    public const int MaxUsernameLength = 20;
    public const int MaxMessageLength = 500;

    /// <summary>
    /// Kullanıcı adını kontrol eder. Geçerliyse null, değilse hata mesajı döner.
    /// Hem istemci (göndermeden önce) hem sunucu (güvenmeden önce) kullanır.
    /// </summary>
    public static string? ValidateUsername(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return "Kullanıcı adı boş olamaz.";

        if (username.Length > MaxUsernameLength)
            return $"Kullanıcı adı en fazla {MaxUsernameLength} karakter olabilir.";

        if (!username.All(c => char.IsLetterOrDigit(c) || c is '_' or '-' or '.'))
            return "Kullanıcı adında yalnızca harf, rakam, '_', '-' ve '.' kullanılabilir.";

        if (username.Equals("Sunucu", StringComparison.OrdinalIgnoreCase) ||
            username.Equals("Herkes", StringComparison.OrdinalIgnoreCase))
            return $"'{username}' adı ayrılmıştır, başka bir ad seçin.";

        return null;
    }

    /// <summary>Mesajı kırpar ve izin verilen uzunluğa indirir.</summary>
    public static string NormalizeText(string? text)
    {
        string trimmed = (text ?? "").Trim();
        return trimmed.Length > MaxMessageLength ? trimmed[..MaxMessageLength] : trimmed;
    }
}
