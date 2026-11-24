namespace ChatServerApp;

internal static class Program
{
    /// <summary>Sunucu uygulamasının giriş noktası.</summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ServerForm());
    }
}
