using System;
using System.Linq;
using MusicPlayer.Models;
using System.IO;
using NAudio.Wave;
using System.Collections.Concurrent;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Reads and writes songs from the db
    /// </summary>
    /// 
    // TODO: no static 
    public static class SongInfoController
    {
        /// <summary>
        /// The songs that are currently in the database.
        /// </summary>
        private static ConcurrentDictionary<string, SongInformation> _resolvedSongs = new ConcurrentDictionary<string, SongInformation>();

        /// <summary>
        /// Resoloves the song info.
        /// </summary>
        /// <param name="song">The song to resolve.</param>
        /// <returns>The resolved song.</returns>
        public static SongInformation Resolve(SongInformation song)
        {
            if (song != null && !string.IsNullOrEmpty(song.Location))
            {
                if (_resolvedSongs.TryGetValue(song.Location.ToLower(), out SongInformation found))
                    return found;

                // TODO: remove if not needed
                try
                {
                    using var file = TagLib.File.Create(song.Location);

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

                    _resolvedSongs.TryAdd(song.Location.ToLower(), song);
                }
                catch
                {
                }
            }

            return song;
        }
    }
}
