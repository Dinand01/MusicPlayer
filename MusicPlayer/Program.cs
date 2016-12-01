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
using System.Data.Entity.Infrastructure;
using MusicPlayer.UI;

namespace MusicPlayer
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Import the embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args2) =>
            {
                string originalAssemblyName = new AssemblyName(args2.Name).Name + ".dll";
                string resourceName = "MusicPlayer.Packages." + originalAssemblyName;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        Byte[] assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        try
                        {
                            return Assembly.Load(assemblyData);
                        }
                        catch
                        {
                            using (FileStream fsDst = new FileStream(originalAssemblyName, FileMode.Create, FileAccess.Write))
                            {
                                fsDst.Write(assemblyData, 0, assemblyData.Length);
                            }

                            try
                            {
                                string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                                location = (location.EndsWith("\\") ? location : location + "\\") + originalAssemblyName;
                                return Assembly.LoadFrom(location);
                            }
                            catch (Exception e)
                            {
                                return Assembly.Load(assemblyData);
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            };

            InstallSQLServerCompact();
            UIFactory.CreateEtoUI();
            ////Application.Run(new MusicPlayer(args));
        }

        /// <summary>
        /// Launches the installer for SQL server compact when it is not installed.
        /// Creates the connection factory.
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

            string directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            directory = directory.Replace("file:\\", "");
            var dataSource = "Data Source=" + directory + "\\MusicPlayer.DAL.DbContext.sdf;Persist Security Info=False;";
            Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0", directory, dataSource);
            var test = dataSource;
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
