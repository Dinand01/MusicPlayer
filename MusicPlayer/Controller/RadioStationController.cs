using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Helpers;
using MusicPlayer.Models;

namespace MusicPlayer.Controller
{
    internal class RadioStationController
    {
        public List<RadioStation> GetStations(string searchText)
        {
            throw new NotImplementedException("TODO");
        }

        private async Task<List<RadioStation>> GetFromDirble()
        {
            try
            {
                using (var client = new DirbleClient())
                {
                    return (await client.GetPopularStations())?.Select(s => new RadioStation(s)).ToList();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Dirble radio station retrieval failed.");
                return null;
            }
        }
    }
}
