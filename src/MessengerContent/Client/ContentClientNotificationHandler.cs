/******************************************************************************
 * Filename    = ContentClientNotificationHandler.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Notification handles for all types of messages
 *****************************************************************************/
using MessengerContent.DataModels;
using MessengerNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MessengerNetworking.NotificationHandler;
using System.Net.Sockets;
using TraceLogger;

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
            Logger.Log("[ContentClientNotificationHandler] Deserializing data received from network", LogLevel.INFO);
            try
            {
                // get type of serialized data
                string deserializedType = _serialzer.GetObjType(data, "Content");
                // if data is of ChatData type
                if (string.Equals(deserializedType, typeof(ChatData).ToString()))
                {
                    _receivedMessage = _serialzer.Deserialize<ChatData>(data);
                    _messageHandler.OnReceive(_receivedMessage);
                    Logger.Log($"[ContentClientNotificationHandler] Deserialized data and sending it to content client", LogLevel.INFO);
                }
                // if data is a List<ChatThread>
                else if (string.Equals(deserializedType, typeof(List<ChatThread>).ToString()))
                {
                    _allMessages = _serialzer.Deserialize<List<ChatThread>>(data);
                    _messageHandler.OnReceive(_allMessages);
                    Logger.Log($"[ContentClientNotificationHandler] Deserialized data and sending it to content client", LogLevel.INFO);
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
                Logger.Log($"[ContentClientNotificationHandler] Error during deserialization of received data.\n{e.GetType().Name} : {e.Message}", LogLevel.WARNING);
            }
        }

    }
}
