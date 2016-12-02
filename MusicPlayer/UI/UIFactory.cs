using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.UI
{
    /// <summary>
    /// Factory class for generating UI's.
    /// </summary>
    internal static class UIFactory
    {
        /// <summary>
        /// creates the ETO based UI.
        /// </summary>
        ///<param name="args">The arguments.</param>
        public static void CreateEtoUI(string[] args = null)
        {
            new Application().Run(new EtoUI(args));
        }
    }
}
