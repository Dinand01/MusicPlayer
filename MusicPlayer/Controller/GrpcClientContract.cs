using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MusicPlayer.Protos;
using Google.Protobuf;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// gRPC implementation for the client contract.
    /// Acts as a client to call the server's gRPC service (MusicPlayerServerService).
    /// </summary>
    internal class GrpcClientContract : IClientContract
    {
        private readonly Protos.MusicPlayerServerService.MusicPlayerServerServiceClient _client;
        private readonly GrpcChannel _channel;
        private SongInformation _currentSong;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrpcClientContract"/> class.
        /// </summary>
        /// <param name="serverUrl">The gRPC server URL.</param>
        public GrpcClientContract(string serverUrl)
        {
            _channel = GrpcChannel.ForAddress(serverUrl);
            _client = new Protos.MusicPlayerServerService.MusicPlayerServerServiceClient(_channel);
        }

        /// <summary>
        /// The server will disconnect.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _client.Goodbye(new GoodbyeRequest());
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
            return _client.GoodbyeAsync(new GoodbyeRequest()).ResponseAsync;
        }

        /// <summary>
        /// Pause the music or video.
        /// </summary>
        public void Pause()
        {
            // Note: Pause is a server-side method in WCF, but in gRPC it's called via MusicPlayerClientService
            // This is a placeholder - actual implementation depends on bidirectional streaming setup
            Console.WriteLine("Pause called - requires server→client streaming setup");
        }

        /// <summary>
        /// Pause async.
        /// </summary>
        public Task PauseAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Play the music or video.
        /// </summary>
        public void Play()
        {
            Console.WriteLine("Play called - requires server→client streaming setup");
        }

        /// <summary>
        /// Play async.
        /// </summary>
        public Task PlayAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Play from an online location.
        /// </summary>
        public void PlayRadio(SongInformation radioInfo, string url)
        {
            Console.WriteLine("PlayRadio called - requires server→client streaming setup");
        }

        /// <summary>
        /// PlayRadio async.
        /// </summary>
        public Task PlayRadioAsync(SongInformation radioInfo, string url)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Play a video.
        /// </summary>
        public void PlayVideo(string video)
        {
            Console.WriteLine("PlayVideo called - requires server→client streaming setup");
        }

        /// <summary>
        /// PlayVideo async.
        /// </summary>
        public Task PlayVideoAsync(string video)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Seek in the video.
        /// </summary>
        public void SeekVideo(double position)
        {
            Console.WriteLine("SeekVideo called - requires server→client streaming setup");
        }

        /// <summary>
        /// SeekVideo async.
        /// </summary>
        public Task SeekVideoAsync(double position)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the file information.
        /// </summary>
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
                    FileData = ByteString.CopyFrom(_currentSong.File),
                    Song = ConvertToProtoSong(_currentSong)
                };
                _client.Anounce(new AnounceRequest { ClientId = "client", IpAddress = "127.0.0.1", Port = 5000 });
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
                FileData = ByteString.CopyFrom(_currentSong.File),
                Song = ConvertToProtoSong(_currentSong)
            };
            return _client.AnounceAsync(new AnounceRequest { ClientId = "client", IpAddress = "127.0.0.1", Port = 5000 }).ResponseAsync;
        }

        /// <summary>
        /// Sets the song information.
        /// </summary>
        public void SetSong(SongInformation song)
        {
            _currentSong = song;
            try
            {
                var request = new SetSongRequest
                {
                    Song = ConvertToProtoSong(song)
                };
                _client.Anounce(new AnounceRequest { ClientId = "client", IpAddress = "127.0.0.1", Port = 5000 });
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
            return _client.AnounceAsync(new AnounceRequest { ClientId = "client", IpAddress = "127.0.0.1", Port = 5000 }).ResponseAsync;
        }

        /// <summary>
        /// Sets the song position.
        /// </summary>
        public void SetSongPosition(double position)
        {
            try
            {
                _client.GetCurrentPosition(new GetCurrentPositionRequest { ClientId = "client" });
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
            return _client.GetCurrentPositionAsync(new GetCurrentPositionRequest { ClientId = "client" }).ResponseAsync;
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
                File = ByteString.CopyFrom(song.File ?? Array.Empty<byte>())
            };
        }
    }
}
