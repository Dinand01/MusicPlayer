using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The video interface.
    /// </summary>
    public interface IVideo : IMusicPlayer, IDisposable
    {
        /// <summary>
        /// Start a video.
        /// </summary>
        /// <param name="url">The url of the video.</param>
        void StartVideo(string url);

        /// <summary>
        /// Stop the video and return the music player.
        /// </summary>
        /// <returns>The musicplayer.</returns>
        IMusicPlayer StopVideo();
    }
}
