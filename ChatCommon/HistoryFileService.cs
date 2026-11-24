using System.Text;

namespace ChatCommon;

/// <summary>Mesaj geçmişini / sunucu günlüğünü .txt dosyasına kaydeder.</summary>
public static class HistoryFileService
{
    /// <summary>
    /// Satırları UTF-8 olarak dosyaya yazar. Yazma hatasında IOException veya
    /// UnauthorizedAccessException fırlatır; hatayı kullanıcıya göstermek formun görevidir.
    /// </summary>
    public static void Save(string path, string title, IEnumerable<string> lines)
    {
        using var writer = new StreamWriter(path, append: false, Encoding.UTF8);
        writer.WriteLine(title);
        writer.WriteLine($"Kaydedilme: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        writer.WriteLine(new string('-', 60));
        foreach (string line in lines)
        {
            writer.WriteLine(line);
        }
    }
}
