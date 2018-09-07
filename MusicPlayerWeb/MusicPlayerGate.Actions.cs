using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicPlayer;
using MusicPlayer.Controller;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using Newtonsoft.Json;

namespace MusicPlayerWeb
{
    /// <summary>
    /// The partial containing actions a user can perform. 
    /// </summary>
    public partial class MusicPlayerGate
    {
        /// <summary>
        /// Load all audio files in a folder.
        /// </summary>
        /// <returns>A boolean indicating whether anything was opened.</returns>
        public bool OpenFolder()
        {
            return _owner.Dispatcher.Invoke(() =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath += Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
                    {
                        LoadFolder(dialog.SelectedPath);
                        return true;
                    }

                    return false;
                }
            });
        }

        /// <summary>
        /// Load all audio files in a folder.
        /// </summary>
        /// <param name="files">The files to load.</param>
        /// <returns>A boolean indicating whether anything was opened.</returns>
        public bool OpenFiles(string[] files = null)
        {
            NewPlayer();
            if (files == null)
            {
                return _owner.Dispatcher.Invoke(() =>
                {
                    using (var dialog = new OpenFileDialog())
                    {
                        dialog.Filter = "Audio files |*.mp3;*.flac;*.wma|All files (*.*)|*.*";
                        dialog.Multiselect = true;
                        DialogResult result = dialog.ShowDialog();
                        if (result == DialogResult.OK && dialog.FileNames.Length > 0)
                        {
                            var song = _player.LoadFiles(dialog.FileNames).FirstOrDefault();
                            _dispatcher.Invoke(() => _player.Play(song));
                            return true;
                        }

                        return false;
                    }
                });
            }
            else
            {
                var song = _player.LoadFiles(files).FirstOrDefault();
                _dispatcher.Invoke(() => _player.Play(song));
                return true;
            }
        }

        /// <summary>
        /// Lets the user select a folder, returns the selection.
        /// </summary>
        /// <returns>The folder selection.</returns>
        public string SelectFolder()
        {
            string result = string.Empty;
            _owner.Dispatcher.Invoke(() =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath += Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    DialogResult diagResult = dialog.ShowDialog();
                    if (diagResult == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
                    {
                        result = dialog.SelectedPath;
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Go to the next song when possible.
        /// </summary>
        public void NextSong()
        {
            _owner.Dispatcher.Invoke(() =>
            {
                _player?.Next();
            });
        }

        /// <summary>
        /// Play a specific song.
        /// </summary>
        /// <param name="jsonSong">The json song.</param>
        public void Play(string jsonSong)
        {
            _owner.Dispatcher.Invoke(() =>
            {
                SongInformation song = JsonConvert.DeserializeObject<SongInformation>(jsonSong);
                _player?.Play(song);
            });
        }

        /// <summary>
        /// Plays a song from it's url.
        /// </summary>
        /// <param name="url">The url.</param>
        public void PlayFromURL(string url)
        {
            NewPlayer();
            _owner.Dispatcher.Invoke(() =>
            {
                _player?.Play(url);
            });
        }

        /// <summary>
        /// Toggles the play/pause.
        /// </summary>
        public void TogglePlay()
        {
            _owner.Dispatcher.Invoke(() =>
            {
                _player?.TogglePlay();
            });
        }

        /// <summary>
        /// Move to the requested time.
        /// </summary>
        /// <param name="seconds">The seconds to move to.</param>
        public void MoveToTime(long seconds)
        {
            _owner.Dispatcher.Invoke(() =>
            {
                _player?.MoveToTime(seconds);
            });
        }

        /// <summary>
        /// Seek to a video position.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        public void SeekVideo(double position)
        {
            var ctrl = _player as IVideo;
            ctrl?.Seek(position);
        }

        /// <summary>
        /// Stop playing music.
        /// </summary>
        public void Stop()
        {
            _player?.Dispose();
            _player = null;
            SongChanged(null);
            ServerInfoChanged(null);
        }

        /// <summary>
        /// Change the shuffle setting.
        /// </summary>
        /// <param name="shuffle">The shuffle setting.</param>
        public void Shuffle(bool shuffle)
        {
            _player?.SetShuffle(shuffle);
        }

        /// <summary>
        /// Sets the volume.
        /// </summary>
        /// <param name="percentage">The percentage to set the volume to.</param>
        public void SetVolume(int percentage)
        {
            _player?.SetVolume(percentage);
        }

        /// <summary>
        /// Host a server.
        /// </summary>
        /// <param name="port">The port to host the server on.</param>
        public void HostServer(int port)
        {
            _player = Factory.GetServerPlayer(port, _player);
            var server = (_player as IServer);
            server.OnInfoChanged += ServerInfoChanged;
            ServerInfoChanged(server.GetInfo());
        }

        /// <summary>
        /// Connect to a music server.
        /// </summary>
        /// <param name="ip">The ip address.</param>
        /// <param name="port">The port.</param>
        public void ConnectToServer(string ip, int port)
        {
            IPAddress address;
            if (IPAddress.TryParse(ip, out address))
            {
                DataController.SetSetting<string>(SettingType.RemoteIP, ip);
                NewPlayer(Factory.GetClientPlayer(address, port, _player));
                var client = _player as IClient;
                client.OnInfoChanged += ServerInfoChanged;
                this.ServerInfoChanged(client?.GetInfo());
            }
        }

        /// <summary>
        /// Disconnect the server.
        /// </summary>
        public void DisconnectServer()
        {
            var network = (_player as INetwork);
            if (network != null)
            {
                network.OnInfoChanged -= ServerInfoChanged;
                _player = network.Disconnect();
                this.ServerInfoChanged(null);
            }
        }

        /// <summary>
        /// Copies randome song from a source to a destinations foldr.
        /// </summary>
        /// <param name="source">The source folder.</param>
        /// <param name="dest">The destination folder.</param>
        /// <param name="number">The amount of song to copy.</param>
        public void CopySongs(string source, string dest, int number)
        {
            if (_copy != null)
            {
                _copy.ProgressChanged -= CopyProgressChanged;
            }

            _copy = Factory.GetCopy();
            _copy.ProgressChanged += CopyProgressChanged;
            _copy.CopyRandomSongs(source, dest, number);
        }

        /// <summary>
        /// Start a video.
        /// </summary>
        /// <param name="url">The url of the video.</param>
        public void StartVideo(string url)
        {
            _player = Factory.GetVideoPlayer(_player);
            var video = _player as IVideo;
            video.StartVideo(url);
        }

        /// <summary>
        /// Stop the video.
        /// </summary>
        public void StopVideo()
        {
            var video = _player as IVideo;
            if (video != null)
            {
                _player = video.StopVideo();
            }
        }

        /// <summary>
        /// Loads the folder by it's path.
        /// </summary>
        /// <param name="path">The folder path.</param>
        internal void LoadFolder(string path)
        {
            NewPlayer();
            _player.LoadFolder(path);
            _dispatcher.Invoke(() => _player.Next());
        }
    }
}
