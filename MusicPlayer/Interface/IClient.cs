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
    public interface IClient : IMusicPlayer, INetwork
    {
    }
}
