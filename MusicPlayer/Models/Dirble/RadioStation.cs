using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models.Dirble
{
    /// <summary>
    /// The dirble model for a radio station.
    /// </summary>
    internal class RadioStation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public Image Image { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public string Slug { get; set; }
        public AudioStream[] Streams { get; set; }
        public Category[] Categories { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
    }
}
