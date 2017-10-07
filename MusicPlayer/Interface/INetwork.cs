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
    }
}
