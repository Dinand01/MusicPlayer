using MusicPlayer;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initialize CefSharp.
        /// </summary>
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            try
            {
                MusicPlayerWeb.Startup.Start();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Application startup failure");
                RunResource("vcredist_x64_(1).exe");
                RunResource("vcredist_x64_(2).exe");
                MusicPlayerWeb.Startup.Start();
            }
        }

        /// <summary>
        /// Logs unhandled exceptions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogError(e.Exception, "Application failure");
        }

        /// <summary>
        /// Runs a resource.
        /// </summary>
        /// <param name="filename">The embedded file.</param>
        private void RunResource(string filename)
        {
            var assembly = typeof(App).Assembly;
            var resourceName = "MusicPlayerWeb.redist." + filename;
            string dir = Path.GetTempPath();
            dir = dir.EndsWith("\\") ? dir : dir + "\\";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (FileStream fs = new FileStream(dir + filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.CopyTo(fs);
                }

                using (var pr = Process.Start(dir + filename, "/install /quiet /norestart"))
                {
                    pr.WaitForExit();
                    File.Delete(dir + filename);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Installation of resource failed");
            }
        }
    }
}
