using MusicPlayer.Extensions;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// New implementation to transfer data (transmit).
    /// </summary>
    internal class NetworkServer : MusicPlayerWrapper, IServer
    {
        #region Variables

        /// <summary>
        /// The port to listen on.
        /// </summary>
        private int _port;

        /// <summary>
        /// The list of server threads.
        /// </summary>
        private List<Thread> _senders = new List<Thread>();

        /// <summary>
        /// The listener.
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// When this is true server threads can run.
        /// </summary>
        private bool _run = true;

        /// <summary>
        /// A new song is being loaded.
        /// </summary>
        private bool _loading = false;

        /// <summary>
        /// The current file data.
        /// </summary>
        private byte[] _currentFile = null;

        /// <summary>
        /// The current song.
        /// </summary>
        private Song _currentSong = null;

        /// <summary>
        /// A optional priority message.
        /// </summary>
        private Message _priorityMessage;

        /// <summary>
        /// The server info.
        /// </summary>
        private ServerInfo _serverInfo;

        #endregion

        /// <summary>
        /// The server info changed.
        /// </summary>
        public event ServerInfoChanged OnInfoChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkServer" /> class.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="port"></param>
        public NetworkServer(IMusicPlayer player, int port) : base(player)
        {
            this._port = port;
            this._player.SongChanged += SongHasChanged;
            _serverInfo = new ServerInfo();
            _serverInfo.IsHost = true;
            _serverInfo.Port = port;
            _serverInfo.Clients = new Dictionary<string, int>();
            IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, port);
            _listener = new TcpListener(ipendpoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), new object());
        }

        /// <summary>
        /// Move to the time specified in seconds.
        /// </summary>
        /// <param name="seconds">The second to move to.</param>
        public override void MoveToTime(long seconds)
        {
            _player.MoveToTime(seconds);
            this.GotoPosition(new TimeSpan(0, 0, (int)seconds));
        }

        /// <summary>
        /// Disconnect the server.
        /// </summary>
        /// <returns>The music player.</returns>
        public IMusicPlayer Disconnect()
        {
            Dispose(false);
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
        /// Notifies the clients with the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Notify(string text)
        {
            _priorityMessage = CreateNotificationMessage(text);
        }

        /// <summary>
        /// The destructor.
        /// </summary>
        ~NetworkServer()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the class.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// The song has chnged, when the location changed start hosting the new song.
        /// </summary>
        /// <param name="song">The song.</param>
        internal void SongHasChanged(Song song)
        {
            if (song?.Location != _currentSong?.Location)
            {
                _loading = true;
                _currentFile = File.ReadAllBytes(song.Location);
                _currentSong = song;
                _loading = false;
            }

            InvokeSongChanged(song);
        }

        /// <summary>
        /// Notify the client to go to a certain position in the song.
        /// </summary>
        /// <param name="position">The position.</param>
        internal void GotoPosition(TimeSpan position)
        {
            _priorityMessage = CreateGotoMessage(position);
        }

        /// <summary>
        /// Dispose of the resources.
        /// </summary>
        /// <param name="disposeMusicPlayer">Dispose of the internal music player.</param>
        private void Dispose(bool disposeMusicPlayer)
        {
            _run = false;
            _listener?.Stop();
            _listener = null;
            if (disposeMusicPlayer)
            {
                _player?.Dispose();
            }
        }

        /// <summary>
        /// Create a send thread when a client connects.
        /// </summary>
        /// <param name="asyn"></param>
        private void OnClientConnect(IAsyncResult asyn)
        {
            if (_listener != null)
            {
                try
                {
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = _listener.EndAcceptTcpClient(asyn);
                    clientSocket.SendTimeout = 400;
                    if (clientSocket.Connected)
                    {
                        var remoteaddress = clientSocket.Client.RemoteEndPoint as IPEndPoint;
                        if (!_serverInfo.Clients.ContainsKey(remoteaddress.Address.ToString()))
                        {
                            _serverInfo.Clients.Add(remoteaddress.Address.ToString(), remoteaddress.Port);
                        }

                        OnInfoChanged?.Invoke(_serverInfo);
                        var tempTread = new Thread(newt => Transmit(clientSocket));
                        tempTread.Start();
                        _senders.Add(tempTread);
                    }

                    _listener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), new object());
                }
                catch 
                {
                    _listener?.Start();
                    _listener?.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), new object());
                }
            }
        }

        /// <summary>
        /// Transmits the appropriate messages to a client.
        /// </summary>
        /// <param name="socket">The socket to send the data over.</param>
        private void Transmit(TcpClient socket)
        {
            byte[] prevFile = null;
            bool sending = true;
            socket.SendBufferSize = 262144;
            socket.ReceiveBufferSize = 262144;
            NetworkStream networkStream = socket.GetStream();
            var remoteaddress = socket.Client.RemoteEndPoint as IPEndPoint;
            int currentIndex = 0;
            bool endofSongSent = false;
            Message previousPriorityMessage = null;
            Message prev = null;

            while (_run && sending)
            {
                Message messageToSend = null;
                if (_currentFile != null && _currentFile.Length > 0 && _currentSong != null && !_loading)
                {
                    if (prevFile == null || prevFile.Length != _currentFile.Length)
                    {
                        // New song
                        prevFile = _currentFile;
                        currentIndex = 0;
                        messageToSend = CreateNewSongMessage();
                        endofSongSent = false;
                    }
                    else
                    {
                        messageToSend = CreateDataMessage(ref currentIndex);
                        if (messageToSend == null)
                        {
                            if (!endofSongSent)
                            {
                                messageToSend = CreateEndOfsongMessage();
                                endofSongSent = true;
                            }
                            else if(_priorityMessage != null && _priorityMessage != previousPriorityMessage)
                            {
                                previousPriorityMessage = _priorityMessage;
                                messageToSend = _priorityMessage;
                            }
                        }
                        else
                        {
                            endofSongSent = false;
                        }
                    }

                    if (messageToSend != null)
                    {
                        byte[] serealizedData = Serialize(messageToSend);

                        try
                        {
                            SendSerializedData(networkStream, serealizedData);
                            prev = messageToSend;
                        }
                        catch
                        {
                            sending = false;
                            socket.Close();
                        }
                    }
                    else
                    {
                        Thread.Sleep(25);
                    }
                }
            }

            _serverInfo.Clients.Remove(remoteaddress.Address.ToString());
            OnInfoChanged?.Invoke(_serverInfo);
            networkStream.Dispose();
            socket.Close();
        }

        /// <summary>
        /// Send the serialized data, retries a 100 times when it failes, then throws exception.
        /// </summary>
        /// <param name="stream">The networkstream.</param>
        /// <param name="data">The data.</param>
        /// <param name="retryCount">The current number of retries.</param>
        private void SendSerializedData(NetworkStream stream, byte[] data, int retryCount = 0)
        {
            if (retryCount < 10)
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    ThreadExtensions.SaveSleep(20);
                    retryCount++;
                    SendSerializedData(stream, data, retryCount);
                }
            }
            else
            {
                throw new Exception("Client not responding, Connection failed");
            }
        }

        /// <summary>
        /// Create a new song message.
        /// </summary>
        /// <returns>The message.</returns>
        private Message CreateNewSongMessage()
        {
            Message result = new Message();
            result.Type = MessageType.NewSong;
            result.Song = _currentSong;
            result.Name = Path.GetFileName(_currentSong.Location);
            result.DataLength = _currentFile.Length;
            return result;
        }

        /// <summary>
        /// Creates an end of song message.
        /// </summary>
        /// <returns>The message.</returns>
        private Message CreateEndOfsongMessage()
        {
            Message result = new Message();
            result.Type = MessageType.EndOfSong;
            var currentTime = new TimeSpan(0, 0, (int)_currentSong.Position);
            if (currentTime != null)
            {
                _priorityMessage = CreateGotoMessage(currentTime);
            }

            return result;
        }

        /// <summary>
        /// Creates a data message for file transfer.
        /// </summary>
        /// <param name="currentIndex">The current index.</param>
        /// <returns>The message.</returns>
        private Message CreateDataMessage(ref int currentIndex)
        {
            if (_currentFile != null && currentIndex < _currentFile.Length - 1)
            {
                Message result = new Message();
                int chunksize = 128000;

                result.ID = currentIndex;
                result.Type = MessageType.Data;
                int datatoread = chunksize;
                if ((_currentFile.Length - chunksize - 1) < currentIndex)
                {
                    datatoread = _currentFile.Length - currentIndex;
                }

                byte[] data = _currentFile.Skip(currentIndex).Take(datatoread).ToArray();
                result.Data = data;
                result.DataLength = data.Length;
                currentIndex += data.Length;
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a GOTO message.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The message.</returns>
        private Message CreateGotoMessage(TimeSpan time)
        {
            Message result = new Message();
            result.Type = MessageType.Goto;
            result.Duration = time;
            return result;
        }

        /// <summary>
        /// Creates a notification message.
        /// </summary>
        /// <param name="text">The notification text.</param>
        /// <returns>The message.</returns>
        private Message CreateNotificationMessage(string text)
        {
            return new Message
            {
                Type = MessageType.Notification,
                Name = text
            };
        }

        /// <summary>
        /// Serialize an object to bytes.
        /// </summary>
        /// <param name="anySerializableObject">The object to serialize.</param>
        /// <returns>The byte array.</returns>
        private byte[] Serialize(object anySerializableObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, anySerializableObject);
                return memoryStream.ToArray();
            }
        }
    }
}
