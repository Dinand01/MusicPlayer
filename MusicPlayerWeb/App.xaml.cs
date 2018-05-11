using CefSharp;
using MusicPlayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            string directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            directory = directory.EndsWith("\\") ? directory : directory + "\\";
            try
            {
                CefSettings settings = new CefSettings();
                settings.RegisterScheme(new CefCustomScheme
                {
                    SchemeName = "custom",
                    SchemeHandlerFactory = new SchemeHandlerFactory(directory)
                });

                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Application startup failure");
                throw e;
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
    }
}
