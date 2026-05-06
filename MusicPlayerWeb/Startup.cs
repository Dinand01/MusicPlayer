using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xilium.CefGlue;

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
            string directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            directory = directory.EndsWith("\\") ? directory : directory + "\\";
            
            // CefGlue: Initialize CEF runtime with correct arguments
            CefRuntime.Load();
            
            var settings = new CefSettings
            {
                // Disable logging for cleaner output
                LogSeverity = CefLogSeverity.Disable,
                // Set browser subprocess path
                BrowserSubprocessPath = Path.Combine(directory, "CefGlue.Avalonia.BrowserProcess.exe")
            };
            
            // Correct CefGlue initialization: CefMainArgs, CefSettings, CefApp, IntPtr
            CefRuntime.Initialize(new CefMainArgs(Environment.GetCommandLineArgs()), settings, null, IntPtr.Zero);
            
            System.Diagnostics.Debug.WriteLine("CefGlue initialized successfully");
        }
    }
}
