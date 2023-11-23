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
        /// Notifies that subscribers list has been changed when a client either starts or stops screen sharing.
        /// </summary>
        public void OnSubscribersUpdated(List<SharedClientScreen> subscribers);

        /// <summary>
        /// Notifies that a client has started screen sharing.
        /// </summary>
        
        public void OnScreenshareStart(int clientId, string clientName);

        /// <summary>
        /// Notifies that a client has stopped screen sharing.
        /// </summary>
        public void OnScreenshareStop(int clientId, string clientName);
    }
}
