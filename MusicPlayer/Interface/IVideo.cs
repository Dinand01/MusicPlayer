using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The video interface.
    /// </summary>
    public interface IVideo : IMusicPlayer, IDisposable
    {
        /// <summary>
        /// Gets the youtube channels video's.
        /// </summary>
        /// <param name="id">The id of the youtube channel.</param>
        /// <returns>A list of video info.</returns>
        Task<List<VideoInfo>> GetYoutubeChannel(string id = "UCj1xVEV4zKWr2dslkAgQAFA");

        /// <summary>
        /// Gets the playlists videos.
        /// </summary>
        /// <param name="id">The id of the playlist.</param>
        /// <returns>The video info.</returns>
        Task<List<VideoInfo>> GetYoutubePlayList(string id);

        /// <summary>
        /// Start a video.
        /// </summary>
        /// <param name="url">The url of the video.</param>
        void StartVideo(string url);

        /// <summary>
        /// Seek to a specific position.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        void Seek(double position);

        /// <summary>
        /// Stop the video and return the music player.
        /// </summary>
        /// <returns>The musicplayer.</returns>
        IMusicPlayer StopVideo();
    }
}
