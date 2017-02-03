using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.DAL;
using MusicPlayer.Models;
using System.Diagnostics;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Reads and writes songs from the db
    /// </summary>
    public class SongController
    {
        /// <summary>
        /// The songs that are currently in the database.
        /// </summary>
        private List<Song> databaseSongs;

        /// <summary>
        /// The Queue of song that will be added to the database.
        /// </summary>
        private List<Song> dbSongQue;
        private static object _lock = new object();

        public SongController()
        {
            dbSongQue = new List<Song>();
        }

        /// <summary>
        /// Gets the details of the given song.
        /// </summary>
        /// <param name="entry">The entry to find.</param>
        /// <returns>The Song.</returns>
        public Song GetDetails(Song entry, int retrycount = 0)
        {
            var song = this.databaseSongs.FirstOrDefault(s => s.Location.ToLower() == entry.Location.ToLower());
            if (song == null)
            {
                try
                {
                    song = DbContextStore.Ctrl.Context.Songs.FirstOrDefault(s => s.Location.ToLower() == entry.Location.ToLower());
                }
                catch
                {
                    if(retrycount > 10)
                    {
                        throw new Exception("Database connection failed");
                    }

                    return GetDetails(entry, retrycount++);
                }

                if(song != null)
                {
                    song.SourceIsDb = true;
                    this.databaseSongs.Add(song);
                }
            }

            return song;
        }

        /// <summary>
        /// Gets all the songs in a rootfolder via the db
        /// </summary>
        /// <param name="folder">the rootfolder</param>
        /// <returns></returns>
        public List<Song> GetAllForFolder(string folder)
        {
            var items = DbContextStore.Ctrl.Context.Songs.Where(ct => ct.Location.StartsWith(folder));
            this.databaseSongs = items.ToList();
            this.databaseSongs.ForEach(s => s.SourceIsDb = true);
            return databaseSongs;
        }

        /// <summary>
        /// Adds o updates a db entry of a song.
        /// </summary>
        /// <param name="song">The new data.</param>
        /// <returns>The song.</returns>
        public Song AddSongToDb(Song song)
        {
            if (this.databaseSongs == null)
            {
                this.databaseSongs = new List<Song>();
            }

            try
            {
                if (!this.databaseSongs.Any(ct => ct.Location == song.Location) && !DbContextStore.Ctrl.Context.Songs.Any(ct => ct.Location == song.Location))
                {
                    dbSongQue.Add(song);
                    this.databaseSongs.Add(song);
                    Task.Delay(10000).ContinueWith(t => AddsongsTodbThread());
                }
            }
            catch 
            { 
                // Memory was in use
                if (!this.databaseSongs.Any(ct => ct.Location == song.Location))
                {
                    dbSongQue.Add(song);
                    this.databaseSongs.Add(song);
                }
            }

            return song;
        }

        /// <summary>
        /// Thread to save songs to the database.
        /// </summary>
        private void AddsongsTodbThread()
        {
            lock (_lock)
            {
                if (this.dbSongQue != null && dbSongQue.Count > 0)
                {

                    var copy = dbSongQue.ToList();
                    foreach (var song in copy)
                    {
                        if (song.Album != null && song.Album.Length > 512)
                        {
                            song.Album = song.Album.Take(512).ToString();
                        }
                    }

                    dbSongQue.Clear();

                    try
                    {
                        DbContextStore.Ctrl.Context.Songs.AddRange(copy);
                        DbContextStore.Ctrl.Context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        DbContextStore.Ctrl.Invalidate();
                        Debug.Write("Could not save songs to db", e.Message);
                    }
                }
            }
        }
    }
}
