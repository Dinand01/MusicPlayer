using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// The Message that can be send over the network.
    /// </summary>
    [Serializable]
    internal class Message
    {
        /// <summary>
        /// The ID or index of the data.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets The message type.
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// The Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Song meta data.
        /// </summary>
        public Song Song { get; set; }

        /// <summary>
        /// Song duration.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Total data length
        /// </summary>
        public int DataLength { get; set; }

        /// <summary>
        /// The data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
