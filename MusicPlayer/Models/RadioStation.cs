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
        /// Default constructor.
        /// </summary>
        public RadioStation()
        {
        }

        /// <summary>
        /// Initializes the radio station with dirble data.
        /// </summary>
        /// <param name="station">The dirble station.</param>
        internal RadioStation(Dirble.RadioStation station)
        {
            this.Name = station.Name;
            this.Genre = station.Categories?.Aggregate(string.Empty, (res, cat) => res = string.IsNullOrEmpty(res) ? cat.Title : " ," + cat.Title) ?? string.Empty;
            this.Priority = 999;
            this.Url = station.Streams?.FirstOrDefault()?.Stream ?? string.Empty;
        }

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
