using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The video controller class.
    /// </summary>
    internal class VideoController : MusicPlayerWrapper, IVideo
    {
        /// <summary>
        /// The music player.
        /// </summary>
        private new IMusicPlayer _player;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoController" /> class. 
        /// </summary>
        /// <param name="player">The music player.</param>
        public VideoController(IMusicPlayer player) : base(player)
        {
            _player = player;
        }

        /// <summary>
        /// Start a video.
        /// </summary>
        /// <param name="url">The url of the video.</param>
        public void StartVideo(string url)
        {
            _player?.TogglePlay(true);
            IServer server = _player as IServer;
            if (server != null)
            {
                server.GetInfo().VideoUrl = url;
                server.SendMessage<string>(MessageType.Pause);
                server.SendMessage<string>(MessageType.Video, url);
            }
        }

        /// <summary>
        /// Stop the video.
        /// </summary>
        /// <returns>The music player.</returns>
        public IMusicPlayer StopVideo()
        {
            IServer server = _player as IServer;
            if (server != null)
            {
                server.GetInfo().VideoUrl = null;
                server.SendMessage<string>(MessageType.Video, string.Empty);
                server.SendMessage<string>(MessageType.Play, null);
            }

            return _player;
        }
    }
}
