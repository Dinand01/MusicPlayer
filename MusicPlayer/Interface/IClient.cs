using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// Describes the music player client.
    /// </summary>
    public interface IClient : IMusicPlayer, IDisposable
    {
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
    }
}
