/******************************************************************************
 * Filename    = ContentClientNotificationHandler.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = 
 *****************************************************************************/
using MessengerContent.DataModels;
using MessengerNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MessengerNetworking.NotificationHandler;
using System.Net.Sockets;

namespace MessengerContent.Client
{
    public class ContentClientNotificationHandler : INotificationHandler
    {
        private readonly IContentSerializer _serialzer;
        private readonly ContentClient _messageHandler;
        protected ChatData _receivedMessage;
        protected List<ChatThread> _allMessages;

        public ContentClientNotificationHandler(IContentClient contentHandler)
        {
            _serialzer = new ContentSerializer();
            _messageHandler = contentHandler as ContentClient;
        }

        public void OnClientJoined(TcpClient socket)
        {
        }
        public void OnClientLeft(string clientId)
        {
        }

        public void OnDataReceived(string data)
        {
            Trace.WriteLine("[ContentClientNotificationHandler] Deserializing data received from network");
            try
            {
                // get type of serialized data
                string deserializedType = _serialzer.GetObjType(data, "Content");
                // if data is of ChatData type
                if (string.Equals(deserializedType, typeof(ChatData).ToString()))
                {
                    _receivedMessage = _serialzer.Deserialize<ChatData>(data);
                    _messageHandler.OnReceive(_receivedMessage);
                    Trace.WriteLine($"[ContentClientNotificationHandler] Deserialized data and sending it to content client");
                }
                else
                {
                    throw new ArgumentException($"Deserialized object of unknown type : {deserializedType}");
                }
            }
            catch (ArgumentException e)
            {
                throw;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentClientNotificationHandler] Error during deserialization of received data.\n{e.GetType().Name} : {e.Message}");
            }
        }

    }
}
