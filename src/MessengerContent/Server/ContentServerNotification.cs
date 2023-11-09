﻿/******************************************************************************
* Filename    = ContentServerNotificationHandler.cs
* 
* Author      = Manikanta Gudipudi
* 
* Product     = Messenger
* 
* Project     = MessengerContent
* 
* Description = file to handle the notifications from Network Module.
*****************************************************************************/

using System.Net.Sockets;
using MessengerNetworking;
using MessengerNetworking.NotificationHandler;

namespace MessengerContent.Server
{
    public class ContentServerNotificationHandler : INotificationHandler
    {
        public readonly ContentServer ContentServer;
        /// <inheritdoc />
        public ContentServerNotificationHandler(ContentServer contentServer)
        {
            ContentServer = contentServer;
        }
        /// <inheritdoc />
        public void OnClientJoined(TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void OnClientJoined(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string ipAddress, int port)
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
