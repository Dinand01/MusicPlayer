using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.UI
{
    /// <summary>
    /// The interface for a user interface.
    /// </summary>
    internal interface IUI
    {
        void SetSongDuration(TimeSpan duration);

        void SetSongs(List<Song> songs);

        void SetSong(Song song);

        void SetSongPosition(TimeSpan currentTime);

        void SetNotification(string message);

        void SetCopyProgress(int value, int total);

        void SetVolume(int value);
    }
}
