using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.Installer
{
    /// <summary>
    /// The installer class.
    /// </summary>
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Initialize the installer.
        /// </summary>
        public Installer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start the installation.
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            try
            {
                WriteLog("Target dir: " + Context.Parameters["TargetDir"].ToString());
                stateSaver.Add("TargetDir", Context.Parameters["TargetDir"].ToString());
            }
            catch (Exception e)
            {
                WriteLog("Error in install event: " + e.ToString());
                throw e;
            }
        }

        /// <summary>
        /// Commit the installation.
        /// </summary>
        /// <param name="savedState"></param>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            try
            {
                string folder = savedState["TargetDir"].ToString();
                WriteLog("Commit target dir = " + folder);
                AddToContextMenu(folder);
                SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
                FileSystemAccessRule writerule = new FileSystemAccessRule(sid, FileSystemRights.Write | FileSystemRights.ReadAndExecute | FileSystemRights.CreateFiles, AccessControlType.Allow);

                if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                {
                    DirectorySecurity fsecurity = Directory.GetAccessControl(folder);
                    fsecurity.AddAccessRule(writerule);
                    Directory.SetAccessControl(folder, fsecurity);
                }
            }
            catch (Exception e)
            {
                WriteLog("Error in commit event: " + e.ToString());
                throw e;
            }

            base.Commit(savedState);
        }

        /// <summary>
        /// Unitstall the application.
        /// </summary>
        /// <param name="savedState"></param>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistryHelper.RemoveFromContextMenu();
        }

        /// <summary>
        /// Adds the option to open a folder to the context menu.
        /// </summary>
        /// <param name="folder">The install directory.</param>
        private void AddToContextMenu(string folder)
        {
            folder = folder.EndsWith("\\\\") ? folder.Replace("\\\\", "\\") : folder;
            folder = folder.EndsWith("\\") ? folder : (folder + "\\");
            string path = folder + "MusicPlayerWeb.exe \"%L%\"";
            RegistryHelper.AddToContextMenu("Open with Music Player", path);
        }

        /// <summary>
        /// Write a log message.
        /// </summary>
        /// <param name="message"></param>
        private void WriteLog(string message)
        {
            try
            {
                string path = "C:\\Temp\\MusicPlayer_Install_Log.txt";
                Directory.CreateDirectory("C:\\Temp\\");
                if (!File.Exists(path))
                {
                    File.Create(path);
                }

                File.AppendAllLines(path, new string[] { message });
            }
            catch { }
        }
    }
}
