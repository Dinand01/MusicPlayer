using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Class describing video info.
    /// </summary>
    [Serializable]
    public class VideoInfo
    {
        public VideoInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the video from a YoutubeExplode Video model.
        /// </summary>
        /// <param name="video">The YoutubeExplode Video model.</param>
        public VideoInfo(Video video)
        {
            this.ID = video.Id.Value;
            this.Title = video.Title;
            // Statistics.ViewCount doesn't exist in YoutubeExplode 6.x
            this.ViewCount = null;
            // Thumbnails is now IReadOnlyList<Thumbnail>
            var thumbnail = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault();
            this.ThumbnailUrl = thumbnail?.Url ?? "";
            this.Duration = video.Duration ?? TimeSpan.Zero;
            this.Url = "https://www.youtube.com/watch?v=" + video.Id.Value;
        }

        /// <summary>
        /// Creates a VideoInfo from a YoutubeExplode PlaylistVideo.
        /// </summary>
        /// <param name="playlistVideo">The playlist video.</param>
        /// <returns>A new VideoInfo instance.</returns>
        public static VideoInfo FromPlaylistVideo(YoutubeExplode.Playlists.PlaylistVideo playlistVideo)
        {
            var info = new VideoInfo();
            info.ID = playlistVideo.Id.Value;
            info.Title = playlistVideo.Title;
            info.ViewCount = null; // Not available in PlaylistVideo
            var thumbnail = playlistVideo.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault();
            info.ThumbnailUrl = thumbnail?.Url ?? "";
            info.Duration = playlistVideo.Duration ?? TimeSpan.Zero;
            info.Url = "https://www.youtube.com/watch?v=" + playlistVideo.Id.Value;
            return info;
        }

        /// <summary>
        /// Gets or sets the id of the video.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the title of the video.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the number of times the video was viewed.
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// Gets or sets the duration of the video.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the url of the thumbnail.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the url of the video.
        /// </summary>
        public string Url { get; set; }
    }
}
