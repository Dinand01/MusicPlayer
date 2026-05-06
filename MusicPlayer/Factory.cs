using MusicPlayer.Controller;
using MusicPlayer.Interface;
using MusicPlayer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Grpc.AspNetCore;
using Grpc.Core;
using MusicPlayer.Protos;

namespace MusicPlayer
{
    /// <summary>
    /// The factory for music players.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Gets a standard music player.
        /// </summary>
        /// <returns>The music player.</returns>
        public static IMusicPlayer GetPlayer()
        {
            return new Controller.MusicPlayer();
        }

        /// <summary>
        /// Gets a standard music player for receive mode.
        /// </summary>
        /// <returns>The music player.</returns>
        internal static IMusicPlayer GetPlayerForReceiveMode()
        {
            return new Controller.MusicPlayer(true);
        }

        /// <summary>
        /// Gets a player with server capabilities.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        /// <param name="player">The optional current music player.</param>
        /// <returns>The music player with server capabilities.</returns>
        public static IMusicPlayer GetServerPlayer(int port, IMusicPlayer player = null)
        {
            if (player == null)
            {
                player = new Controller.MusicPlayer();
            }

            // Store reference to player in GrpcServerService
            GrpcServerService.Player = player as Controller.MusicPlayer;

            // Start gRPC server in background
            Task.Run(() =>
            {
                try
                {
                    var server = new Server
                    {
                        Services = { MusicPlayerServerService.BindService(new GrpcServerService()) },
                        Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
                    };
                    server.Start();
                    Console.WriteLine($"gRPC Server started on port {port}");
                    // Keep server running
                    server.ShutdownTask.Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"gRPC Server error: {ex.Message}");
                }
            });

            return player;
        }

        /// <summary>
        /// Gets a player with client capabilities.
        /// </summary>
        /// <param name="ip">The ip address.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="player">The optional current music player.</param>
        /// <returns>The music player with server capabilities.</returns>
        public static IMusicPlayer GetClientPlayer(IPAddress ip, int port, IMusicPlayer player = null)
        {
            player?.Dispose();
            
            // Create gRPC client contract
            var serverUrl = $"http://{ip}:{port}";
            var clientContract = new GrpcClientContract(serverUrl);
            
            var musicPlayer = GetPlayerForReceiveMode();
            // TODO: Attach clientContract to musicPlayer for server callbacks
            // This requires bidirectional streaming setup
            
            return musicPlayer;
        }

        /// <summary>
        /// Gets the video player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The video player.</returns>
        public static IMusicPlayer GetVideoPlayer(IMusicPlayer player = null)
        {
            // TODO: Fix YoutubeExplode API integration
            // return player is IVideo p ? p : new VideoController(player);
            return player;
        }

        /// <summary>
        /// Gets a copy controller.
        /// </summary>
        /// <returns>The copy controller.</returns>
        public static ICopy GetCopy()
        {
            return new CopyController();
        }

        /// <summary>
        /// Gets the controller for handling radio station information.
        /// </summary>
        /// <returns>The controller.</returns>
        public static IRadioStation GetRadioInfo()
        {
            return new RadioStationController();
        }
    }
}
