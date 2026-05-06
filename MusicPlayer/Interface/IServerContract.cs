using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The server contract (client calling server).
    /// Supports both sync and async patterns for gRPC communication.
    /// </summary>
    public interface IServerContract
    {
        /// <summary>
        /// Anounce the client.
        /// </summary>
        void Anounce();

        /// <summary>
        /// Anounce the client (async version for gRPC).
        /// </summary>
        Task AnounceAsync();

        /// <summary>
        /// Anounce the departure of the client.
        /// </summary>
        void Goodbye();

        /// <summary>
        /// Anounce the departure of the client (async version for gRPC).
        /// </summary>
        Task GoodbyeAsync();

        /// <summary>
        /// Get the current song position.
        /// </summary>
        /// <returns>The position.</returns>
        double? GetCurrentPosition();

        /// <summary>
        /// Get the current song position (async version for gRPC).
        /// </summary>
        Task<double?> GetCurrentPositionAsync();
    }
}
