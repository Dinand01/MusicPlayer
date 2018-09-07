using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer;
using MusicPlayer.Controller;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using Newtonsoft.Json;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Contains data retrieval or modification methods.
    /// </summary>
    public partial class MusicPlayerGate
    {
        /// <summary>
        /// Gets the songs.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="querry">The search querry.</param>
        /// <returns>A list of results in JSON format.</returns>
        public string GetSongs(int index, string querry)
        {
            var songs = _player?.GetSongs(index, querry);
            return JsonConvert.SerializeObject(songs);
        }

        /// <summary>
        /// Gets video info from a youtube playlist id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The serialized video info.</returns>
        public string GetVideoInfoFromPlaylist(string id)
        {
            var videoCtrl = Factory.GetVideoPlayer(_player) as IVideo;
            var task = videoCtrl.GetYoutubePlayList(id);
            task.Wait();
            return JsonConvert.SerializeObject(task.Result);
        }

        /// <summary>
        /// Gets youtube videos.
        /// </summary>
        /// <returns>The serialized video info.</returns>
        public string GetChannelVideos()
        {
            var videoCtrl = Factory.GetVideoPlayer(_player) as IVideo;
            var task = videoCtrl.GetYoutubeChannel();
            task.Wait();
            return JsonConvert.SerializeObject(task.Result);
        }

        /// <summary>
        /// Gets the default ip address.
        /// </summary>
        /// <returns>The default ip address.</returns>
        public string GetDefaultIP()
        {
            return DataController.GetSetting<string>(SettingType.RemoteIP, "127.0.0.1");
        }

        /// <summary>
        /// Gets the current song.
        /// </summary>
        /// <returns>The current song.</returns>
        public string GetCurrentSong()
        {
            var currentSong = _player?.GetCurrentSong();
            return JsonConvert.SerializeObject(currentSong);
        }

        /// <summary>
        /// Gets the shuffle setting.
        /// </summary>
        /// <returns>The shuffle setting.</returns>
        public bool? GetShuffle()
        {
            return _player?.GetShuffle();
        }

        /// <summary>
        /// Gets the current volume.
        /// </summary>
        /// <returns>The volume.</returns>
        public int? GetVolume()
        {
            return _player?.GetVolume();
        }

        /// <summary>
        /// Gets the radio stations.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>The stations.</returns>
        public string GetRadioStations(string searchText)
        {
            var stations = Task.Run(async () => await Factory.GetRadioInfo().GetStations(searchText)).Result;
            return JsonConvert.SerializeObject(stations);
        }

        /// <summary>
        /// Gets a radio station by it's id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The radio station.</returns>
        public string GetRadioStation(int id)
        {
            var station = Task.Run(async () => await Factory.GetRadioInfo().GetStation(id)).Result;
            return JsonConvert.SerializeObject(station);
        }

        /// <summary>
        /// Updates an existing radio station or creates a new one.
        /// </summary>
        /// <param name="jsonStation">The station data in json format.</param>
        public void UpdateOrCreateRadioStation(string jsonStation)
        {
            var station = JsonConvert.DeserializeObject<RadioStation>(jsonStation);
            if (station?.ID > 0)
            {
                Task.Run(async () => await Factory.GetRadioInfo().UpdateStation(station)).Wait();
            }
            else
            {
                Task.Run(async () => await Factory.GetRadioInfo().AddStations(station: station)).Wait();
            }
        }
    }
}
