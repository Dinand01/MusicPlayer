using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using MusicPlayer.Extensions;
using MusicPlayer.Models;
using NAudio.CoreAudioApi;
using MusicPlayer.Interface;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// Plays the music
    /// </summary>
    internal partial class Player : IMusicPlayer
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
        /// An integer indicating the current song index.
        /// </summary>
        private int _currentIdx = 0;

        /// <summary>
        ///  audio output.
        /// </summary>
        private WaveOut _waveOutDevice;

        /// <summary>
        /// Indicates whether the class is owned by another class and is only receiving commands.
        /// </summary>
        private bool _isReceiveMode;

        /// <summary>
        /// The boolean indicating whether shuffle is on.
        /// </summary>
        private bool _shuffle = false;

        /// <summary>
        /// The currently set volume.
        /// </summary>
        private int _volume = 50;

        /// <summary>
        /// Indicates that the class is disposing, all threads should have this var in the while loop.
        /// </summary>
        private bool _disposing = false;

        /// <summary>
        /// The current song.
        /// </summary>
        private Song _currentSong;

        /// <summary>
        /// The thread for updating the time via the event.
        /// </summary>
        private Thread _timetracker;

        /// <summary>
        /// The Naudio foundation reader, used to decode audio files.
        /// </summary>
        private MediaFoundationReader _playstream;

        #endregion

        /// <summary>
        /// The songchanged event.
        /// </summary>
        public event SongChanged SongChanged;

        /// <summary>
        /// Initialises the player
        /// </summary>
        public Player(bool isReceiveMode = false) 
        {
            _volume = DataController.GetSetting<int>(SettingType.Volume, 50);
            _shuffle = DataController.GetSetting<bool>(SettingType.Shuffle, false);
            _sourceList = new List<Song>();
            _random = new Random();
            this._isReceiveMode = isReceiveMode;
        }

        #region PlayerInformation

        // <summary>
        /// Gets the current song.
        /// </summary>
        /// <returns>The current song.</returns>
        public Song GetCurrentSong()
        {
            return _currentSong;
        }

        /// Gets the position of the current playing song.
        /// </summary>
        /// <returns>The position in seconds.</returns>
        public int? GetSongPosition()
        {
            if (_waveOutDevice != null && _waveOutDevice.PlaybackState != PlaybackState.Stopped && _playstream != null)
            {
                return Convert.ToInt32(_playstream.CurrentTime.TotalSeconds);
            }

            return null;
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        /// <returns>The current time or null.</returns>
        public TimeSpan? GetCurrentTime()
        {
            if (_playstream != null)
            {
                return _playstream.CurrentTime;
            }

            return null;
        }

        #endregion

        #region PlayerControls

        /// <summary>
        /// Pauses or plays the music.
        /// </summary>
        /// <returns>A boolean indicating whether the music is playing.</returns>
        public bool TogglePlay(bool? pause = null)
        {
            bool result = false;
            if (_waveOutDevice != null && _currentSong != null)
            {
                if ((_waveOutDevice.PlaybackState == PlaybackState.Playing && pause != false) || pause == true)
                {
                    _currentSong.IsPlaying = false;
                    _waveOutDevice.Pause();
                }
                else if (_playstream != null)
                {
                    _waveOutDevice.Play();
                    _currentSong.IsPlaying = true;
                    _timetracker?.Abort();
                    _timetracker = new Thread(UpdateTime);
                    _timetracker.Start();
                    result = true;
                }

                SongChanged?.Invoke(_currentSong);
            }

            return result;
        }

        /// <summary>
        /// Loads a song
        /// </summary>
        /// <param name="song">The song object.</param>
        private void Load(Song song)
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
            try
            {
                DisposeWaveOut(true);
                _playstream?.Dispose();
                _playstream = new MediaFoundationReader(path);
                if (_playstream != null)
                {
                    _waveOutDevice.Init(_playstream);
                    _currentSong.Duration = Convert.ToInt64(_playstream.TotalTime.TotalSeconds);
                    SongChanged?.Invoke(_currentSong);
                    TogglePlay();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "MusicPlayer: Could not load song " + path);
                _playstream = null;
                if (!_isReceiveMode)
                {
                    Next();
                }
            }
        }

        /// <summary>
        /// Loads all the files in the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>The loaded songs.</returns>
        public List<Song> LoadFolder(string folder)
        {
            var files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
            return this.LoadFiles(files.ToArray());
        }

        /// <summary>
        /// Gets songs from the currently loaded songs according to the provided paramters.
        /// </summary>
        /// <param name="index">The index to start at.</param>
        /// <param name="querry">The querry string.</param>
        /// <param name="amount">The maximum amount of songs to return.</param>
        /// <returns>The songs.</returns>
        public List<Song> GetSongs(int index = 0, string querry = null, int amount = 50)
        {
            querry = querry.ToLower();
            var matches = _sourceList?.Where(s => (s.Title != null && s.Title.ToLower().Contains(querry)) || (s.Band != null && s.Band.ToLower().Contains(querry)) || (s.Location != null  && s.Location.ToLower().Contains(querry)))
                            ?.OrderBy(s => s.Title)
                            ?.Skip(index)
                            ?.Take(amount)
                            ?.ToList();

            if (matches != null)
            {
                List<Task> resolvetasks = new List<Task>();
                for(int i = 0; i < matches.Count; i++)
                {
                    int j = i;
                    var task = Task.Run(() => {
                        matches[j] = SongInfoController.Resolve(matches[j]);
                        int sourceIndex = _sourceList.FindIndex(s => s.Location == matches[j].Location);
                        _sourceList[sourceIndex] = matches[j];
                    });
                    resolvetasks.Add(task);
                }

                Task.WaitAll(resolvetasks.ToArray());
            }

            return matches;
        }

        /// <summary>
        /// Loads a list of file locations.
        /// </summary>
        /// <param name="files">The file locations.</param>
        /// <param name="number">The number of files to load.</param>
        /// <returns>A List of songs.</returns>
        public List<Song> LoadFiles(string[] files)
        {
            if (_waveOutDevice != null && _waveOutDevice.PlaybackState != PlaybackState.Stopped)
            {
                _waveOutDevice.PlaybackStopped -= OnWaveOutStop;
                _waveOutDevice.Stop();
            }

            _sourceList = new List<Song>();
            foreach (string st in files)
            {
                string ex = Path.GetExtension(st).ToLower();
                if ((ex == ".mp3" || ex == ".flac" || ex == ".wma") && st.IndexOfAny(Path.GetInvalidPathChars()) < 0)
                {
                    Song newSong = new Song(st);
                    _sourceList.Add(newSong);
                }
            }

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
        /// Gets the shuffle setting.
        /// </summary>
        /// <returns>The shuffle setting.</returns>
        public bool GetShuffle()
        {
            return this._shuffle;
        }

        /// <summary>
        /// Change the shuffle settings.
        /// </summary>
        /// <param name="shuffle">To shuffle or not to shuffle.</param>
        public void SetShuffle(bool shuffle)
        {
            this._shuffle = shuffle;
            DataController.SetSetting<bool>(SettingType.Shuffle, shuffle);
        }

        /// <summary>
        /// Play a song object.
        /// </summary>
        /// <param name="song">The song.</param>
        public void Play(Song song)
        {
            if (song != null)
            {
                if (!_sourceList.Any(ct => ct.Location == song.Location))
                {
                    _sourceList.Add(song);
                }

                this.Play(song.Location);
            }
        }

        /// <summary>
        /// Play from key (path).
        /// </summary>
        /// <param name="key"></param>
        public void Play(string key = null)
        {
            Song song;
            if (key == null)
            {
                if(_currentIdx == -1)
                {
                    Next();
                    return;
                }
                else
                {
                    song = _sourceList[_currentIdx];
                }
            }
            else
            {
                song = _sourceList.FirstOrDefault(ct => ct.Location == key);
            }

            if (song != null)
            {
                if (!song.IsResolved)
                {
                    SongInfoController.Resolve(song);
                }

                Load(song);
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
                if (_waveOutDevice == null)
                {
                    _waveOutDevice = new WaveOut();
                }

                _waveOutDevice.Volume = percentage / (float)100;
                DataController.SetSetting<int>(SettingType.Volume, percentage);
            }
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public int GetVolume()
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
        /// Starts a next song from the source list.
        /// </summary>
        public void Next()
        {
            if (_sourceList != null && _sourceList.Count > 0)
            {
                _currentIdx = _shuffle ? _random.Next(0, _sourceList.Count) : ++_currentIdx;
                if (_currentIdx >= _sourceList.Count)
                {
                    _currentIdx = 0;
                }
                    
                var song = _sourceList[_currentIdx];
                Play(song);
            }
        }



        /// <summary>
        /// Sets the Song position to a certain time.
        /// </summary>
        /// <param name="seconds">The second to move to.</param>
        public void MoveToTime(long seconds)
        {
            if (_playstream != null && _playstream.TotalTime.TotalSeconds > seconds)
            {
                _playstream.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }

        #endregion

        /// <summary>
        /// Updates the position of the song.
        /// </summary>
        private void UpdateTime()
        {
            while (!_disposing && _waveOutDevice != null && _currentSong != null && _playstream != null
                    && (_waveOutDevice.PlaybackState == PlaybackState.Paused || _waveOutDevice.PlaybackState == PlaybackState.Playing))
            {
                try
                {
                    if (_waveOutDevice.PlaybackState == PlaybackState.Playing)
                    {
                        _currentSong.Position = Convert.ToInt64(_playstream.CurrentTime.TotalSeconds);
                        SongChanged?.Invoke(_currentSong);
                    }

                    ThreadExtensions.SaveSleep(1000);
                }
                catch
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
            if (!_isReceiveMode && _currentSong.IsPlaying)
            {
                Next();
            }
        }

        /// <summary>
        /// Disposes of the wave out device.
        /// </summary>
        /// <param name="reinit">A boolean indicating whether to reinitialize the wave out device.</param>
        private void DisposeWaveOut(bool reinit = false)
        {
            if (_waveOutDevice != null && (_waveOutDevice.PlaybackState == PlaybackState.Playing || _waveOutDevice.PlaybackState == PlaybackState.Paused))
            {
                _waveOutDevice.PlaybackStopped -= OnWaveOutStop;
                try
                {
                    _waveOutDevice.Stop();
                }
                catch
                {
                }

                _waveOutDevice.Dispose();
            }

            if (reinit)
            {
                _waveOutDevice = new WaveOut();
                _waveOutDevice.PlaybackStopped += OnWaveOutStop;
                SetVolume(_volume);
            }
        }

        ~Player()
        {
            this.Dispose();
        }

        /// <summary>
        /// Disposes this class properly
        /// </summary>
        public void Dispose()
        {
            try
            {
                _disposing = true;
                DisposeWaveOut();

                if (_playstream != null)
                {
                    _playstream.Dispose();
                }
            }
            catch 
            {
                // TODO: fix other thread exception
            }
        }
    }
}
