using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Models;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Class describing video info.
    /// </summary>
    public class VideoInfo
    {
        public VideoInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the video from a YoutubeExplode model.
        /// </summary>
        /// <param name="video">The YoutubeExplode model.</param>
        public VideoInfo(PlaylistVideo video)
        {
            this.ID = video.Id;
            this.Title = video.Title;
            this.ViewCount = video.Statistics.ViewCount;
            this.ThumbnailUrl = video.Thumbnails.HighResUrl;
            this.Duration = video.Duration;
            this.Url = "https://www.youtube.com/watch?v=" + video.Id;
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
        /// Gets or sest the number of times the video was viewed.
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
