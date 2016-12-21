using MusicPlayer.Extensions;
using MusicPlayer.Models;
using MusicPlayer.UI;
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
    internal class NetworkClient
    {
        #region Variables

        /// <summary>
        /// The user interface.
        /// </summary>
        private IUI _gui;

        private IPAddress _ip;

        private int _port;

        private Player _player;

        private int _currentFileIndex;

        private Song _currentSong;

        private TcpClient _clientSocket;

        private Thread _receiver;

        private bool _run = true;

        private int _errorCount = 0;

        /// <summary>
        /// The song that were received song this object exists.
        /// </summary>
        private List<Song> _receivedSongs = new List<Song>();

        /// <summary>
        /// Gets a list of received song.
        /// </summary>
        public List<Song> ReceivedSongs
        {
            get
            {
                return _receivedSongs;
            }
        }

        #endregion

        public NetworkClient(IUI gui, IPAddress address, int port)
        {
            this._gui = gui;
            this._ip = address;
            this._port = port;

            _clientSocket = CreateTcpClient();

            _receiver = new Thread(newt => Receive());
            _receiver.Start();
        }

        /// <summary>
        /// Gets the volume of the embedded player.
        /// </summary>
        /// <returns>The volume.</returns>
        public int GetVolume()
        {
            return Player.GetVolume();
        }

        /// <summary>
        /// Sets the volume of the embedded player.
        /// </summary>
        /// <param name="value">The volume.</param>
        public void SetVolume(int value)
        {
            if(_player != null)
            {
                _player.SetVolume(value);
            }
        }

        /// <summary>
        /// Disposes of this thread;
        /// </summary>
        public void Dispose()
        {
            _run = false;
            if (_player != null)
            {
                _player.Dispose();
            }

            if (_clientSocket != null)
            {
                _clientSocket.Close();
            }
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
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }

                    stream = HandleNewSongMessage(message);
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
                    _gui.SetNotification(message.Name);
                    break;
                default:
                    throw new Exception("Unknown message type received, please update your client.");
                    break;
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
            if (this._player != null)
            {
                this._player.Dispose();
                this._player = null;
            }

            var currentDir = Directory.GetCurrentDirectory();
            var filePath = currentDir + "\\NetworkFiles\\" + message.Name;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            _currentSong = message.Song;
            _currentSong.Location = filePath;
            _currentSong.SourceIsDb = true;
            _currentSong.DateAdded = DateTime.Now;
            _currentSong.DateCreated = DateTime.Now;
            _currentFileIndex = 0;

            try
            {
                if (File.Exists(_currentSong.Location))
                {
                    File.Delete(_currentSong.Location);
                    // TODO: implement a way to just read the file and not download it anymore (requires 2 way com)
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
                        Console.WriteLine("Duplicate received.");
                    }

                    if (_currentFileIndex > 1000000 && this._player == null)
                    {
                        try
                        {
                            this._player = new Player(_gui, true);
                            _receivedSongs.Add(_currentSong);
                            _player.SetSongs(_receivedSongs);
                            _player.Play(_currentSong);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error on preplay, waiting till end of file.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Can't write to open stream.");
                }
            }
            catch (Exception e)
            {
                if(retryCount == 10)
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
            if (stream != null)
            {
                stream.Close();
                stream = null;
                _currentFileIndex = 0;
            }

            if (!_player.IsPlaying())
            {
                _player.SetSongs(_receivedSongs);
                _player.Play(_currentSong);
            }
        }

        /// <summary>
        /// Handles a goto message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void HandleGotoMessage(Message message)
        {
            if (_player != null)
            {
                var current = _player.GetCurrentTime();
                if (current != null && ((TimeSpan)(current - message.Duration)).Duration() > new TimeSpan(0, 0, 0, 0, 100))
                {
                    _player.MoveToTime(message.Duration + new TimeSpan(0, 0, 0, 0, 100));
                }
            }
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
    }
}
