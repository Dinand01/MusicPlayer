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
        static int Main(string[] args)
        {
            try
            {
                // Generate a unique cache path to avoid problems when launching more than one process
                var cachePath = Path.Combine(Path.GetTempPath(), "MusicPlayerWeb_Cef_" + Guid.NewGuid().ToString().Replace("-", ""));
                
                AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };
                
                AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .AfterSetup(_ => 
                      {
                          CefRuntimeLoader.Initialize(new CefSettings()
                          {
                              RootCachePath = cachePath,
                              WindowlessRenderingEnabled = false,
                              LogFile = Path.Combine(cachePath, "cef.log"),
                              LogSeverity = CefLogSeverity.Disable
                          },
                          flags: new KeyValuePair<string, string>[]
                          {
                              new KeyValuePair<string, string>("disable-gpu", "1"),
                              new KeyValuePair<string, string>("no-sandbox", "1"),
                              new KeyValuePair<string, string>("in-process-gpu", "1"),
                              new KeyValuePair<string, string>("disable-software-rasterizer", "1"),
                              new KeyValuePair<string, string>("disable-network-service", "1"),
                              new KeyValuePair<string, string>("disable-features", "NetworkService"),
                              new KeyValuePair<string, string>("enable-features", "NetworkServiceInProcess")
                          },
                          customSchemes: new[]
                          {
                              new Xilium.CefGlue.Common.Shared.CustomScheme()
                              {
                                  SchemeName = "local",
                                  SchemeHandlerFactory = new SchemeHandlerFactory(
                                      Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                              }
                          });
                      })
                      .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start application: {ex.Message}");
                return 1;
            }
            
            return 0;
        }

        private static void Cleanup(string cachePath)
        {
            try
            {
                CefRuntime.Shutdown();
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
