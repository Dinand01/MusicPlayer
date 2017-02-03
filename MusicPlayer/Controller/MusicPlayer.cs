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
using MusicPlayer.Extensions;
using MusicPlayer.Models;
using TagLib;
using System.Net;
using MusicPlayer.UI;
using NAudio.Wave.SampleProviders;
using NAudio.CoreAudioApi;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Plays the music
    /// </summary>
    internal partial class Player
    {
        #region Variables

        /// <summary>
        /// Contains the list of songs (absolute paths).
        /// </summary>
        private List<Song> _sourceList;

        /// <summary>
        /// Used for providing random songs.
        /// </summary>
        private Random _random;

        /// <summary>
        /// An integer indicating the current song index, when -1 the songs are randomly selected.
        /// </summary>
        private int currentIdx = -1;

        /// <summary>
        ///  audio output.
        /// </summary>
        private WaveOut waveOutDevice;

        /// <summary>
        /// The server instance when hosting the music.
        /// </summary>
        private NetworkServer _networkServer;

        /// <summary>
        /// Hosting a music stream over network
        /// </summary>
        private bool hosting = false;

        /// <summary>
        /// Indicates whether the class is owned by another class and is only receiving commands.
        /// </summary>
        private bool _isReceiveMode;

        /// <summary>
        /// A boolean indicating whether files are being copied.
        /// </summary>
        private bool _isCopying;

        /// <summary>
        /// The currently set volume.
        /// </summary>
        private static int _volume = 100;

        /// <summary>
        /// Indicates that the class is disposing, all threads should have this var in the while loop.
        /// </summary>
        private bool _disposing = false;

        /// <summary>
        /// TODO: clean this up.
        /// </summary>
        private bool locker;

        /// <summary>
        /// The current song.
        /// </summary>
        private Song _currentSong;

        private Thread _timetracker;

        /// <summary>
        /// The ui interface, used to send events to the user interface.
        /// </summary>
        private IUI _gui;

        private SongController _songCtrl;

        /// <summary>
        /// The Naudio foundation reader, used to decode audio files.
        /// </summary>
        private MediaFoundationReader _playstream;

        #endregion

        /// <summary>
        /// Initialises the player
        /// </summary>
        /// <param name="gui">The GUI</param>
        public Player(IUI gui, bool isReceiveMode = false) 
        {
            this._gui = gui;
            waveOutDevice = new WaveOut();
            _volume = SettingController.Get(SettingType.Volume).AsInt(50);
            _sourceList = new List<Song>();
            waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(OnWaveOutStop);
            locker = false;
            this._songCtrl = new SongController();
            _random = new Random();
            this._isReceiveMode = isReceiveMode;
        }

        #region PlayerInformation

        /// <summary>
        /// A boolean indicating whether the music is being hosted.
        /// </summary>
        public bool Hosting
        {
            get
            {
                return hosting;
            }
        }

        /// <summary>
        /// Returns a boolean whether the player is playing.
        /// </summary>
        /// <returns>The boolean.</returns>
        public bool IsPlaying()
        {
            if (waveOutDevice != null && (waveOutDevice.PlaybackState == PlaybackState.Playing) && _playstream != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a boolean indcating whether files are being copied.
        /// </summary>
        /// <returns></returns>
        public bool IsCopying()
        {
            return _isCopying;
        }

        /// <summary>
        /// Gets the current active song.
        /// </summary>
        public Song ActiveSong
        {
            get
            {
                return this._currentSong;
            }
        }

        /// <summary>
        /// Gets the current song collection.
        /// </summary>
        public List<Song> SongList
        {
            get
            {
                return _sourceList;
            }
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <returns>The current time or null.</returns>
        public TimeSpan? GetCurrentTime()
        {
            if (IsPlaying() && _playstream != null)
            {
                return _playstream.CurrentTime;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether schuffle is on.
        /// </summary>
        public bool Shuffle
        {
            get
            {
                return currentIdx == -1;
            }

            set
            {
                if (value)
                {
                    currentIdx = -1;
                }
                else
                {
                    currentIdx = 0;
                }
            }
        }

        #endregion

        #region PlayerControls

        /// <summary>
        /// Pauses or plays the music.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void TogglePlay(object sender = null, EventArgs e = null) 
        {
            try
            {
                if (waveOutDevice.PlaybackState == PlaybackState.Playing)
                {
                    waveOutDevice.Pause();
                }
                else
                {
                    waveOutDevice.Play();
                    _gui.SetSongDuration(_playstream.TotalTime);
                    if (_timetracker != null)
                    {
                        _timetracker.Abort();
                    }

                    _timetracker = new Thread(UpdateTime);
                    _timetracker.Start();
                }
            }
            catch (Exception ee)
            {
                if (!_isReceiveMode)
                {
                    MessageBox.Show("ERROR on PLAY: " + ee.Message + "\n" + ee.InnerException);
                }
            }
        }

        /// <summary>
        /// Loads a song
        /// </summary>
        /// <param name="song">The song object.</param>
        public void Load(Song song)
        {
            _currentSong = song;
            this.Load(song.Location);
        }

        /// <summary>
        /// Load a song
        /// </summary>
        /// <param name="path">the absolute path</param>
        private void Load(string path)
        {
            DisposeWaveOut(true);

            // Start foundation reader
            try
            {
                if (_playstream != null)
                {
                    _playstream.Dispose();
                    _playstream = null;
                }

                _playstream = new MediaFoundationReader(path, new MediaFoundationReader.MediaFoundationReaderSettings
                {
                    RepositionInRead = true,
                    SingleReaderObject = true
                });

                if (_playstream != null)
                {
                    waveOutDevice.Init(_playstream);
                    if (_networkServer != null)
                    {
                        _networkServer.HostSong(_currentSong);
                    }

                    if (_isReceiveMode)
                    {
                        _gui.SetSongs(_sourceList);
                    }

                    _gui.SetSong(_currentSong);
                }
                else
                {
                    if (!_isReceiveMode)
                    {
                        MessageBox.Show("File was not found or could not be opened at:" + path);
                    }
                }
            }
            catch (SystemException ee) 
            {
                if (!_isReceiveMode)
                {
                    NextSong();
                }
            }
            catch (Exception e) 
            {
                string extension = Path.GetExtension(path);
                Console.WriteLine("ERRROR on Load: " + e.Message + "\n" + e.StackTrace + "\n" + e.InnerException);
                if (!_isReceiveMode)
                {
                    MessageBox.Show("Your system dus not support this audio codec: " + extension +
                            "\n Please install the media foundation codec");
                }
            }

            TogglePlay();
        }

        /// <summary>
        /// Loads a list of file locations.
        /// </summary>
        /// <param name="files">The file locations.</param>
        /// <param name="number">The number of files to load.</param>
        /// <returns>A List of songs.</returns>
        public List<Song> LoadAll(string[] files)
        {
            if (waveOutDevice != null && waveOutDevice.PlaybackState != PlaybackState.Stopped)
            {
                locker = true;
                waveOutDevice.Stop();
            }

            _sourceList = new List<Song>();
            foreach (string st in files)
            {
                string ex = Path.GetExtension(st).ToLower();
                if ((ex == ".mp3" || ex == ".flac" || ex == ".wma") && st.IndexOfAny(System.IO.Path.GetInvalidPathChars()) < 0)
                {
                    Song newSong = new Song(st);
                    _sourceList.Add(newSong);
                }
            }

            EnrichSource(null);
            return _sourceList;
        }

        /// <summary>
        /// Sets the available songs for the player.
        /// </summary>
        /// <param name="songs">The songs to set.</param>
        public void SetSongs(List<Song> songs)
        {
            _sourceList = songs;
        }
         
        /// <summary>
        /// Play a song object.
        /// </summary>
        /// <param name="song">The song.</param>
        public void Play(Song song)
        {
            if (!_sourceList.Any(ct => ct.Location == song.Location))
            {
                _sourceList.Add(song);
            }

            this.Play(song.Location);
        }

        /// <summary>
        /// Play from key (path).
        /// </summary>
        /// <param name="key"></param>
        public void Play(string key = null)
        {
            Song song;
            if(key == null)
            {
                if(currentIdx == -1)
                {
                    NextSong();
                    return;
                }
                else
                {
                    song = _sourceList[currentIdx];
                }
            }
            else
            {
                song = _sourceList.FirstOrDefault(ct => ct.Location == key);
            }

            if (song != null)
            {
                Load(song);
            }
            else 
            {
                MessageBox.Show("Error: " + key + " not found!");
            }
        }

        /// <summary>
        /// Sets the volume of the waveout device.
        /// </summary>
        /// <param name="percentage">The volume in percentage.</param>
        public void SetVolume(int percentage)
        {
            if (percentage <= 100 && percentage > -1)
            {
                _volume = percentage;
                waveOutDevice.Volume = percentage / (float)100;
                SettingController.Set(SettingType.Volume, Convert.ToString(percentage));
            }
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public static int GetVolume()
        {
            return _volume;
        }

        /// <summary>
        /// Not in use, may be usefull in the future.
        /// </summary>
        /// <returns></returns>
        private static int GetVolumeOfDefaultAudioDevice()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            int leftVolume = (int)(defaultDevice.AudioMeterInformation.PeakValues[0] * 100);
            return leftVolume;
        }

        /// <summary>
        /// Starts a next random song from the source list.
        /// </summary>
        public void NextSong()
        {
            if (!locker)
            {
                if (_sourceList != null && _sourceList.Count > 0)
                {
                    int idx = -1;
                    if (currentIdx == -1)
                    {
                        idx = _random.Next(0, _sourceList.Count);
                    }
                    else
                    {
                        if (currentIdx + 1 < _sourceList.Count)
                        {
                            currentIdx++;
                        }
                        else
                        {
                            currentIdx = 0;
                        }

                        idx = currentIdx;
                    }

                    var song = _sourceList[idx];
                    if (!hosting || Path.GetExtension(song.Location) == ".mp3")
                    {
                        Play(song);
                    }
                    else
                    {
                        NextSong();
                    }
                }
            }
            else
            {
                locker = false;
            }
        }

        /// <summary>
        /// Copies all songs from the sourcelist to the destination folder
        /// </summary>
        /// <param name="destination"></param>
        public void CopyRandomSongs(string destination, int amount)
        {
            List<Song> copylist = new List<Song>();
            if(this._sourceList.Count <= amount)
            {
                copylist = this._sourceList;
            }
            else
            {
                while(copylist.Count < amount)
                {
                    copylist.Add(TakeRandom(copylist));
                }
            }

            _isCopying = true;
            Task.Run(() => Copy(copylist, destination));
        }

        /// <summary>
        /// Copies the source files to the destination.
        /// </summary>
        /// <param name="source">The files to copy.</param>
        /// <param name="destination">The destination.</param>
        private void Copy(List<Song> source, string destination)
        {
            for (int i = 0; i < source.Count; i++)
            {
                _isCopying = true;
                string path = source[i].Location;
                _gui.SetCopyProgress((int)(((i + 1) / (double)source.Count) * 100), 100);

                try
                {
                    System.IO.File.Copy(path, destination + "\\" + Path.GetFileName(path), true);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            _isCopying = false;
            _gui.SetCopyProgress(100, 100);
        }

        /// <summary>
        /// Sets the Song position to a certain time.
        /// </summary>
        /// <param name="time">The time.</param>
        public void MoveToTime(TimeSpan time)
        {
            _playstream.CurrentTime = time;
            if (hosting && _networkServer != null)
            {
                _networkServer.GotoPosition(time);
            }
        }

        #endregion

        /// <summary>
        /// Updates the time in the GUI.
        /// </summary>
        private void UpdateTime()
        {
            while (!_disposing && waveOutDevice != null
                && (waveOutDevice.PlaybackState == PlaybackState.Paused || waveOutDevice.PlaybackState == PlaybackState.Playing))
            {
                try
                {
                    _gui.SetSongPosition(_playstream.CurrentTime);
                    ThreadExtensions.SaveSleep(500);
                }
                catch (Exception e)
                {
                }
            }
        }

        /// <summary>
        /// Next random song from the sourceList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWaveOutStop(object sender, EventArgs e)
        {
            if (!_isReceiveMode)
            {
                NextSong();
            }
        }

        /// <summary>
        /// Trackbar scroll event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveSongPosition(object sender, EventArgs e)
        {
            try
            {
                TrackBar temp = (TrackBar)sender;
                int pos = temp.Value;
                MoveToTime(new TimeSpan(0, 0, pos));
            }
            catch { }
        }

        /// <summary>
        /// Starts the audio server. 
        /// </summary>
        /// <param name="ip">Thge ip adress string.</param>
        /// <param name="port">The port string.</param>
        public void StartAudioServer(IPAddress ip, int port)
        {
            hosting = true;
            _networkServer = new NetworkServer(port, this);
            if (_currentSong != null)
            {
                _networkServer.HostSong(_currentSong);
            }
        }

        /// <summary>
        /// Notifies the clients when the server is active.
        /// </summary>
        /// <param name="text">The text.</param>
        public void NotifyClients(string text)
        {
            if(_networkServer != null)
            {
                _networkServer.Notify(text);
            }
        }

        /// <summary>
        /// Disconnects from the audio server
        /// </summary>
        public void DisconnectFromAudioServer()
        {
            if (_networkServer != null)
            {
                _networkServer.Dispose();
                _networkServer = null;
            }

            hosting = false;
        }

        /// <summary>
        /// Takes a random valid song.
        /// </summary>
        /// <param name="list">The already chosen song, they will not be chosen again.</param>
        /// <returns>A random song.</returns>
        private Song TakeRandom(List<Song> alreadyChosenSongs)
        {
            List<Song> pool = this._sourceList.Except(alreadyChosenSongs).ToList();
            Song rand = pool[_random.Next(0, pool.Count - 1)];
            return rand;
        }

        /// <summary>
        /// Copies a mediafoundation reader stream to a buffered waveprovider.
        /// Sets the pos of the mediafoundstream to 0 when done.
        /// </summary>
        /// <param name="input">The source stream.</param>
        /// <returns>The buffered wave provider.</returns>
        private BufferedWaveProvider CopyStream(MediaFoundationReader input)
        {
            BufferedWaveProvider test = new BufferedWaveProvider(WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, 44100, 2, 176400, 4, 16));
            test.BufferDuration = input.TotalTime + new TimeSpan(0, 0, 1);
            test.DiscardOnBufferOverflow = true;
            using (WaveStream waveStream = WaveFormatConversionStream.CreatePcmStream(input))
            {
                long previouspos = 0;
                while ((waveStream.Length - waveStream.Position) > 0)
                {
                    int size = 8092;
                    if (size > waveStream.Length - waveStream.Position)
                    {
                        size = (int)(waveStream.Length - waveStream.Position);
                    }

                    byte[] bytes = new byte[size];
                    waveStream.Read(bytes, 0, size);
                    test.AddSamples(bytes, 0, size);
                    previouspos += size;

                    if (size != 8092)
                    {
                        break;
                    }
                }
            }

            input.Position = 0;
            return test;
        }

        /// <summary>
        /// Disposes of the wave out device.
        /// </summary>
        /// <param name="reinit">A boolean indicating whether to reinitialize the wave out device.</param>
        private void DisposeWaveOut(bool reinit = false)
        {
            if (waveOutDevice.PlaybackState == PlaybackState.Playing || waveOutDevice.PlaybackState == PlaybackState.Paused)
            {
                locker = true;
                try
                {
                    waveOutDevice.Stop();
                }
                catch
                {
                }

                waveOutDevice.Dispose();

                if (reinit)
                {
                    locker = false;
                    waveOutDevice = new WaveOut();
                    waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(OnWaveOutStop);
                    SetVolume(_volume);
                }
            }
        }

        /// <summary>
        /// Disposes this class properly
        /// </summary>
        public void Dispose()
        {
            try
            {
                _disposing = true;
                locker = true;

                if (_networkServer != null)
                {
                    _networkServer.Dispose();
                }

                DisposeWaveOut();

                if (_playstream != null)
                {
                    _playstream.Dispose();
                }
            }
            catch 
            {
            }
        }
    }
}
