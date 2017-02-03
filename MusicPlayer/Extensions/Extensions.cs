using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Extensions
{
    /// <summary>
    /// Class containing extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension method to convert a string to an int.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="def">The default value.</param>
        /// <returns>The converted int.</returns>
        public static int AsInt(this string str, int def = 0)
        {
            int.TryParse(str, out def);
            return def;
        }

        /// <summary>
        /// Converts a string to a boolean.
        /// </summary>
        /// <param name="str">The str to convert.</param>
        /// <param name="def">The default value when conversion fails.</param>
        /// <returns>The boolean.</returns>
        public static bool AsBoolean(this string str, bool def = false)
        {
            bool.TryParse(str, out def);
            return def;
        }
    }
}
