using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// Describes the client contract (callback from server to client).
    /// Supports both sync and async patterns for gRPC communication.
    /// </summary>
    public interface IClientContract
    {
        /// <summary>
        /// Play a video.
        /// </summary>
        /// <param name="video">The video url.</param>
        void PlayVideo(string video);

        /// <summary>
        /// Play a video (async version for gRPC).
        /// </summary>
        Task PlayVideoAsync(string video);

        /// <summary>
        /// Seek in a video.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        void SeekVideo(double position);

        /// <summary>
        /// Seek in a video (async version for gRPC).
        /// </summary>
        Task SeekVideoAsync(double position);

        /// <summary>
        /// Set the song position.
        /// </summary>
        /// <param name="position">The position to set.</param>
        void SetSongPosition(double position);

        /// <summary>
        /// Set the song position (async version for gRPC).
        /// </summary>
        Task SetSongPositionAsync(double position);

        /// <summary>
        /// Set the song.
        /// </summary>
        /// <param name="song">The song.</param>
        void SetSong(SongInformation song);

        /// <summary>
        /// Set the song (async version for gRPC).
        /// </summary>
        Task SetSongAsync(SongInformation song);

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        void SendFile(Stream stream);

        /// <summary>
        /// Sends the file (async version for gRPC).
        /// </summary>
        Task SendFileAsync(Stream stream);

        /// <summary>
        /// Play the song.
        /// </summary>
        void Play();

        /// <summary>
        /// Play the song (async version for gRPC).
        /// </summary>
        Task PlayAsync();

        /// <summary>
        /// Play from an online location.
        /// </summary>
        /// <param name="radioInfo">The radio station.</param>
        /// <param name="url">The url of the station.</param>
        void PlayRadio(SongInformation radioInfo, string url);

        /// <summary>
        /// Play from an online location (async version for gRPC).
        /// </summary>
        Task PlayRadioAsync(SongInformation radioInfo, string url);

        /// <summary>
        /// Pause the song.
        /// </summary>
        void Pause();

        /// <summary>
        /// Pause the song (async version for gRPC).
        /// </summary>
        Task PauseAsync();

        /// <summary>
        /// The server will disconnect.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// The server will disconnect (async version for gRPC).
        /// </summary>
        Task DisconnectAsync();
    }
}
