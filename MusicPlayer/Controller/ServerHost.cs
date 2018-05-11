using MusicPlayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.IO;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The WCF service host.
    /// </summary>
    internal class ServerHost : MusicPlayerWrapper, IServer
    {
        /// <summary>
        /// The wcf service.
        /// </summary>
        private ServiceHost _service;

        /// <summary>
        /// The port at which the wcf service is hosted.
        /// </summary>
        private int _port;

        /// <summary>
        /// The serverinfo.
        /// </summary>
        private ServerInfo _serverInfo;

        /// <summary>
        /// The connected clients.
        /// </summary>
        private List<Client> _clients;

        /// <summary>
        /// The current song.
        /// </summary>
        private Song _currentSong;

        /// <summary>
        /// The muisicplayer.
        /// </summary>
        private IMusicPlayer _player;

        /// <summary>
        /// The event that must be called when the serverinfo changes.
        /// </summary>
        public event ServerInfoChanged OnInfoChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHost" /> class.
        /// </summary>
        /// <param name="player">The underlying music player.</param>
        /// <param name="port">The port to host the service at.</param>
        public ServerHost(IMusicPlayer player, int port) : base(player)
        {
            _port = port;
            player.SongChanged += Player_SongChanged;
            _clients = new List<Client>();
            _player = player;
            InitWCFHost();
        }

        /// <summary>
        /// Pauses or plays the music.
        /// </summary>
        /// <returns>A boolean indicating whether the music is playing.</returns>
        public override bool TogglePlay(bool? pause)
        {
            bool isPlaying = base.TogglePlay(pause);
            foreach (var client in _clients.ToList())
            {
                try
                {
                    if (!isPlaying)
                    {
                        client.ClientContract.Pause();
                    }
                    else
                    {
                        client.ClientContract.Play();
                    }
                }
                catch (Exception e)
                {
                    HandleClientError(client, e);
                }
            }

            return isPlaying;
        }

        /// <summary>
        /// Move to a time in the song.
        /// </summary>
        /// <param name="seconds">The second to move to.</param>
        public override void MoveToTime(long seconds)
        {
            base.MoveToTime(seconds);
            foreach (var client in _clients.ToList())
            {
                try
                {
                    client.ClientContract.SetSongPosition(seconds);
                }
                catch (Exception e)
                {
                    HandleClientError(client, e);
                }
            }
        }

        /// <summary>
        /// Dispose of the server.
        /// </summary>
        public override void Dispose()
        {
            QuitService();
            base.Dispose();
        }

        /// <summary>
        /// Disconnect the WCF service.
        /// </summary>
        /// <returns>The underlying music player.</returns>
        public IMusicPlayer Disconnect()
        {
            QuitService();
            return _player;
        }

        /// <summary>
        /// Gets the server info.
        /// </summary>
        /// <returns>The server info.</returns>
        public ServerInfo GetInfo()
        {
            return _serverInfo;
        }

        /// <summary>
        /// Sets the video.
        /// </summary>
        /// <param name="url">The video url.</param>
        public void SetVideo(string url)
        {
            foreach (var client in _clients.ToList())
            {
                try
                {
                    client.ClientContract.PlayVideo(url);
                }
                catch (Exception e)
                {
                    HandleClientError(client, e);
                }
            }
        }

        /// <summary>
        /// Seek to a video position.
        /// </summary>
        /// <param name="position">The position in seconds.</param>
        public void SeekVideo(double position)
        {
            foreach (var client in _clients.ToList())
            {
                try
                {
                    client.ClientContract.SeekVideo(position);
                }
                catch (Exception e)
                {
                    HandleClientError(client, e);
                }
            }
        }

        /// <summary>
        /// Disconnect the WCF service.
        /// </summary>
        private void QuitService()
        {
            if (_service?.State == CommunicationState.Opened)
            {
                foreach (var client in _clients.ToList())
                {
                    try
                    {
                        client.ClientContract.Disconnect();
                    }
                    catch { }
                }
                
                _service?.Close();
            }

            _service = null;
        }

        /// <summary>
        /// Initializes the WCF host.
        /// </summary>
        private void InitWCFHost()
        {
            if (_service != null)
            {
                QuitService();
            }

            string address = $"net.tcp://localhost:{_port}/";
            _service = new ServiceHost(typeof(WCFServerService), new Uri[] { new Uri(address) });
            WCFServerService.Player = _player;
            NetTcpContextBinding binding = new NetTcpContextBinding(SecurityMode.None);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            binding.SendTimeout = new TimeSpan(0, 1, 0);
            _service.AddServiceEndpoint(typeof(IServerContract), binding, address);
            _service.Open();

            _serverInfo = new ServerInfo
            {
                Clients = new Dictionary<string, int>(),
                IsHost = true,
                Port = _port
            };

            WCFServerService.ClientConnected += WCFServerService_ClientConnected;
            WCFServerService.ClientDisconnected += WCFServerService_ClientDisconnected;
        }

        /// <summary>
        /// A client disconnected.
        /// </summary>
        /// <param name="ip">The ip address of the client.</param>
        /// <param name="port">The port the client was connected to.</param>
        private void WCFServerService_ClientDisconnected(string ip, int port)
        {
            var client = _clients.FirstOrDefault(c => c.IpAddress == ip && c.Port == port);
            if (client != null)
            {
                _clients.Remove(client);
                _serverInfo.Clients = _clients.ToDictionary(c => c.IpAddress, c => c.Port);
                OnInfoChanged?.Invoke(_serverInfo);
            }
        }

        /// <summary>
        /// The client connected.
        /// </summary>
        /// <param name="client">The connected client.</param>
        private void WCFServerService_ClientConnected(Client client)
        {
            if (!_clients.Any(c => c.IpAddress == client.IpAddress && c.Port == client.Port))
            {
                _clients.Add(client);
                _serverInfo.Clients = _clients.GroupBy(c => c.IpAddress).Select(g => g.First()).ToDictionary(c => c.IpAddress, c => c.Port);
                OnInfoChanged?.Invoke(_serverInfo);
                var currentsong = _player.GetCurrentSong();
                if (currentsong != null)
                {
                    Play(currentsong, client);
                }
            }
        }

        /// <summary>
        /// The current song has changed.
        /// </summary>
        /// <param name="song">The song.</param>
        private void Player_SongChanged(Song song)
        {
            if (song.Location != _currentSong?.Location)
            {
                _currentSong = song;
                foreach (var client in _clients.ToList())
                {
                    Play(song, client);
                }
            }
        }

        /// <summary>
        /// Play the song on the client.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <param name="client">The client.</param>
        private void Play(Song song, Client client)
        {
            try
            {
                song.FileName = Path.GetFileName(song.Location);
                client.ClientContract.SetSong(song);
                client.ClientContract.SendFile(new MemoryStream(File.ReadAllBytes(song.Location)));
            }
            catch (Exception e)
            {
                HandleClientError(client, e);
            }
        }

        /// <summary>
        /// A client could not be reached.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The exception.</param>
        private void HandleClientError(Client client, Exception e)
        {
            var cl = _clients.FirstOrDefault(c => c.IpAddress == client.IpAddress && c.Port == client.Port);
            if (cl != null)
            {
                _clients.Remove(cl);
                _serverInfo.Clients = _clients.GroupBy(c => c.IpAddress).Select(g => g.First()).ToDictionary(c => c.IpAddress, c => c.Port);
                OnInfoChanged?.Invoke(_serverInfo);
            }
        }
    }
}
