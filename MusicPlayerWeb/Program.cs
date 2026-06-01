using System;
using System.IO;
using Avalonia;
using Xilium.CefGlue;
using Xilium.CefGlue.Common;
using System.Collections.Generic;

namespace MusicPlayerWeb
{
    class Program
    {
        static bool cefInitialized = false;
        
        static int Main(string[] args)
        {
            try
            {
                // Generate a unique cache path to avoid problems when launching more than one process
                var cachePath = Path.Combine(Path.GetTempPath(), "MusicPlayerWeb_Cef_" + Guid.NewGuid().ToString().Replace("-", ""));
                
                AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };
                
                Console.WriteLine("Initializing CEF...");
                
                // Calculate path to BrowserProcess directory
                var browserProcessDir = Path.Combine(AppContext.BaseDirectory, "CefGlueBrowserProcess");
                var browserProcessPath = Path.Combine(browserProcessDir, "Xilium.CefGlue.BrowserProcess");
                
                Console.WriteLine($"BrowserProcess path: {browserProcessPath}");
                Console.WriteLine($"BrowserProcess exists: {File.Exists(browserProcessPath)}");
                Console.WriteLine($"Cache path: {cachePath}");
                
                 // Initialize CEF before building the app
                 try
                 {
                     var settings = new CefSettings()
                     {
                         RootCachePath = cachePath,
                         WindowlessRenderingEnabled = true, // required for off-screen/custom process
                         LogFile = Path.Combine(cachePath, "cef.log"),
                         LogSeverity = CefLogSeverity.Verbose,
                         BrowserSubprocessPath = browserProcessPath
                     };

                     var flags = new KeyValuePair<string, string>[]
                     {
                         new KeyValuePair<string, string>("disable-gpu", "1"),
                         new KeyValuePair<string, string>("no-sandbox", "1"),
                         new KeyValuePair<string, string>("in-process-gpu", "1"),
                         new KeyValuePair<string, string>("disable-software-rasterizer", "1"),
                         new KeyValuePair<string, string>("disable-network-service", "1"),
                         new KeyValuePair<string, string>("disable-features", "NetworkService,VizDisplayCompositor"),
                         new KeyValuePair<string, string>("enable-features", "NetworkServiceInProcess")
                     };

                     var customSchemes = new[]
                     {
                         new Xilium.CefGlue.Common.Shared.CustomScheme
                         {
                             SchemeName = "local",
                             SchemeHandlerFactory = new SchemeHandlerFactory(
                                 Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                         }
                     };

                     CefRuntimeLoader.Initialize(settings, flags, customSchemes);
                    
                    cefInitialized = true;
                    Console.WriteLine("CEF initialized successfully.");
                }
                catch (Exception cefEx)
                {
                    Console.WriteLine($"CEF initialization failed: {cefEx.Message}");
                    Console.WriteLine($"Stack trace: {cefEx.StackTrace}");
                    throw;
                }
                
                Console.WriteLine("Building Avalonia app...");
                
                AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .AfterSetup(_ => 
                      {
                          Console.WriteLine("AfterSetup callback called.");
                      })
                      .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start application: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 1;
            }
            
            return 0;
        }

        private static void Cleanup(string cachePath)
        {
            try
            {
                if (cefInitialized)
                {
                    CefRuntime.Shutdown();
                }
            }
            catch
            {
                // Ignore
            }

            try
            {
                var dirInfo = new DirectoryInfo(cachePath);
                if (dirInfo.Exists)
                {
                    dirInfo.Delete(true);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore
            }
            catch (IOException)
            {
                // Ignore
            }
        }
    }
}
