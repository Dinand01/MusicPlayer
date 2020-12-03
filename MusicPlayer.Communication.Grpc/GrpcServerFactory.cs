using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.Communication.Grpc
{
    internal class GrpcServerFactory : IGrpcServerFactory, IHostedService
    {
        private IWebHost _server;
        private Task _serverService;
        private CancellationTokenSource _cancellationTokenSource;

        public GrpcServerFactory()
        {
        }

        public void CreateGrpcServer(int port)
        {
            if (_cancellationTokenSource != default && !_cancellationTokenSource.IsCancellationRequested)
                throw new ArgumentException("A server already exists");

            _cancellationTokenSource = new CancellationTokenSource();
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {

                })
                .ConfigureKestrel(ko =>
                {
                    ko.ListenAnyIP(port);
                });

            _server = builder.Build();
            _serverService = _server.RunAsync(_cancellationTokenSource.Token);
        }

        public async Task DestroyGrpcServer()
        {
            _cancellationTokenSource?.Cancel();
            await _serverService;
            _server?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await DestroyGrpcServer();
        }
    }
}
