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
        private DbContext db;
        private List<Song> databaseSongs;
        private List<Song> dbSongQue;
        private static object _lock = new object();

        public SongController() 
        {
            dbSongQue = new List<Song>();
            this.db = new DbContext();
        }

        /// <summary>
        /// Gets all the songs in a rootfolder via the db
        /// </summary>
        /// <param name="folder">the rootfolder</param>
        /// <returns></returns>
        public List<Song> GetAllForFolder(string folder)
        {
           var items = db.Songs.Where(ct => ct.Location.StartsWith(folder));
           this.databaseSongs = items.ToList();
           return databaseSongs;
        }

        public Song AddSongToDb(Song song)
        {
            if (this.databaseSongs == null)
            {
                this.databaseSongs = new List<Song>();
            }

            try
            {
                if (!this.databaseSongs.Any(ct => ct.Location == song.Location) && !db.Songs.Any(ct => ct.Location == song.Location))
                {
                    try
                    {
                        dbSongQue.Add(song);
                    }
                    catch { }

                    this.databaseSongs.Add(song);
                    Task.Delay(10000).ContinueWith(t => AddsongsTodbThread());
                }
            }
            catch 
            { 
                // Memory was in use
                if (!this.databaseSongs.Any(ct => ct.Location == song.Location)){
                    dbSongQue.Add(song);
                    this.databaseSongs.Add(song);
                }
            }

            return song;
        }

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
                        db.Songs.AddRange(copy);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Debug.Write("Could not save songs to db", e.Message);
                    }
                }
            }
        }

        public void Dispose() 
        {
            this.db.Dispose();
        }
    }
}
