using MusicPlayer.Extensions;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using MusicPlayer.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// The network client music player.
    /// </summary>
    internal class NetworkClient : MusicPlayerWrapper, IClient, INetwork
    {
        #region Variables

        /// <summary>
        /// The ip address of the host.
        /// </summary>
        private IPAddress _ip;

        /// <summary>
        /// Thge port of the host.
        /// </summary>
        private int _port;

        /// <summary>
        /// When this is set playback will not be retried until the end of file is retrieved.
        /// </summary>
        private bool _waitUntilEnd;

        /// <summary>
        /// The current index in the file.
        /// </summary>
        private int _currentFileIndex;

        /// <summary>
        /// The current song.
        /// </summary>
        private Song _currentSong;

        /// <summary>
        /// The client socket.
        /// </summary>
        private TcpClient _clientSocket;

        /// <summary>
        /// The receiving thread.
        /// </summary>
        private Thread _receiver;

        /// <summary>
        /// Allows the thread to run.
        /// </summary>
        private bool _run = true;

        /// <summary>
        /// The amount of errors encountered on transfer.
        /// </summary>
        private int _errorCount = 0;

        /// <summary>
        /// The song that were received song this object exists.
        /// </summary>
        private List<Song> _receivedSongs = new List<Song>();

        #endregion

        /// <summary>
        /// The server info changed.
        /// </summary>
        public event ServerInfoChanged OnInfoChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkClient"/> class. 
        /// </summary>
        /// <param name="address">The ip address.</param>
        /// <param name="port">The port.</param>
        public NetworkClient(IPAddress address, int port) : base(Factory.GetPlayerForReceiveMode())
        {
            this._ip = address;
            this._port = port;
            _clientSocket = CreateTcpClient();

            _receiver = new Thread(newt => Receive());
            _receiver.Start();
        }

        /// <summary>
        /// Gets a list of received song.
        /// </summary>
        public override List<Song> GetSongs(int index = 0, string querry = null, int amount = 50)
        {
            return _receivedSongs;
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        /// <returns>A disconnected music player.</returns>
        public IMusicPlayer Disconnect()
        {
            Dispose();
            return Factory.GetPlayer();
        }

        /// <summary>
        /// Gets the server info.
        /// </summary>
        /// <returns>The info.</returns>
        public ServerInfo GetInfo()
        {
            if (_clientSocket?.Connected != true)
            {
                return null;
            }

            return new ServerInfo
            {
                IsHost = false,
                Host = _ip.ToString(),
                Clients = null,
                Port = _port
            };
        }

        /// <summary>
        /// Disposes of the client.
        /// </summary>
        public override void Dispose()
        {
            _run = false;
            base.Dispose();
            _clientSocket?.Close();
        }

        /// <summary>
        /// Receives messages from the audio server and takes the appropriate action.
        /// </summary>
        private void Receive()
        {
            if (_clientSocket != null)
            {
                _clientSocket.ReceiveBufferSize = 262144;
                _clientSocket.SendBufferSize = 262144;
                NetworkStream stream = _clientSocket.GetStream();
                FileStream filestream = null;
                Message previousMessage = null;
                bool error = false;

                while (_run && !error)
                {
                    if (stream != null && stream.CanRead)
                    {
                        try
                        {
                            var formatter = new BinaryFormatter();
                            Message message = (Message)formatter.Deserialize(stream);
                            HandleMessages(message, previousMessage, ref filestream);
                            previousMessage = message;
                            error = false;
                            _errorCount = 0;
                        }
                        catch (Exception e)
                        {
                            ThreadExtensions.SaveSleep(10);
                            _errorCount++;

                            if (_errorCount > 10)
                            {
                                error = true;
                                ThreadExtensions.SaveSleep(2000);
                                _clientSocket = CreateTcpClient();
                                if (_clientSocket != null)
                                {
                                    _receiver = new Thread(newt => Receive());
                                    _receiver.Start();
                                }
                            }
                        }
                    }
                    else
                    {
                        ThreadExtensions.SaveSleep(10);
                    }
                }

                if (!error)
                {
                    stream.Dispose();
                    _clientSocket.Close();
                }
            }
        }

        /// <summary>
        /// Handles the differnt types of messages.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="previous">The previous message that was received.</param>
        private void HandleMessages(Message message, Message previous, ref FileStream stream)
        {
            switch (message.Type)
            {
                case MessageType.NewSong:
                    stream?.Close();
                    stream?.Dispose();
                    stream = null;
                    stream = HandleNewSongMessage(message);
                    break;
                case MessageType.Pause:
                    base.TogglePlay(true);
                    break;
                case MessageType.Play:
                    base.TogglePlay(false);
                    break;
                case MessageType.Data:
                    HandleDataMessage(message, previous, ref stream, 0);
                    break;
                case MessageType.EndOfSong:
                    HandleEndOfSongMessage(message, ref stream);
                    _errorCount = 0;
                    break;
                case MessageType.Goto:
                    HandleGotoMessage(message);
                    break;
                case MessageType.Notification:
                    break;
                case MessageType.Video:
                    var info = GetInfo();
                    info.VideoUrl = Deserialize<string>(message.Data);
                    OnInfoChanged?.Invoke(info);
                    break;
                default:
                    throw new Exception("Unknown message type received, please update your client.");
            }
        }

        /// <summary>
        /// Hanldes a new song message, sets the current song and creates the file at the exceutable location.
        /// TODO: check if user already has the file.
        /// </summary>
        /// <param name="message">The message.</param>
        private FileStream HandleNewSongMessage(Message message)
        {
            FileStream result = null;
            _waitUntilEnd = false;
            ////var currentDir = Directory.GetCurrentDirectory();
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\Music player downloaded files\\" + message.Name;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            _currentSong = message.Song;
            _currentSong.Location = filePath;
            _currentSong.DateAdded = DateTime.Now;
            _currentFileIndex = 0;

            try
            {
                if (File.Exists(_currentSong.Location))
                {
                    File.Delete(_currentSong.Location);
                }

                result = new FileStream(_currentSong.Location, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                result.SetLength(message.DataLength);
            }
            catch (Exception e)
            {
                throw new Exception("Access was denied, you do not have the apropriate permissions in the directory wher this excecutable is.");
            }

            return result;
        }

        /// <summary>
        /// Handles a data message, writes to the file and to the _currentFile byte array.
        /// Invokes Player.play when enough data is present.
        /// </summary>
        /// <param name="message">The message.</param>
        private void HandleDataMessage(Message message, Message previous, ref FileStream openStream, int retryCount)
        {
            try
            {
                if (openStream.CanWrite)
                {
                    if (previous.ID != message.ID)
                    {
                        openStream.Seek(message.ID, SeekOrigin.Begin);
                        openStream.Write(message.Data, 0, message.Data.Length);
                        _currentFileIndex += message.Data.Length;
                    }
                    else
                    {
                        openStream.Seek(message.ID, SeekOrigin.Begin);
                        openStream.Write(message.Data, 0, message.Data.Length);
                        Debug.WriteLine("Duplicate received.");
                    }

                    if (_currentFileIndex > 1000000 && !_waitUntilEnd)
                    {
                        _receivedSongs.Add(_currentSong);
                        _player.Play(_currentSong);
                        _waitUntilEnd = true;
                    }
                }
            }
            catch (Exception e)
            {
                if (retryCount == 10)
                {
                    throw e;
                }

                HandleDataMessage(message, previous, ref openStream, retryCount +1);
            }
        }

        /// <summary>
        /// Handles the end of song message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void HandleEndOfSongMessage(Message message, ref FileStream stream)
        {
            stream?.Close();
            stream = null;
            _currentFileIndex = 0;
            _waitUntilEnd = false;
            if (_player?.GetSongPosition() == null || _player?.GetSongPosition() == 0)
            {
                _player.Play(_currentSong);
            }
        }

        /// <summary>
        /// Handles a goto message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void HandleGotoMessage(Message message)
        {
            _player?.MoveToTime(Convert.ToInt64((message.Duration + new TimeSpan(0, 0, 0, 0, 100)).TotalSeconds));
        }

        /// <summary>
        /// Creates a Tcp Client.
        /// </summary>
        /// <returns></returns>
        private TcpClient CreateTcpClient()
        {
            TcpClient clientSocket = null;
            try
            {
                clientSocket = new TcpClient();
                clientSocket.ReceiveBufferSize = 64000;
                clientSocket.Connect(_ip, _port);
            }
            catch
            {
                clientSocket = null;
            }

            return clientSocket;
        }

        /// <summary>
        /// Deserializes a byte array into an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="data">The byte array.</param>
        /// <returns>The object.</returns>
        private T Deserialize<T>(byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                return (T)(new BinaryFormatter()).Deserialize(memStream);
            }
        }
    }
}
