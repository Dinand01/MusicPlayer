using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using MusicPlayer;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Class that allows the application to start when the browser process fails (log problem).
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Will start the browser process.
        /// </summary>
        public static void Start()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var missingDeps = CefSharp.DependencyChecker.CheckDependencies(true, false, dir, string.Empty, Path.Combine(dir, "CefSharp.BrowserSubprocess.exe"));
            if (missingDeps?.Count > 0)
            {
                Logger.LogInfo("Missing dependancies:" + missingDeps.Aggregate((r, s) => r + ", " + s));
            }

            string directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            directory = directory.EndsWith("\\") ? directory : directory + "\\";
            CefSettings settings = new CefSettings();
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "custom",
                SchemeHandlerFactory = new SchemeHandlerFactory(directory)
            });

            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }
    }
}
