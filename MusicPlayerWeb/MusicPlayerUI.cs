using MusicPlayer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicPlayer.Models;
using MusicPlayer;
using MusicPlayer.Interface;
using System.Threading;
using CefSharp.Wpf;
using CefSharp;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net;
using MusicPlayer.Controller;

namespace MusicPlayerWeb
{
    /// <summary>
    /// The music player UI class.
    /// </summary>
    public class MusicPlayerUI : IDisposable
    {
        /// <summary>
        /// The musicplayer instance.
        /// </summary>
        private IMusicPlayer _player;

        /// <summary>
        /// The instance for copying files.
        /// </summary>
        private ICopy _copy;
        
        /// <summary>
        /// The cromium web browser UI.
        /// </summary>
        private ChromiumWebBrowser _browser;

        /// <summary>
        /// The main dispatcher.
        /// </summary>
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// The owner window.
        /// </summary>
        private Window _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicPlayerUI" /> class.
        /// </summary>
        /// <param name="browser"></param>
        public MusicPlayerUI(ChromiumWebBrowser browser, Window window)
        {
            this._browser = browser;
            this._owner = window;
        }

        /// <summary>
        /// Load all audio files in a folder.
        /// </summary>
        /// <returns>A boolean indicating whether anything was opened.</returns>
        public bool OpenFolder()
        {
            NewPlayer();
            return _owner.Dispatcher.Invoke(() =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath += Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
                    {
                        var song = _player.LoadFolder(dialog.SelectedPath).FirstOrDefault();
                        _dispatcher.Invoke(() => _player.Play(song));
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
        /// Gets the songs.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="querry">The search querry.</param>
        /// <returns>A list of results in JSON format.</returns>
        public string GetSongs(int index, string querry)
        {
            var songs = _player?.GetSongs(index, querry);
            return JsonConvert.SerializeObject(songs);
        }

        /// <summary>
        /// Gets video info from a youtube playlist id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The serialized video info.</returns>
        public string GetVideoInfoFromPlaylist(string id)
        {
            var videoCtrl = Factory.GetVideoPlayer(_player) as IVideo;
            var task = videoCtrl.GetYoutubePlayList(id);
            task.Wait();
            return JsonConvert.SerializeObject(task.Result);
        }

        /// <summary>
        /// Gets youtube videos.
        /// </summary>
        /// <returns>The serialized video info.</returns>
        public string GetChannelVideos()
        {
            var videoCtrl = Factory.GetVideoPlayer(_player) as IVideo;
            var task = videoCtrl.GetYoutubeChannel();
            task.Wait();
            return JsonConvert.SerializeObject(task.Result);
        }

        /// <summary>
        /// Gets the default ip address.
        /// </summary>
        /// <returns>The default ip address.</returns>
        public string GetDefaultIP()
        {
            return DataController.GetSetting<string>(SettingType.RemoteIP, "127.0.0.1");
        }

        /// <summary>
        /// Gets the current song.
        /// </summary>
        /// <returns>The current song.</returns>
        public string GetCurrentSong()
        {
            var currentSong = _player?.GetCurrentSong();
            return JsonConvert.SerializeObject(currentSong);
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
                Song song = JsonConvert.DeserializeObject<Song>(jsonSong);
                _player?.Play(song);
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
            SongChanged(null);
        }

        /// <summary>
        /// Gets the shuffle setting.
        /// </summary>
        /// <returns>The shuffle setting.</returns>
        public bool? GetShuffle()
        {
            return _player?.GetShuffle();
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
        /// Gets the current volume.
        /// </summary>
        /// <returns>The volume.</returns>
        public int? GetVolume()
        {
            return _player?.GetVolume();
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
            _player.SongChanged += SongChanged;
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
        /// The copy progress has changed.
        /// </summary>
        /// <param name="percentage">The new percentage.</param>
        private void CopyProgressChanged(double percentage)
        {
            _browser?.ExecuteScriptAsync("window.CSSharpDispatcher.dispatchSetCopyProgress", percentage != 100 ? (double?)percentage : null);
        }

        /// <summary>
        /// The song has changed.
        /// </summary>
        /// <param name="song">The new song.</param>
        private void SongChanged(Song song)
        {
            _browser?.ExecuteScriptAsync("window.CSSharpDispatcher.dispatchSetCurrentSong", JsonConvert.SerializeObject(song).Replace("\\", "\\\\"));
        }

        /// <summary>
        /// The server info changed.
        /// </summary>
        /// <param name="serverInfo">The new server info.</param>
        private void ServerInfoChanged(ServerInfo serverInfo)
        {
            _browser?.ExecuteScriptAsync("window.CSSharpDispatcher.dispatchSetServerInfo", JsonConvert.SerializeObject(serverInfo).Replace("\\", "\\\\"));
        }

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="player">The new player.</param>
        private void NewPlayer(IMusicPlayer player = null)
        {
            var serverInfo = (_player as IServer)?.GetInfo();
            _player?.Dispose();
            _player = null;
            if (serverInfo != null)
            {
                HostServer(serverInfo.Port);
            }
            else
            {
                _player = player == null ? Factory.GetPlayer() : player;
                _player.SongChanged += SongChanged;
            }
        }

        /// <summary>
        /// Dispose of the class.
        /// </summary>
        public void Dispose()
        {
            _browser?.Dispose();
            _browser = null;
            _player?.Dispose();
        }
    }
}
