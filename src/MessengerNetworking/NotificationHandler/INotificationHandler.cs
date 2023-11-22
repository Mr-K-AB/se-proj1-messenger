using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.NotificationHandler
{
    /// <summary>
    /// Interface has functions for events such as data recieved, client joined, client left
    /// </summary>
     public interface INotificationHandler
    { 
        /// <summary>
        /// Function will be called when some module will get message
        /// </summary>
        /// <param name="serializedData"></param>
        public void OnDataReceived(string serializedData);

        /// <summary>
        /// Server will call function when new client connects
        /// </summary>
        /// <param name="socket"></param>
        public void OnClientJoined(string ipAddress, int port);

        /// <summary>
        /// Server will call function when a client disconnects
        /// </summary>
        /// <param name="clientId"></param>
        public void OnClientLeft(string ipAddress, int port);
    
    }
}
