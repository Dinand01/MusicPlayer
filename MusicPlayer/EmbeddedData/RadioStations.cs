using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;

namespace MusicPlayer.EmbeddedData
{
    /// <summary>
    /// Contains static data about radio stations.
    /// </summary>
    internal static class RadioStations
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The data.</returns>
        public static List<RadioStation> Get()
        {
            return new List<RadioStation>
            {
                new RadioStation
                {
                    Url = "http://pulseedm.cdnstream1.com:8124/1373_128",
                    Name = "PulseEDM Dance Music Radio",
                    Genre = "EDM"
                },
                new RadioStation
                {
                    Url = "http://uk2.internet-radio.com:8024/;stream",
                    Name = "Dance UK",
                    Genre = "Dance, trance, elektro"
                },
                new RadioStation
                {
                    Url = "http://uk2.internet-radio.com:8358/;stream",
                    Name = "1FM - 60s/70s/80s/90s/00s!"
                },
                new RadioStation
                {
                    Url = "http://us3.internet-radio.com:8264/;stream",
                    Name = "FX Alternative Radio",
                }
            };
        }
    }
}
