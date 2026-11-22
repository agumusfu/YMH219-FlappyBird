namespace FlappyBird;

static class Program
{
    /// <summary>
    /// Uygulamanın ana giriş noktası.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }    
}
