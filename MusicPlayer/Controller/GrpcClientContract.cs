using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MusicPlayer.Protos;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// gRPC implementation for the client contract.
    /// Acts as a client to call the WPF app's gRPC service.
    /// </summary>
    internal class GrpcClientContract : IClientContract
    {
        private readonly Protos.MusicPlayerClientService.MusicPlayerClientServiceClient _client;
        private readonly GrpcChannel _channel;
        private SongInformation _currentSong;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcClientContract"/> class.
        /// </summary>
        /// <param name="serverUrl">The gRPC server URL (WPF app's service).</param>
        public GrpcClientContract(string serverUrl)
        {
            _channel = GrpcChannel.ForAddress(serverUrl);
            _client = new Protos.MusicPlayerClientService.MusicPlayerClientServiceClient(_channel);
        }

        /// <summary>
        /// The server will disconnect.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _client.Disconnect(new DisconnectRequest());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC Disconnect error: {ex.Message}");
            }
        }

        /// <summary>
        /// Disconnect async.
        /// </summary>
        public Task DisconnectAsync()
        {
            return _client.DisconnectAsync(new DisconnectRequest()).ResponseAsync;
        }

        /// <summary>
        /// Pause the music or video.
        /// </summary>
        public void Pause()
        {
            try
            {
                _client.Pause(new PauseRequest());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC Pause error: {ex.Message}");
            }
        }

        /// <summary>
        /// Pause async.
        /// </summary>
        public Task PauseAsync()
        {
            return _client.PauseAsync(new PauseRequest()).ResponseAsync;
        }

        /// <summary>
        /// Play the music or video.
        /// </summary>
        public void Play()
        {
            try
            {
                _client.Play(new PlayRequest());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC Play error: {ex.Message}");
            }
        }

        /// <summary>
        /// Play async.
        /// </summary>
        public Task PlayAsync()
        {
            return _client.PlayAsync(new PlayRequest()).ResponseAsync;
        }

        /// <summary>
        /// Play from an online location.
        /// </summary>
        /// <param name="radioInfo">The radio station.</param>
        /// <param name="url">The url of the station.</param>
        public void PlayRadio(SongInformation radioInfo, string url)
        {
            try
            {
                var request = new PlayRadioRequest
                {
                    RadioInfo = ConvertToProtoSong(radioInfo),
                    Url = url
                };
                _client.PlayRadio(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC PlayRadio error: {ex.Message}");
            }
        }

        /// <summary>
        /// PlayRadio async.
        /// </summary>
        public Task PlayRadioAsync(SongInformation radioInfo, string url)
        {
            var request = new PlayRadioRequest
            {
                RadioInfo = ConvertToProtoSong(radioInfo),
                Url = url
            };
            return _client.PlayRadioAsync(request).ResponseAsync;
        }

        /// <summary>
        /// Play a video.
        /// </summary>
        /// <param name="video">The video url.</param>
        public void PlayVideo(string video)
        {
            try
            {
                _client.PlayVideo(new PlayVideoRequest { VideoUrl = video });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC PlayVideo error: {ex.Message}");
            }
        }

        /// <summary>
        /// PlayVideo async.
        /// </summary>
        public Task PlayVideoAsync(string video)
        {
            return _client.PlayVideoAsync(new PlayVideoRequest { VideoUrl = video }).ResponseAsync;
        }

        /// <summary>
        /// Seek in the video.
        /// </summary>
        /// <param name="position">The video position in seconds.</param>
        public void SeekVideo(double position)
        {
            try
            {
                _client.SeekVideo(new SeekVideoRequest { Position = position });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC SeekVideo error: {ex.Message}");
            }
        }

        /// <summary>
        /// SeekVideo async.
        /// </summary>
        public Task SeekVideoAsync(double position)
        {
            return _client.SeekVideoAsync(new SeekVideoRequest { Position = position }).ResponseAsync;
        }

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SendFile(Stream stream)
        {
            try
            {
                using (var mem = new MemoryStream())
                {
                    stream.CopyTo(mem);
                    _currentSong.File = mem.ToArray();
                }

                var request = new SendFileRequest
                {
                    FileData = Google.Protobuf.ByteString.CopyFrom(_currentSong.File),
                    Song = ConvertToProtoSong(_currentSong)
                };
                _client.SendFile(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC SendFile error: {ex.Message}");
            }
        }

        /// <summary>
        /// SendFile async.
        /// </summary>
        public Task SendFileAsync(Stream stream)
        {
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                _currentSong.File = mem.ToArray();
            }

            var request = new SendFileRequest
            {
                FileData = Google.Protobuf.ByteString.CopyFrom(_currentSong.File),
                Song = ConvertToProtoSong(_currentSong)
            };
            return _client.SendFileAsync(request).ResponseAsync;
        }

        /// <summary>
        /// Sets the song information.
        /// </summary>
        /// <param name="song">The song.</param>
        public void SetSong(SongInformation song)
        {
            _currentSong = song;
            try
            {
                var request = new SetSongRequest
                {
                    Song = ConvertToProtoSong(song)
                };
                _client.SetSong(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC SetSong error: {ex.Message}");
            }
        }

        /// <summary>
        /// SetSong async.
        /// </summary>
        public Task SetSongAsync(SongInformation song)
        {
            _currentSong = song;
            var request = new SetSongRequest
            {
                Song = ConvertToProtoSong(song)
            };
            return _client.SetSongAsync(request).ResponseAsync;
        }

        /// <summary>
        /// Sets the song position.
        /// </summary>
        /// <param name="position">The position in seconds.</param>
        public void SetSongPosition(double position)
        {
            try
            {
                _client.SetSongPosition(new SetSongPositionRequest { Position = position });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC SetSongPosition error: {ex.Message}");
            }
        }

        /// <summary>
        /// SetSongPosition async.
        /// </summary>
        public Task SetSongPositionAsync(double position)
        {
            return _client.SetSongPositionAsync(new SetSongPositionRequest { Position = position }).ResponseAsync;
        }

        /// <summary>
        /// Converts SongInformation model to proto message.
        /// </summary>
        private SongInformationProto ConvertToProtoSong(SongInformation song)
        {
            if (song == null) return null;
            return new SongInformationProto
            {
                Title = song.Title ?? "",
                Artist = song.Band ?? "",
                Album = song.Album ?? "",
                Location = song.Location ?? "",
                Duration = song.Duration,
                File = Google.Protobuf.ByteString.CopyFrom(song.File ?? Array.Empty<byte>())
            };
        }
    }
}
