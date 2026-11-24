namespace ChatClientApp;

internal static class Program
{
    /// <summary>İstemci uygulamasının giriş noktası.</summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ClientForm());
    }
}
