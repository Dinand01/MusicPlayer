
using System.Threading.Tasks;

namespace MusicPlayer.Communication.Grpc
{
    public interface IGrpcServerFactory
    {
        void CreateGrpcServer(int port);

        Task DestroyGrpcServer();
    }
}
