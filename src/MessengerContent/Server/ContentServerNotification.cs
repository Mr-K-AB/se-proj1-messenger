/******************************************************************************
* Filename    = ContentServerNotificationHandler.cs
* Author      = 
* Product     = Messenger
* Project     = MessengerContent
* Description = 
*****************************************************************************/

using System.Net.Sockets;
using MessengerNetworking;
using MessengerNetworking.NotificationHandler;

namespace MessengerContent.Server
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        public readonly ContentServer ContentServer;
        public ContentServerNotificationHandler(ContentServer contentServer)
        {
            ContentServer = contentServer;
        }

        public void OnClientJoined(TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnDataReceived(string data)
        {
            ContentServer.Receive(data);
        }
    }
}
