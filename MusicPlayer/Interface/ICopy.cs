using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The progress changed.
    /// </summary>
    /// <param>The percentage.</param>
    public delegate void ProgressChanged(double percentage);

    /// <summary>
    /// The copy interface.
    /// </summary>
    public interface ICopy
    {
        /// <summary>
        /// The progress changed event.
        /// </summary>
        event ProgressChanged ProgressChanged;

        /// <summary>
        /// Copies random song from the sourceLoc to the destination loc.
        /// </summary>
        /// <param name="sourceLoc">The source location.</param>
        /// <param name="destinationLoc">The destination location.</param>
        /// <param name="amount">The amount of songs to copy.</param>
        void CopyRandomSongs(string sourceLoc, string destinationLoc, int amount);
    }
}
