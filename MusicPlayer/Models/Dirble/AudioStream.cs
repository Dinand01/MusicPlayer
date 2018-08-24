using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models.Dirble
{
    internal class AudioStream
    {
        public string Stream { get; set; }
        public int Bitrate { get; set; }
        public string Content_type { get; set; }
        public int Listeners { get; set; }
        public bool Status { get; set; }
    }
}
