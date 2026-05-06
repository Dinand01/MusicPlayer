using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MusicPlayer;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Avalonia Application class for MusicPlayerWeb
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initializes the application.
        /// </summary>
        public override void Initialize()
        {
            // Avalonia automatically loads XAML for Application class
            // No need to call AvaloniaXamlLoader.Load(this) here
            base.Initialize();
        }

        /// <summary>
        /// Called when framework initialization is completed.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            try
            {
                EnsureExecutingDirectoryIsExecutableDirectory();
                MusicPlayerWeb.Startup.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Startup failure: {e.Message}");
                // Try again without ensuring directory
                try
                {
                    MusicPlayerWeb.Startup.Start();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Second startup failure: {ex.Message}");
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Ensures the executing directory is correct.
        /// </summary>
        private static void EnsureExecutingDirectoryIsExecutableDirectory()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            string directory = new FileInfo(location.LocalPath).Directory.FullName;
            Directory.SetCurrentDirectory(directory);
        }
    }
}
