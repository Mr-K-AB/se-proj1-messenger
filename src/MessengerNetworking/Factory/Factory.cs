using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;

namespace Factory
{
    /// <summary>
    /// Gives a unique instance of server to each system
    /// </summary>
    public class Factory
    {
        public IPAddress _serverId {  get; private set; }

        private static object _instanceLock = new object();

        private Factory() { }

        private static Factory _instance;

        /// <summary>
        /// public method which can be accessed by other modules
        /// </summary>
        /// <param name="_serverId"> unique for each system like IP </param>
        /// <returns> instance of server </returns>
        
        public static Factory GetInstance(IPAddress _serverId)
        {
            if (_instance == null)
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new Factory();
                        _instance._serverId = _serverId;
                    }
                }
            }
            return _instance;
        }
    }
}