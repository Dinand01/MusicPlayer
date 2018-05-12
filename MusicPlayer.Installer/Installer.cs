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
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

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

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            try
            {
                string folder = savedState["TargetDir"].ToString();
                WriteLog("Commit target dir = " + folder);
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
