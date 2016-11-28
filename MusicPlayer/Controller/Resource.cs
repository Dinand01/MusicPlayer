using Eto.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Class containing methods for accessing resources.
    /// </summary>
    internal class Resource
    {
        /// <summary>
        /// Gets an image resource.
        /// </summary>
        /// <param name="assetName">The image file name.</param>
        /// <returns>A bitmap.</returns>
        public static Bitmap GetImage(string assetName)
        {
            var image = new Bitmap(Assembly.GetEntryAssembly().GetManifestResourceStream("MusicPlayer.Resources." + assetName));
            return image;
        }
    }
}
