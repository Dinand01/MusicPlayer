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
    public interface IUI
    {
        void SetSongDuration(TimeSpan duration);

        void SetSongs(List<SongInformation> songs);

        void SetSong(SongInformation song);

        void SetSongPosition(TimeSpan currentTime);

        void SetNotification(string message);

        void SetCopyProgress(int value, int total);

        void SetVolume(int value);
    }
}
