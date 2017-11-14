using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The adapter class for the music player. 
    /// </summary>
    internal class MusicPlayerWrapper : IMusicPlayer
    {
        /// <summary>
        /// The real music player.
        /// </summary>
        protected IMusicPlayer _player;

        /// <summary>
        /// The song changed event.
        /// </summary>
        public event SongChanged SongChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicPlayerWrapper" /> class.
        /// </summary>
        /// <param name="player">The music player.</param>
        public MusicPlayerWrapper(IMusicPlayer player)
        {
            this._player = player;
            this._player.SongChanged += InvokeSongChanged;
        }

        /// <summary>
        /// Invoke the songChanged event.
        /// </summary>
        /// <param name="song">The song.</param>
        public void InvokeSongChanged(Song song)
        {
            SongChanged?.Invoke(song);
        }

        public virtual List<Song> LoadFolder(string folder)
        {
            return _player.LoadFolder(folder);
        }

        public virtual Song GetCurrentSong()
        {
            return _player.GetCurrentSong();
        }

        public virtual List<Song> GetSongs(int index = 0, string querry = null, int amount = 50)
        {
            return _player.GetSongs(index, querry, amount);
        }

        public virtual void Play(Song song)
        {
            _player.Play(song);
        }

        public virtual bool TogglePlay(bool? pause)
        {
            return _player.TogglePlay(pause);
        }

        public virtual void Next()
        {
            _player.Next();
        }

        public virtual void MoveToTime(long seconds)
        {
            _player.MoveToTime(seconds);
        }

        public virtual bool GetShuffle()
        {
            return _player.GetShuffle();
        }

        public virtual void SetShuffle(bool shuffle)
        {
            _player.SetShuffle(shuffle);
        }

        public virtual int GetVolume()
        {
            return _player.GetVolume();
        }

        public virtual void SetVolume(int percentage)
        {
            _player.SetVolume(percentage);
        }

        public virtual List<Song> LoadFiles(string[] files)
        {
            return _player.LoadFiles(files);
        }

        public virtual void Dispose()
        {
            _player?.Dispose();
        }

        public virtual int? GetSongPosition()
        {
            return _player.GetSongPosition();
        }
    }
}
