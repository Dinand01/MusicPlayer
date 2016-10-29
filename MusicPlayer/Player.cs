using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Wave;
using NAudio.MediaFoundation;
using MusicPlayer.Controller;
using MusicPlayer.Models;
using TagLib;

namespace MusicPlayer
{
    /// <summary>
    /// Plays the music
    /// </summary>
    class Player
    {
        /// <summary>
        /// Conatins the list of songs (absolute paths)
        /// </summary>
        private List<Song> sourceList;
        private Random random;


        /// <summary>
        /// output
        /// </summary>
        private IWavePlayer waveOutDevice;

        private bool locker;
        private string activepath;
        public string ActivePath { get {return this.activepath;} }
        private Thread timetracker;
        private GUI gui;
        private MediaFoundationReader reader;
        private SongController songCtrl;

        /// <summary>
        /// Initialises the player
        /// </summary>
        /// <param name="gui">The GUI</param>
        public Player(GUI gui) {
            this.gui = gui;
            waveOutDevice = new WaveOutEvent();
            sourceList = new List<Song>();
            waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(NextSong);
            locker = false;
            this.songCtrl = new SongController();
            random = new Random();
        }

        /// <summary>
        /// Pauses or plays the music
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PausePlay(object sender, EventArgs e) {
            try
            {
                if (waveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    waveOutDevice.Pause();
                }
                else
                {
                    waveOutDevice.Play();
                    gui.redrawTrackbar(0, (int)reader.TotalTime.TotalSeconds);
                    if (timetracker != null)
                        timetracker.Abort();
                    timetracker = new Thread(UpdateTime);
                    timetracker.Start();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("ERROR on PLAY: " + ee.Message + "\n" + ee.InnerException);
            }

        }

        /// <summary>
        /// Gets a song from the list at the certain location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Song GetSongAtLocation(string location)
        {
            Song result = null;
            if (sourceList != null)
            {
                result = sourceList.FirstOrDefault(ct => ct.Location == location);
            }

            return result;
        }

        /// <summary>
        /// sets a song in the sourcellist if it exisst
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        public Song SetSong(Song song)
        {
            int index = -1;
            if (sourceList != null)
            {
                index = sourceList.FindIndex(ct => ct.Location == song.Location);
            }

            if (index > -1)
            {
                sourceList[index] = song;
            }
            else 
            {
                song = null;
            }

            return song;
        }

        /// <summary>
        /// Loads a song
        /// </summary>
        /// <param name="song"></param>
        public void Load(Song song)
        {
            this.Load(song.Location);
        }

        /// <summary>
        /// Load a song
        /// </summary>
        /// <param name="path">the absolute path</param>
        public void Load(String path){
            // dispose old mp3
            if (waveOutDevice.PlaybackState == PlaybackState.Playing || waveOutDevice.PlaybackState == PlaybackState.Paused) {
                locker = true;
                waveOutDevice.Stop();
            }

            string extension = Path.GetExtension(path);
            // start foundation reader
            try
            {
                reader = null;
                reader = new MediaFoundationReader(path);
                var outputFormat = new object();
                waveOutDevice.Init(reader);
                this.activepath = path;
                gui.SetActive();
            }
            catch (SystemException ee) 
            {
                NextSong(null, null);
            }
            catch (Exception e) {
                Console.WriteLine("ERRROR on Load: " + e.Message + "\n" + e.StackTrace + "\n" + e.InnerException);
                MessageBox.Show("Your system dus not support this audio codec: " + extension +
                        "\n Please install the media foundation codec");
            }
                
            PausePlay(null,null);
        }

        //load a list
        public List<Song> LoadAll(string[] files, int? number){
            if (waveOutDevice != null && waveOutDevice.PlaybackState != PlaybackState.Stopped)
            {
                locker = true;
                waveOutDevice.Stop();
            }

            List<Song> filestoload = new List<Song>();

            if (number != null)
            {
                Random r = new Random();
                while (filestoload.Count < number) {
                    filestoload.Add(new Song(TakeRandom(files)));
                }
            }

            if (filestoload.Count == 0)
            {
                sourceList = new List<Song>();
                foreach (string st in files)
                {
                    string ex = Path.GetExtension(st).ToLower();
                    if ((ex == ".mp3" || ex == ".flac" || ex == ".wma") && st.IndexOfAny(System.IO.Path.GetInvalidPathChars()) < 0)
                        sourceList.Add(new Song(st));
                }
            }
            else {
                sourceList = filestoload;
            }

            NextSong(null, null);
            EnrichSource(null);
            
            return sourceList;
        }

        public void Play(Song song)
        {
            this.Play(song.Location);
        }

        /// <summary>
        /// Play from key (path)
        /// </summary>
        /// <param name="key"></param>
        public void Play(string key) {

            if (sourceList.Any(ct => ct.Location == key))
            {
                Load(key);
                
            }
            else 
            {
                MessageBox.Show("Error: " + key + " not found!");
            }

        }

        /// <summary>
        /// Loads the deatisl of a song into the model
        /// Adds the details to the db
        /// </summary>
        /// <param name="song">the song to load info of</param>
        /// <returns>the loaded song or null</returns>
        public Song LoadDetails(Song song)
        {
            Song result = null;

            try
            {
                var file = TagLib.File.Create(song.Location);
                song.Gengre = string.Join(", ", file.Tag.Genres);
                song.Album = file.Tag.Album;

                string band = string.Join(", ", file.Tag.AlbumArtists);
                if (!string.IsNullOrEmpty(band))
                {
                    song.Band = band;
                }

                song.DateAdded = new FileInfo(song.Location).CreationTime;
                if (file.Tag.Year > 0)
                {
                    song.DateCreated = new DateTime((int)file.Tag.Year, 1, 1);
                }

                string title = file.Tag.Title;
                if (title != null)
                {
                    title = title.TrimStart();
                    if(title != string.Empty)
                        song.Title = title;
                }

                songCtrl.AddSongToDb(song);
                result = SetSong(song);

                file.Dispose();
            }
            catch (CorruptFileException cor)
            {
                // TODO
            }

            return result;
        }

        private void UpdateTime() {
            while (waveOutDevice.PlaybackState == PlaybackState.Paused || waveOutDevice.PlaybackState == PlaybackState.Playing) {
                gui.SetTrackbarPos((int)reader.CurrentTime.TotalSeconds);
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Next random song from the sourceList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NextSong(object sender, EventArgs e) {
            if (!locker)
            {
                Play(sourceList[random.Next(0, sourceList.Count)]);
            }
            else
                locker = false;
        }

        /// <summary>
        /// Copies all songs from the sourcelist to the destination folder
        /// </summary>
        /// <param name="destination"></param>
        public void CopyRandomSongs(string destination) {
            List<Song> copylist = this.sourceList.ToList();
            new Thread(() => new Progress(copylist, destination).ShowDialog()).Start();
        }

        /// <summary>
        /// Disposes this class properly
        /// </summary>
        public void Dispose()
        {
            try
            {
                locker = true;
                songCtrl.Dispose();
                //audioFileReader.Dispose();
                if (timetracker != null)
                {
                    timetracker.Abort();
                }

                if (waveOutDevice != null)
                {
                    waveOutDevice.Dispose();
                    waveOutDevice = null;
                }

                if (reader != null)
                {
                    
                    reader.Dispose();
                    reader = null;
                }
            }
            catch { }
        }

        /// <summary>
        /// Trackbar scroll event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveSongPosition(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.TrackBar temp = (System.Windows.Forms.TrackBar)sender;
                int pos = temp.Value;
                reader.CurrentTime = new TimeSpan(0, 0, pos);
            }
            catch { }
        }

        /// <summary>
        /// Enriches the data of the sourceList via the db
        /// </summary>
        /// <param name="rootFolder">optional parameter</param>
        private void EnrichSource(string rootFolder)
        {
            if (rootFolder == null) 
            {
                rootFolder = GetRootFolder(sourceList.Select(ct => ct.Location).ToArray());
            }

            var dbSongs = songCtrl.GetAllForFolder(rootFolder);
            for(int i = 0; i < sourceList.Count; i++)
            {
                var temp = dbSongs.FirstOrDefault(ct => ct.Location == sourceList[i].Location);
                if (temp != null)
                {
                    sourceList[i] = temp;
                }
            }
        }

        /// <summary>
        /// Takes a random valid song
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string TakeRandom(string[] list)
        {
            string rand = list[random.Next(0, list.Length - 1)];
            string ex = Path.GetExtension(rand).ToLower();
            if (!(ex == ".mp3" || ex == ".flac" || ex == ".wma"))
                rand = TakeRandom(list);

            return rand;
        }

        /// <summary>
        /// Gets the rootfolder from a collection of strings
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetRootFolder(string[] ss)
        {
            if (ss.Length == 0)
            {
                return "";
            }

            if (ss.Length == 1)
            {
                return ss[0];
            }

            int prefixLength = 0;

            foreach (char c in ss[0])
            {
                foreach (string s in ss)
                {
                    if (s.Length <= prefixLength || s[prefixLength] != c)
                    {
                        return ss[0].Substring(0, prefixLength);
                    }
                }
                prefixLength++;
            }

            return ss[0]; // all strings identical
        
        }
    }
}
