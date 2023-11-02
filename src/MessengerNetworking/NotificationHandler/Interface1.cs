using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessengerNetworking.NotificationHandler
{
     public interface INotificationHandler
    { 
        public void OnDataReceived(string serializedData);
        public void OnClientJoined(TcpClient socket);


        public void OnClientLeft(string clientId);
    
    }
}
