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

        private static readonly object s_instanceLock = new();

        private Factory() { }

        private static Factory? s_instance;

        /// <summary>
        /// public method which can be accessed by other modules
        /// </summary>
        /// <param name="_serverId"> unique for each system like IP </param>
        /// <returns> instance of server </returns>
        
        public static Factory GetInstance(IPAddress _serverId)
        {
            if (s_instance == null)
            {
                lock (s_instanceLock)
                {
                    s_instance ??= new Factory
                        {
                            _serverId = _serverId
                        };
                }
            }
            return s_instance;
        }
    }
}
