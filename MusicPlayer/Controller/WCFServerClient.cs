using MusicPlayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using System.IO;
using System.ServiceModel;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The implementation for the client contract.
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class WCFServerClient : IClientContract
    {
        /// <summary>
        /// The music client.
        /// </summary>
        private IClient _musicClient;

        /// <summary>
        /// The current song.
        /// </summary>
        private SongInformation _currentSong;

        /// <summary>
        /// Initializes a new instance of the <see cref="WCFServerClient" /> class.
        /// </summary>
        /// <param name="client"></param>
        public WCFServerClient(IClient client)
        {
            _musicClient = client;
        }

        /// <summary>
        /// The server will disconnect.
        /// </summary>
        public void Disconnect()
        {
            _musicClient.Dispose();
        }

        /// <summary>
        /// Pause the music or video.
        /// </summary>
        public void Pause()
        {
            _musicClient.TogglePlay(true);
        }
        
        /// <summary>
        /// Play the music or video.
        /// </summary>
        public void Play()
        {
            _musicClient.TogglePlay(false);
        }

        /// <summary>
        /// Play from an online location.
        /// </summary>
        /// <param name="url">The url.</param>
        public void PlayRadio(string url)
        {
            _musicClient.Play(url);
        }

        /// <summary>
        /// Play a video.
        /// </summary>
        /// <param name="video">The video url.</param>
        public void PlayVideo(string video)
        {
            _musicClient.SetVideo(video);
        }

        /// <summary>
        /// Seek in the video.
        /// </summary>
        /// <param name="position">The video position in seconds.</param>
        public void SeekVideo(double position)
        {
            _musicClient.SeekVideo(position);
        }

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SendFile(Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                _currentSong.File = mem.ToArray();
            }
            
            _musicClient.Play(_currentSong);
        }

        /// <summary>
        /// Sets the song information.
        /// </summary>
        /// <param name="song">The song.</param>
        public void SetSong(SongInformation song)
        {
            _currentSong = song;
        }

        /// <summary>
        /// Sets the song position.
        /// </summary>
        /// <param name="position">The position in seconds.</param>
        public void SetSongPosition(double position)
        {
            _musicClient.MoveToTime(Convert.ToInt64(position));
        }
    }
}
