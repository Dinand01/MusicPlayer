using MusicPlayer.Interface;
using MusicPlayer.Models;
using Grpc.Core;
using System.Threading.Tasks;
using MusicPlayer.Protos;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// gRPC server implementation replacing WCFServerService.
    /// Inherits from generated MusicPlayerServerServiceBase.
    /// </summary>
    public class GrpcServerService : Protos.MusicPlayerServerService.MusicPlayerServerServiceBase
    {
        /// <summary>
        /// Event that is called when a client connects.
        /// </summary>
        public static event Action<Client> ClientConnected;

        /// <summary>
        /// A client disconnected.
        /// </summary>
        public static event Action<string, int> ClientDisconnected;

        /// <summary>
        /// Gets or sets The music player.
        /// </summary>
        internal static IMusicPlayer Player { get; set; }

        /// <summary>
        /// Anounce a new connection.
        /// </summary>
        public override Task<AnounceResponse> Anounce(AnounceRequest request, ServerCallContext context)
        {
            // Invoke client connected event
            ClientConnected?.Invoke(new Client
            {
                IpAddress = request.IpAddress,
                Port = request.Port,
                ClientContract = null // gRPC doesn't use callback contracts
            });

            return Task.FromResult(new AnounceResponse
            {
                Success = true,
                Message = "Connected"
            });
        }

        /// <summary>
        /// Notify the server of a disconnect.
        /// </summary>
        public override Task<GoodbyeResponse> Goodbye(GoodbyeRequest request, ServerCallContext context)
        {
            ClientDisconnected?.Invoke(request.IpAddress, request.Port);

            return Task.FromResult(new GoodbyeResponse
            {
                Success = true
            });
        }

        /// <summary>
        /// Get the current song position.
        /// </summary>
        public override Task<GetCurrentPositionResponse> GetCurrentPosition(GetCurrentPositionRequest request, ServerCallContext context)
        {
            double position = Player?.GetSongPosition() ?? 0.0;

            return Task.FromResult(new GetCurrentPositionResponse
            {
                Position = position
            });
        }
    }
}
