using MusicPlayer.Extensions;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using IVideo = MusicPlayer.Interface.IVideo;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The video controller class.
    /// </summary>
    internal class VideoController : MusicPlayerWrapper, IVideo
    {
        /// <summary>
        /// The music player.
        /// </summary>
        private new IMusicPlayer _player;

        /// <summary>
        /// The thread that will send ocasional messages.
        /// </summary>
        private Thread _refreshClientInfo;

        /// <summary>
        /// The current video url.
        /// </summary>
        private string _videoUrl;

        /// <summary>
        /// The reference of the video.
        /// </summary>
        private DateTime _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoController" /> class. 
        /// </summary>
        /// <param name="player">The music player.</param>
        public VideoController(IMusicPlayer player) : base(player)
        {
            _player = player;
        }

        /// <summary>
        /// Start a video.
        /// </summary>
        /// <param name="url">The url of the video.</param>
        public void StartVideo(string url)
        {
            _player?.TogglePlay(true);
            IServer server = _player as IServer;
            _videoUrl = url;
            _refreshClientInfo?.Abort();
            _refreshClientInfo = new Thread(() => CheckTime());
            _refreshClientInfo.Start();
            if (server != null)
            {
                server.GetInfo().VideoUrl = url;
                server.TogglePlay(true);
                server.SetVideo(url);
            }
        }

        /// <summary>
        /// Seek to a specific position.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        public void Seek(double position)
        {
            IServer server = _player as IServer;
            _started = DateTime.Now - TimeSpan.FromSeconds(position);
            if (server != null)
            {
                server.SeekVideo(position);
            }
        }

        /// <summary>
        /// Gets the youtube channels video's.
        /// </summary>
        /// <param name="id">The id of the youtube channel.</param>
        /// <returns>A list of video info.</returns>
        public async Task<List<VideoInfo>> GetYoutubeChannel(string id)
        {
            var client = new YoutubeClient();
            // YoutubeExplode 6.x: GetUploadsAsync returns IAsyncEnumerable<PlaylistVideo>
            var videos = new List<VideoInfo>();
            await foreach (var video in client.Channels.GetUploadsAsync(id))
            {
                videos.Add(new VideoInfo
                {
                    ID = video.Id.Value,
                    Title = video.Title,
                    Duration = video.Duration ?? TimeSpan.Zero,
                    ThumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault()?.Url ?? "",
                    Url = "https://www.youtube.com/watch?v=" + video.Id.Value
                });
            }
            return videos;
        }

        /// <summary>
        /// Gets the playlists videos.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <returns>The video info.</returns>
        public async Task<List<VideoInfo>> GetYoutubePlayList(string id)
        {
            var client = new YoutubeClient();
            var videos = new List<VideoInfo>();
            // YoutubeExplode 6.x: Get playlist videos returns IAsyncEnumerable<PlaylistVideo>
            await foreach (var video in client.Playlists.GetVideosAsync(id))
            {
                videos.Add(new VideoInfo
                {
                    ID = video.Id.Value,
                    Title = video.Title,
                    Duration = video.Duration ?? TimeSpan.Zero,
                    ThumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault()?.Url ?? "",
                    Url = "https://www.youtube.com/watch?v=" + video.Id.Value
                });
            }
            return videos;
        }

        /// <summary>
        /// Stop the video.
        /// </summary>
        /// <returns>The music player.</returns>
        public IMusicPlayer StopVideo()
        {
            IServer server = _player as IServer;
            _videoUrl = null;
            if (server != null)
            {
                server.GetInfo().VideoUrl = null;
                server.SetVideo(string.Empty);
            }

            return _player;
        }

        /// <summary>
        /// Dispose of the controller.
        /// </summary>
        public override void Dispose()
        {
            _videoUrl = null;
            _refreshClientInfo?.Abort();
            base.Dispose();
        }

        /// <summary>
        /// This thread will Send the Vidoe and tim to the clients every 5 seconds.
        /// </summary>
        private void CheckTime()
        {
            _started = DateTime.Now;
            while (!string.IsNullOrEmpty(_videoUrl))
            {
                IServer server = _player as IServer;
                if (server != null)
                {
                    server.SetVideo(_videoUrl);
                    TimeSpan elapsed = DateTime.Now - _started;
                    server.SeekVideo(elapsed.TotalSeconds);
                }

                ThreadExtensions.SaveSleep(5000);
            }
        }
    }
}
