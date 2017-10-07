using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes the server info.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// Gets or sets the host ip address.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gest or sets a value indicating whether we are host. 
        /// </summary>
        public bool IsHost { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The connected clients ip: port.
        /// </summary>
        public Dictionary<string, int> Clients { get; set; }

        /// <summary>
        /// Gets the number of connected clients.
        /// </summary>
        public int Count
        {
            get
            {
                int? count = Clients?.Keys?.Count;
                return count == null ? 0 : (int)count;
            }
        }
        
        /// <summary>
        /// In client mode this is used to play a video.
        /// </summary>
        public string VideoUrl { get; set; }
    }
}
