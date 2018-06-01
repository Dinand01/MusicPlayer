using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MusicPlayer.Installer
{
    /// <summary>
    /// Helps with registry entries.
    /// </summary>
    internal class RegistryHelper
    {
        /// <summary>
        /// The key folder.
        /// </summary>
        private const string _keyFolder = "Folder\\shell\\MusicPlayer";

        /// <summary>
        /// The key command.
        /// </summary>
        private const string _keyCommand = "Folder\\shell\\MusicPlayer\\command";

        /// <summary>
        /// Add an item to the context menu of a folder.
        /// </summary>
        /// <param name="name">The name of the context menu action.</param>
        /// <param name="path">The path of the executable.</param>
        public static void AddToContextMenu(string name, string path)
        {
            try
            {
                using (var regmenu = Registry.ClassesRoot.CreateSubKey(_keyFolder))
                using (var regcmd = Registry.ClassesRoot.CreateSubKey(_keyCommand))
                {
                    regmenu.SetValue(string.Empty, name);
                    regcmd.SetValue("", path);
                }
            }
            catch { }
        }


        /// <summary>
        /// Add an item to the context menu of a folder.
        /// </summary>
        public static void RemoveFromContextMenu()
        {
            try
            {
                using (var regmenu = Registry.ClassesRoot.CreateSubKey(_keyFolder))
                using (var regcmd = Registry.ClassesRoot.CreateSubKey(_keyCommand))
                {
                    if (regcmd != null)
                    {
                        regcmd.Close();
                        Registry.ClassesRoot.DeleteSubKey(_keyCommand);
                    }

                    if(regmenu != null)
                    {
                        regmenu.Close();
                        Registry.ClassesRoot.DeleteSubKey(_keyFolder);
                    }
                }
            }
            catch { }
        }
    }
}
