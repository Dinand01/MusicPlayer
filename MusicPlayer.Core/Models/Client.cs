using System;
using MusicPlayer.Core.Communication.StreamingMessages;

namespace MusicPlayer.Core.Models
{
    /// <summary>
    /// Describes a WCF client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the peer info.
        /// </summary>
        public string Peer { get; set; }

        /// <summary>
        /// Event to send a message to the client.
        /// </summary>
        public event Action<StreamingMessage> SendMessageEvent;

        public void SendMessage<T>(T message)
            where T : StreamingMessage
        {
            SendMessageEvent?.Invoke(message);
        }
    }
}
