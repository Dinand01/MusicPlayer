using MusicPlayer.Core.Models;
using System;

namespace MusicPlayer.Communication
{
    public interface IMusicPlayerServer
    {
        /// <summary>
        /// A client disconnected.
        /// </summary>
        event Action<string> ClientDisconnected;

        /// <summary>
        /// A client connected.
        /// </summary>
        event Action<Client> ClientConnected;
    }
}
