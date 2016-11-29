using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes the different content views.
    /// </summary>
    internal enum ViewType
    {
        /// <summary>
        /// The home view.
        /// </summary>
        Home,

        /// <summary>
        /// The audio playing view.
        /// </summary>
        Playing,

        /// <summary>
        /// The copy files view.
        /// </summary>
        Copy,

        /// <summary>
        /// The server view.
        /// </summary>
        Server, 

        /// <summary>
        /// The client view.
        /// </summary>
        Client
    }
}
