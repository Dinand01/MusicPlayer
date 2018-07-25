using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes a radio station.
    /// </summary>
    public class RadioStation
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The priority of the station, higher will appear first.
        /// </summary>
        public int Priority { get; set; }
    }
}
