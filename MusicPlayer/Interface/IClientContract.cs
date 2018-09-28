using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// Descibes the WCF client contract.
    /// </summary>
    public interface IClientContract
    {
        /// <summary>
        /// Play a video.
        /// </summary>
        /// <param name="video">The video url.</param>
        [OperationContract(IsOneWay = true)]
        void PlayVideo(string video);

        /// <summary>
        /// Seek in a video.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        [OperationContract(IsOneWay = true)]
        void SeekVideo(double position);

        /// <summary>
        /// Set the song position.
        /// </summary>
        /// <param name="position">The position to set.</param>
        [OperationContract(IsOneWay = true)]
        void SetSongPosition(double position);

        /// <summary>
        /// Set the song.
        /// </summary>
        /// <param name="song">The song.</param>
        [OperationContract(IsOneWay = true)]
        void SetSong(SongInformation song);

        /// <summary>
        /// Sends the file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        [OperationContract(IsOneWay = true)]
        void SendFile(Stream stream);

        /// <summary>
        /// Play the song.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Play();

        /// <summary>
        /// Play from an online location.
        /// </summary>
        /// <param name="radioInfo">The radio station.</param>
        /// <param name="url">The url of the station.</param>
        [OperationContract(IsOneWay = true)]
        void PlayRadio(SongInformation radioInfo, string url);

        /// <summary>
        /// Pause the song.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Pause();

        /// <summary>
        /// The server will disconnect.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Disconnect();
    }
}
