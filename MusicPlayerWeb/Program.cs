using Avalonia;

namespace MusicPlayerWeb
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs in here.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}
