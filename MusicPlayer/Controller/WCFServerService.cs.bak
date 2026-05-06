using MusicPlayer.Interface;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The wcf service.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    internal class WCFServerService : IServerContract
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
        /// The client contract.
        /// </summary>
        private IClientContract _client;

        /// <summary>
        /// Initializes a new WCF service session.
        /// </summary>
        public WCFServerService()
        {
            RemoteEndpointMessageProperty endpoint = GetRemoteEndpoint();
            _client = OperationContext.Current.GetCallbackChannel<IClientContract>();
            ClientConnected?.Invoke(new Client
            {
                IpAddress = endpoint?.Address,
                Port = endpoint?.Port ?? 0,
                ClientContract = _client
            });
        }

        /// <summary>
        /// Anounce a new connection.
        /// </summary>
        public void Anounce()
        {
            // Nothing is required, the constructor takes care of this.
        }

        /// <summary>
        /// Notify the server of a disconnect.
        /// </summary>
        public void Goodbye()
        {
            RemoteEndpointMessageProperty endpoint = GetRemoteEndpoint();
            ClientDisconnected?.Invoke(endpoint?.Address, endpoint?.Port ?? 0);
        }

        /// <summary>
        /// Gest the current song position.
        /// </summary>
        /// <returns>The song position.</returns>
        public double? GetCurrentPosition()
        {
            return Player.GetSongPosition();
        }

        /// <summary>
        /// Gets the remote endpoint.
        /// </summary>
        /// <returns>The remote endpoint.</returns>
        private RemoteEndpointMessageProperty GetRemoteEndpoint()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context?.IncomingMessageProperties;
            return prop?[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
        }
    }
}
