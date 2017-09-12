using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// Describes a server info change.
    /// </summary>
    public delegate void ServerInfoChanged(ServerInfo serverInfo);

    /// <summary>
    /// The interface describing a music server.
    /// </summary>
    public interface IServer : IMusicPlayer, IDisposable
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
    }
}
