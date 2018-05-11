using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// Common methods for client and server.
    /// </summary>
    public interface INetwork : IDisposable
    {
        /// <summary>
        /// The server info changed event.
        /// </summary>
        event ServerInfoChanged OnInfoChanged;

        /// <summary>
        /// Disconnect the server.
        /// </summary>
        /// <returns>The music player.</returns>
        IMusicPlayer Disconnect();

        /// <summary>
        /// Gets the server info.
        /// </summary>
        /// <returns></returns>
        ServerInfo GetInfo();

        /// <summary>
        /// Sets the video.
        /// </summary>
        /// <param name="url">The url of the video to set.</param>
        void SetVideo(string url);

        /// <summary>
        /// Seek to a position in  the video.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        void SeekVideo(double position);
    }
}
