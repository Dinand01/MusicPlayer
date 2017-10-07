using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// The message type enumeration, describes the different types of messages.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// File transfer data.
        /// </summary>
        Data,

        /// <summary>
        /// New file transfer.
        /// </summary>
        NewSong,

        /// <summary>
        /// File transfer complete.
        /// </summary>
        EndOfSong,

        /// <summary>
        /// Go to time.
        /// </summary>
        Goto,

        /// <summary>
        /// Notification to display.
        /// </summary>
        Notification,

        /// <summary>
        /// Pause the music.
        /// </summary>
        Pause,

        /// <summary>
        /// Play the music.
        /// </summary>
        Play,

        /// <summary>
        /// Send a video.
        /// </summary>
        Video
    }
}
