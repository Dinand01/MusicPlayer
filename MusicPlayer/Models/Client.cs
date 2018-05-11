using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Interface;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes a WCF client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the ip address of the client.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets teh port the client is connected to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sest the callback contract.
        /// </summary>
        public IClientContract ClientContract { get; set; }
    }
}
