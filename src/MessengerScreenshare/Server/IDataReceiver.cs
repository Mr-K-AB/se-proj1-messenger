using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerScreenshare.Server
{
    public interface IDataReceiver
    {
        /// <summary>
        /// Notifies that subscribers list has been changed.
        /// This will happen when a client either starts or stops screen sharing.
        /// </summary>
        /// <param name="subscribers">
        /// Updated list of the subscribers.
        /// </param>
        public void OnSubscribersUpdated(List<SharedClientScreen> subscribers);

        /// <summary>
        /// Notifies that a client has started screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who started screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who started screen sharing.
        /// </param>
        public void OnScreenshareStart(string clientId, string clientName);

        /// <summary>
        /// Notifies that a client has stopped screen sharing.
        /// </summary>
        /// <param name="clientId">
        /// Id of the client who stopped screen sharing.
        /// </param>
        /// <param name="clientName">
        /// Name of the client who stopped screen sharing.
        /// </param>
        public void OnScreenshareStop(string clientId, string clientName);
    }
}
