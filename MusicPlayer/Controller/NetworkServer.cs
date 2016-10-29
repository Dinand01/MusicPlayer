using MusicPlayer.Extensions;
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
    /// When finished the NetworkControllers will be removed.
    /// </summary>
    public class NetworkServer
    {
        #region Variables

        private int _port;

        private Player _player;

        private int _transmitInterval = 500;

        private List<Thread> _senders = new List<Thread>();

        private TcpListener _listener;

        private bool _run = true;

        private bool _loading = false;

        private byte[] _currentFile = null;

        private Song _currentSong = null;

        private Message _priorityMessage;
        #endregion

        public NetworkServer(int port, Player player)
        {
            this._port = port;
            this._player = player;
            IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, port);
            _listener = new TcpListener(ipendpoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), new object());
        }

        /// <summary>
        /// Sets a song to be hosted.
        /// </summary>
        /// <param name="song">The song.</param>
        public void HostSong(Song song)
        {
            _loading = true;
            _currentFile = File.ReadAllBytes(song.Location);
            _currentSong = song;
            _loading = false;
        }

        /// <summary>
        /// Notify the client to go to a certain position in the song.
        /// </summary>
        /// <param name="position">The position.</param>
        public void GotoPosition(TimeSpan position)
        {
            _priorityMessage = CreateGotoMessage(position);
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
        /// Disposes of the class.
        /// </summary>
        public void Dispose()
        {
            _run = false;
            if (_listener != null)
            {
                _listener.Stop();
            }
        }

        /// <summary>
        /// Create a send thread when a client connects.
        /// </summary>
        /// <param name="asyn"></param>
        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                TcpClient clientSocket = default(TcpClient);
                clientSocket = _listener.EndAcceptTcpClient(asyn);
                clientSocket.SendTimeout = 400;

                var tempTread = new Thread(newt => Transmit(clientSocket));
                tempTread.Start();
                _senders.Add(tempTread);
                _listener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), new object());
            }
            catch (Exception se)
            {
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
            if(retryCount < 10)
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
            var currentTime = _player.GetCurrentTime();
            if (currentTime != null)
            {
                _priorityMessage = CreateGotoMessage((TimeSpan)currentTime);
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
