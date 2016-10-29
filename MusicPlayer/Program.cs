using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MusicPlayer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Import the embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args2) =>
            {
                String resourceName = "MusicPlayer.Packages." + new AssemblyName(args2.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InstallSQLServerCompact();
            Application.Run(new MusicPlayer(args));
        }

        /// <summary>
        /// Launches the installer for SQL server compact when it is not installed.
        /// </summary>
        private static void InstallSQLServerCompact()
        {
            if (!IsSQLServerCompactInstalled())
            {
                string tempFileName = "SSCERuntime_x64-ENU.exe";
                string resourceName = "MusicPlayer.Packages." + tempFileName;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);

                    using (FileStream fsDst = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
                    {
                        fsDst.Write(assemblyData, 0, assemblyData.Length);
                    }

                    var installer = Process.Start(tempFileName);
                    installer.WaitForExit();
                    File.Delete(tempFileName);
                }
            }
        }

        /// <summary>
        /// Checks if SQL server Compact is installed on the pc.
        /// </summary>
        /// <returns>A boolean indicating whether sql server compact is installed.</returns>
        private static bool IsSQLServerCompactInstalled()
        {
            try
            {
                var path = Assembly.GetAssembly(typeof(System.Data.SqlServerCe.SqlCeException)).CodeBase.Substring(8);
                if (File.Exists(path))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
