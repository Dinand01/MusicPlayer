using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Contains the color pallette for the application.
    /// </summary>
    internal static class ColorPallete
    {
        /// <summary>
        /// The availible colors.
        /// </summary>
        public enum Color
        {
            Primary0,

            Primary1,

            Primary2,

            Primary3,

            Primary4
        }

        /// <summary>
        /// The dictionary conatining the colors.
        /// </summary>
        public static Dictionary<Color, Eto.Drawing.Color> Colors = new Dictionary<Color, Eto.Drawing.Color>
        {
            {Color.Primary0, CreaterColor(0.173, 0.286, 0.439)},
            {Color.Primary1, CreaterColor(0.361, 0.569, 0.898)},
            {Color.Primary2, CreaterColor(0.251, 0.396, 0.627)},
            {Color.Primary3, CreaterColor(0.098, 0.161, 0.255)},
            {Color.Primary4, CreaterColor(0.023, 0.043, 0.067)}
        };

        /// <summary>
        /// Creates a color.
        /// </summary>
        /// <param name="red">The red value.</param>
        /// <param name="green">The green value.</param>
        /// <param name="blue">The blue value.</param>
        /// <returns>The color.</returns>
        private static Eto.Drawing.Color CreaterColor(double red, double green, double blue)
        {
            return new Eto.Drawing.Color((float)red, (float)green, (float)blue);
        }

        /// <summary>
        /// Converts a color to a brush.
        /// </summary>
        /// <param name="col">The color.</param>
        /// <returns>The solid brush.</returns>
        public static System.Windows.Media.SolidColorBrush ToBrush(this Eto.Drawing.Color col)
        {
            return new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color
            {
                ScA = col.A,
                ScB = col.B,
                ScG = col.G,
                ScR = col.R
            });
        }
    }
}
