using System.Threading.Tasks;

namespace MusicPlayer.Communication
{
    public interface IMusicPlayerClient
    {

        /// <summary>
        /// Anounce the client.
        /// </summary>
        Task Anounce();

        /// <summary>
        /// Anounce the departure of the client.
        /// </summary>
        Task Goodbye();

        /// <summary>
        /// Gest the current song position.
        /// </summary>
        /// <returns>The position.</returns>
        Task<double?> GetCurrentPosition();
    }
}
