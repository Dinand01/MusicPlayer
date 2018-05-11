using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The WCF contract.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IClientContract), Name = "MusicPlayer.Network")]
    public interface IServerContract
    {
        /// <summary>
        /// Anounce the client.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Anounce();

        /// <summary>
        /// Anounce the departure of the client.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Goodbye();

        /// <summary>
        /// Gest the current song position.
        /// </summary>
        /// <returns>The position.</returns>
        [OperationContract]
        double? GetCurrentPosition();
    }
}
