/******************************************************************************
* Filename    = ContentServer.cs
*
* Author      = Manikanta Gudipudi
*
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = file to obtain the files and chat messages on the server and pass them along after processing.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Messenger.Client;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerNetworking;
using MessengerNetworking.Communicator;
using MessengerNetworking.NotificationHandler;
using MessengerNetworking.Serializer;
//using MessengerNetworking.Factory;

namespace MessengerContent.Server
{
    public class ContentServer : IContentServer
    {
        private static readonly object s_lock = new();
        private readonly INotificationHandler _notificationHandler;
        private ChatServer _chatServer;
        private ContentDataBase _contentDatabase;
        private FileServer _fileServer;
        private IContentSerializer _serializer;
        private List<IMessageListener> _subscribers;

        public ContentServer()
        {
            _subscribers = new List<IMessageListener>();
            //_communicator = Factory.GetCommunicator(false);
            _contentDatabase = new ContentDataBase();
            _notificationHandler = new ContentServerNotificationHandler(this);
            _fileServer = new FileServer(_contentDatabase);
            _chatServer = new ChatServer(_contentDatabase);
            _serializer = new ContentSerializer();
            //_communicator.Subscribe("Content", _notificationHandler);
        }

        /// <summary>
        /// Get and Set Communicator, util for testing the code
        /// </summary>
        public ICommunicator Communicator { get; set; }

        /// <inheritdoc />
        public void ServerSubscribe(IMessageListener subscriber)
        {
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public List<ChatThread> GetAllMessages()
        {
            lock (s_lock)
            {
                return _chatServer.GetMessages();
            }
        }

        /// <inheritdoc />
        public void SendAllMessagesToClient(int userId)
        {
            string allMessagesSerialized = _serializer.Serialize(GetAllMessages());
            Communicator.Send(allMessagesSerialized, "Content", userId.ToString());
        }

        /// <summary>
        /// Receives data from ContentServerNotificationHandler and processes it
        /// </summary>
        /// <param name="data"></param>
        public void Receive(string data)
        {
            ChatData messageData;
            // Try deserializing the data if error then do nothing and return.
            try
            {
                messageData = _serializer.Deserialize<ChatData>(data);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Exception occured while deserialsing data, Exception: {e}");
                return;
            }

            ChatData receivedMessageData;
            Trace.WriteLine("[ContentServer] Received MessageData from ContentServerNotificationHandler");

            // lock to prevent multiple threads from modifying the messages at once.
            lock (s_lock)
            {
                switch (messageData.Type)
                {
                    case MessageType.Chat:
                        Trace.WriteLine("[ContentServer] MessageType is Chat, Calling ChatServer.Receive()");
                        receivedMessageData = _chatServer.Receive(messageData);
                        break;

                    case MessageType.File:
                        Trace.WriteLine("[ContentServer] MessageType is File, Calling FileServer.Receive()");
                        receivedMessageData = _fileServer.Receive(messageData);
                        break;

                    case MessageType.HistoryRequest:
                        Trace.WriteLine("[ContentServer] MessageType is HistoryRequest, Calling ContentServer.SendAllMessagesToClient");
                        SendAllMessagesToClient(messageData.SenderID);
                        return;

                    default:
                        Trace.WriteLine("[ContentServer] MessageType is Unknown");
                        return;
                }
            }

            // If this is null then something went wrong, probably message was not found.
            if (receivedMessageData == null)
            {
                Trace.WriteLine("[ContentServer] Null Message received");
                return;
            }

            try
            {
                // If Event is Download then send the file to client
                if (messageData.Event == MessageEvent.Download)
                {
                    Trace.WriteLine("[ContentServer] MesseageEvent is Download, Sending File to client");
                    SendFile(receivedMessageData);
                }
                // Else send the message to all the receivers and notify the subscribers
                else
                {
                    Trace.WriteLine("[ContentServer] Message received, Notifying subscribers");
                    Notify(receivedMessageData);
                    Trace.WriteLine("[ContentServer] Sending message to clients");
                    Send(receivedMessageData);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentServer] Something went wrong while sending message. Exception {e}");
                return;
            }
            Trace.WriteLine("[ContentServer] Message sent successfully");
        }

        /// <summary>
        /// Sends the message to clients.
        /// </summary>
        /// <param name="messageData"></param>
        public void Send(ChatData messageData)
        {
            string message = _serializer.Serialize(messageData);

            // If no ReceiverIds that means its a broadcast.
            if (messageData.ReceiverIDs.Length == 0)
            {
                Communicator.Send(message, "Content", null);
            }
            // Else send the message to the receivers in ReceiversIds.
            else
            {
                // Send the message to receivers and back to the sender.
                List<string> targetUserIds = new() { messageData.SenderID.ToString() };
                targetUserIds.AddRange(messageData.ReceiverIDs.Select(userId => userId.ToString()));

                foreach (string userId in targetUserIds)
                {
                    Communicator.Send(message, "Content", userId);
                }
            }
        }

        /// <summary>
        /// Sends the file back to the requester.
        /// </summary>
        /// <param name="messageData"></param>
        public void SendFile(ChatData messageData)
        {
            string message = _serializer.Serialize(messageData);
            Communicator.Send(message, "Content", messageData.SenderID.ToString());
        }

        /// <summary>
        /// Notifies all the subscribed modules.
        /// </summary>
        /// <param name="receiveMessageData"></param>
        public void Notify(ReceiveChatData receiveMessageData)
        {
            foreach (IMessageListener subscriber in _subscribers)
            {
                _ = Task.Run(() => { subscriber.OnMessageReceived(receiveMessageData); });
            }
        }

        /// <summary>
        /// Resets the ContentServer, for Testing purpose
        /// </summary>
        public void Reset()
        {
            _subscribers = new List<IMessageListener>();
            _contentDatabase = new ContentDataBase();
            _fileServer = new FileServer(_contentDatabase);
            _chatServer = new ChatServer(_contentDatabase);
            _serializer = new ContentSerializer();
        }
    }
}
