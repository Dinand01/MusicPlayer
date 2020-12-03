using Microsoft.Extensions.DependencyInjection;

namespace MusicPlayer.Communication.Grpc
{
    public static class DiExtensions
    {
        public static void AddGrpc(this IServiceCollection services)
        {
            services.AddHostedService<GrpcServerFactory>();
            services.AddTransient<IGrpcServerFactory>(sp => sp.GetRequiredService<GrpcServerFactory>());
        }
    }
}
