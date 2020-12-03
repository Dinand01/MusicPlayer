using MusicPlayer.Core.Player;
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
    public interface IServer : IMusicPlayer, INetwork
    {
    }
}
