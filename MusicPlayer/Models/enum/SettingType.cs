using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes the posible settings.
    /// </summary>
    public enum SettingType
    {
        /// <summary>
        /// The default remote ip to connect to.
        /// </summary>
        RemoteIP, 

        /// <summary>
        /// The last set volume.
        /// </summary>
        Volume,

        /// <summary>
        /// Shuffle the songs.
        /// </summary>
        Shuffle
    }
}
