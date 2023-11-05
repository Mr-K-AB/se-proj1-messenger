using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using MessengerNetworking.Communicator;

namespace MessengerNetworking.Factory
{
    /// <summary>
    /// Gives a unique instance of server to each system
    /// </summary>
    public static class Factory
    {
        // public IPAddress _serverId {  get; private set; }

        // private static object _instanceLock = new object();

        public static UdpCommunicator s_communicator = new(); 

        
        // private Factory() { }

        // private static Factory _instance;

        /// <summary>
        /// public method which can be accessed by other modules
        /// </summary>
        /// <param name="_serverId"> unique for each system like IP </param>
        /// <returns> instance of server </returns>
        ///     
        
        public static ICommunicator GetInstance(IPAddress _serverId)
        {
            
            return s_communicator;
        }
    }
}
