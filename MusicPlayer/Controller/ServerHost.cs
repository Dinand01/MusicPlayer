using MusicPlayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayer.Models;
using System.IO;
using MusicPlayer.Communication;
using MusicPlayer.Core.Models;
using MusicPlayer.Core.Communication.StreamingMessages;
using MusicPlayer.Core.Player;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The WCF service host.
    /// </summary>
    internal class ServerHost : MusicPlayerWrapper, IServer
    {
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
        private SongInformation _currentSong;

        /// <summary>
        /// The muisicplayer.
        /// </summary>
        private new IMusicPlayer _player;

        private readonly IMusicPlayerServer _server;

        /// <summary>
        /// The event that must be called when the serverinfo changes.
        /// </summary>
        public event ServerInfoChanged OnInfoChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHost" /> class.
        /// </summary>
        /// <param name="player">The underlying music player.</param>
        /// <param name="port">The port to host the service at.</param>
        public ServerHost(IMusicPlayer player, IMusicPlayerServer server) : base(player)
        {
            player.SongChanged += Player_SongChanged;
            _clients = new List<Client>();
            _player = player;
            _server = server;
            _server.Enable();
            _server.ClientConnected += WCFServerService_ClientConnected;
            _server.ClientDisconnected += WCFServerService_ClientDisconnected;
            InitWCFHost();
        }

        /// <summary>
        /// Pauses or plays the music.
        /// </summary>
        /// <returns>A boolean indicating whether the music is playing.</returns>
        public override bool TogglePlay(bool? pause)
        {
            bool isPlaying = base.TogglePlay(pause);
            SendToClients(new TogglePlayMessage { Playing = isPlaying });
            return isPlaying;
        }

        /// <summary>
        /// Move to a time in the song.
        /// </summary>
        /// <param name="seconds">The second to move to.</param>
        public override void MoveToTime(long seconds)
        {
            base.MoveToTime(seconds);
            SendToClients(new SeekSongMessage { Position = seconds });
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
            SendToClients(new PlayVideoMessage { Location = url });
        }

        /// <summary>
        /// Seek to a video position.
        /// </summary>
        /// <param name="position">The position in seconds.</param>
        public void SeekVideo(double position)
        {
            SendToClients(new SeekVideoMessage { Position = position });
        }

        /// <summary>
        /// Disconnect the service.
        /// </summary>
        private void QuitService()
        {
            _server.Disable();
        }

        /// <summary>
        /// Runs a certain action on all clients.
        /// </summary>
        /// <param name="action">The action.</param>
        private void SendToClients<T>(T action) where T : StreamingMessage
        {
            foreach (var client in _clients.ToList())
            {
                try
                {
                    client.SendMessage(action);
                }
                catch (Exception e)
                {
                    HandleClientError(client, e);
                }
            }
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


        }

        /// <summary>
        /// A client disconnected.
        /// </summary>
        /// <param name="ip">The ip address of the client.</param>
        /// <param name="port">The port the client was connected to.</param>
        private void WCFServerService_ClientDisconnected(string peer)
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
        private void Player_SongChanged(SongInformation song)
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
        private void Play(SongInformation song, Client client)
        {
            Task.Run(() =>
            {
                try
                {
                    song.FileName = Path.GetFileName(song.Location);
                    client.ClientContract.SetSong(song);
                    if (song.IsInternetLocation)
                    {
                        client.ClientContract.PlayRadio(song, song.Location);
                    }
                    else if (File.Exists(song.Location))
                    {
                        client.ClientContract.SendFile(new MemoryStream(File.ReadAllBytes(song.Location)));
                    }
                }
                catch (Exception e)
                {
                    HandleClientError(client, e);
                }
            });
        }

        /// <summary>
        /// A client could not be reached.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="e">The exception.</param>
        private void HandleClientError(Client client, Exception e)
        {
            var cl = _clients.FirstOrDefault(c => c.Peer == client.Peer);
            if (cl != null)
            {
                lock (_clients)
                { 
                    _clients.Remove(cl);
                    _serverInfo.Clients = new HashSet<string>(_clients.Select(c => c.Peer));
                    OnInfoChanged?.Invoke(_serverInfo);
                }
            }
        }
    }
}
