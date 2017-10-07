using MusicPlayer.Controller;
using MusicPlayer.Interface;
using MusicPlayer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    /// <summary>
    /// The factory for music players.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Gets a standard music player.
        /// </summary>
        /// <returns>The music player.</returns>
        public static IMusicPlayer GetPlayer()
        {
            return new Player();
        }

        /// <summary>
        /// Gets a standard music player for receive mode.
        /// </summary>
        /// <returns>The music player.</returns>
        internal static IMusicPlayer GetPlayerForReceiveMode()
        {
            return new Player(true);
        }

        /// <summary>
        /// Gets a player with server capabilities.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="player">The optional current music player.</param>
        /// <returns>The music player with server capabilities.</returns>
        public static IMusicPlayer GetServerPlayer(int port, IMusicPlayer player = null)
        {
            if (player == null)
            {
                player = new Player();
            }

            return new NetworkServer(player, port);
        }

        /// <summary>
        /// Gets a player with client capabilities.
        /// </summary>
        /// <param name="ip">The ip address.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="player">The optional current music player.</param>
        /// <returns>The music player with server capabilities.</returns>
        public static IMusicPlayer GetClientPlayer(IPAddress ip, int port, IMusicPlayer player = null)
        {
            player?.Dispose();
            var newplayer = new NetworkClient(ip, port);
            return newplayer;
        }

        /// <summary>
        /// Gets the video player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The video player.</returns>
        public static IMusicPlayer GetVideoPlayer(IMusicPlayer player = null)
        {
            var p = player as IVideo;
            return p != null ? p : new VideoController(player);
        }

        /// <summary>
        /// Gets a copy controller.
        /// </summary>
        /// <returns>The copy controller.</returns>
        public static ICopy GetCopy()
        {
            return new CopyController();
        }
    }
}
