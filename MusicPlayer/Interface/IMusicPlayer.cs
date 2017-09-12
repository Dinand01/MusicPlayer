using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The song changed description.
    /// </summary>
    /// <param name="song">The new song.</param>
    public delegate void SongChanged(Song song);

    /// <summary>
    /// The interface of the music player.
    /// </summary>
    public interface IMusicPlayer : IDisposable
    {
        /// <summary>
        /// An event that is triggered when the song changes.
        /// </summary>
        event SongChanged SongChanged;

        /// <summary>
        /// Loads all the files in the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        List<Song> LoadFolder(string folder);

        /// <summary>
        /// Loads a list of file locations.
        /// </summary>
        /// <param name="files">The file locations.</param>
        /// <param name="number">The number of files to load.</param>
        /// <returns>A List of songs.</returns>
        List<Song> LoadFiles(string[] files);

        /// <summary>
        /// Gets songs from the currently loaded songs according to the provided paramters.
        /// </summary>
        /// <param name="index">The index to start at.</param>
        /// <param name="querry">The querry string.</param>
        /// <param name="amount">The maximum amount of songs to return.</param>
        /// <returns>The songs.</returns>
        List<Song> GetSongs(int index = 0, string querry = null, int amount = 50);

        /// <summary>
        /// Gets the position of the current playing song.
        /// </summary>
        /// <returns>The position in seconds.</returns>
        int? GetSongPosition();

        /// <summary>
        /// Plays a song.
        /// </summary>
        /// <param name="song">The song.</param>
        void Play(Song song);

        /// <summary>
        /// Pause the current song.
        /// </summary>
        void TogglePlay();

        /// <summary>
        /// Play the next song.
        /// </summary>
        void Next();

        /// <summary>
        /// Sets the Song position to a certain time.
        /// </summary>
        /// <param name="seconds">The second to move to.</param>
        void MoveToTime(long seconds);

        /// <summary>
        /// Gets the current shuffle setting.
        /// </summary>
        /// <returns>The shuffle setting.</returns>
        bool GetShuffle();

        /// <summary>
        /// Change the shuffle settings.
        /// </summary>
        /// <param name="shuffle">To shuffle or not to shuffle.</param>
        void SetShuffle(bool shuffle);

        /// <summary>
        /// Gets the current volume.
        /// </summary>
        /// <returns>The volume from 0 - 100%.</returns>
        int GetVolume();

        /// <summary>
        /// Sets the volume.
        /// </summary>
        /// <param name="percentage">The new volume setting.</param>
        void SetVolume(int percentage);
    }
}
