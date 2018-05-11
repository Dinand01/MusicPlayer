using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using System.Diagnostics;
using System.IO;
using NAudio.Wave;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Reads and writes songs from the db
    /// </summary>
    public static class SongInfoController
    {
        /// <summary>
        /// The songs that are currently in the database.
        /// </summary>
        private static SynchronizedCollection<Song> _resolvedSongs = new SynchronizedCollection<Song>();

        /// <summary>
        /// Resoloves the song info.
        /// </summary>
        /// <param name="song">The song to resolve.</param>
        /// <returns>The resolved song.</returns>
        public static Song Resolve(Song song)
        {
            if (song != null && !string.IsNullOrEmpty(song.Location))
            {
                Song found = null;
                lock(_resolvedSongs)
                {
                    found = _resolvedSongs.ToList().FirstOrDefault(s => s.Location.ToLower() == song.Location.ToLower());
                }

                if (found != null)
                {
                    return found;
                }

                try
                {
                    var file = TagLib.File.Create(song.Location);

                    try
                    {
                        using (var reader = new MediaFoundationReader(song.Location))
                        {
                            double? time = reader?.TotalTime.TotalSeconds;
                            song.Duration = time == null ? 0 : (long)time;
                        }
                    }
                    catch
                    {
                    }
                    
                    song.Genre = string.Join(", ", file.Tag.Genres);
                    song.Album = file.Tag.Album;
                    song.Band = string.Join(", ", file.Tag.Composers.Union(file.Tag.Artists));
                    song.DateAdded = new FileInfo(song.Location).CreationTime;

                    if (file.Tag.Year > 0)
                    {
                        song.DateCreated = new DateTime((int)file.Tag.Year, 1, 1);
                    }

                    song.Title = string.IsNullOrEmpty(file.Tag.Title) ? song.Title : file.Tag.Title;
                    song.Image = file.Tag.Pictures?.FirstOrDefault()?.Data?.Data;
                    song.IsResolved = true;
                    lock (_resolvedSongs)
                    {
                        _resolvedSongs.Add(song);
                    }

                    file.Dispose();
                }
                catch
                {
                }
            }

            return song;
        }
    }
}
