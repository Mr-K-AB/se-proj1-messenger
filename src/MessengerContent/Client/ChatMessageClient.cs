using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerNetworking.Communicator;
using System.Runtime.CompilerServices;

namespace MessengerContent.Client
{
    public class ChatMessageClient
    { 
        /// <summary>
        /// Module identifier for communicator
        /// </summary>
        private readonly string _moduleIdentifier = "Content";
        private readonly IContentSerializer _serializer;
        private ICommunicator _communicator;

        /// <summary>
        /// Constructor that instantiates a communicator and serializer.
        /// </summary>
        /// <param name="communicator">Object implementing the ICommunicator interface</param>
        public ChatMessageClient(ICommunicator communicator)
        {
            _communicator = communicator;
            _serializer = new ContentSerializer();
        }

        /// <summary>
        /// Communicator setter function
        /// </summary>
        public ICommunicator Communicator
        {
            set => _communicator = value;
        }

        /// <summary>
        /// Auto-implemented UserID property.
        /// </summary>
        public int UserID { get; set; }

        // helper functions

        /// <summary>
        /// Check if message is empty
        /// </summary>
        /// <param name="message">Message string</param>
        /// <returns>True if empty, false otherwise</returns>
        private bool IsEmptyMessage(string message)
        {
            if (message == null || message == "")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts 'SendContentData' object to 'ContentData' object. 
        /// </summary>
        /// <param name="sendContentData">Instance of the SendContentData class</param>
        /// <param name="eventType">Type of message event</param>
        /// <returns>Instance of ContentData class</returns>
        public ChatData ConvertSendContentData(SendChatData sendContentData, MessageEvent eventType)
        {
            ChatData convertedData = new()
            {
                Type = sendContentData.Type,
                Data = sendContentData.Data,
                ReceiverIDs = sendContentData.ReceiverIDs,
                ReplyMessageID = sendContentData.ReplyMessageID,
                ReplyThreadID = sendContentData.ReplyThreadID,
                SenderID = UserID,
                SentTime = DateTime.Now,
                Starred = false,
                Event = eventType
            };
            Trace.WriteLine("[ChatClient] Converting 'SendContentData' to 'ContentData' object");
            return convertedData;
        }

        /// <summary>
        /// Serializes the ContentData object and sends it to the server via networking module. 
        /// </summary>
        /// <param name="contentData">Instance of SendContentData class</param>
        /// <param name="eventType">Type of message event as string</param>
        private void SerializeAndSendToServer(ChatData contentData, string eventType)
        {
            try
            {
                string serializedStr = _serializer.Serialize(contentData);
                Trace.WriteLine($"[Chat Client] Setting event as '{eventType}' and sending object to server.");
                _communicator.Send(serializedStr, _moduleIdentifier, null);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[Chat Client] Exception occurred while sending object.\n{e.GetType().Name} : {e.Message}");
            }
        }

        // chat client functions

        /// <summary>
        /// Converts the input SendContentData object, sets the event type as New, serializes and sends it to server.
        /// </summary>
        /// <param name="sendContent">Instance of the SendContentData class</param>
        /// <exception cref="ArgumentException"></exception>
        public void NewChat(SendChatData sendContent)
        {
            if (IsEmptyMessage(sendContent.Data))
            {
                throw new ArgumentException("Invalid message string.");
            }
            ChatData convertedData = ConvertSendContentData(sendContent, MessageEvent.New);
            convertedData.MessageID = -1;
            SerializeAndSendToServer(convertedData, "New");
        }

        /// <summary>
        /// Creates ContentData object, sets the event type as Edit, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="newMessage">Edited message string</param>
        /// <param name="replyThreadID">ID of thread to which the message belongs to</param>
        /// <exception cref="ArgumentException"></exception>
        public void EditChat(int messageID, string newMessage, int replyThreadID)
        {
            if (IsEmptyMessage(newMessage))
            {
                throw new ArgumentException("Invalid message string.");
            }
            ChatData sendData = new()
            {
                Type = MessageType.Chat,
                Data = newMessage,
                MessageID = messageID,
                ReplyThreadID = replyThreadID,
                SenderID = UserID,
                Event = MessageEvent.Edit
            };
            SerializeAndSendToServer(sendData, "Edit");
        }

        /// <summary>
        /// Creates ContentData object, sets the event type as Delete, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="replyThreadID">ID of thread to which the message belongs to</param>
        public void DeleteChat(int messageID, int replyThreadID)
        {
            ChatData sendData = new()
            {
                Type = MessageType.Chat,
                Data = "Message Deleted.",
                MessageID = messageID,
                ReplyThreadID = replyThreadID,
                SenderID = UserID,
                Event = MessageEvent.Delete
            };
            SerializeAndSendToServer(sendData, "Delete");
        }

        /// <summary>
        /// Creates ContentData object, sets the event type as Star, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="replyThreadID">ID of thread to which the message belongs to</param>
        public void StarChat(int messageID, int replyThreadID)
        {
            ChatData sendData = new()
            {
                Type = MessageType.Chat,
                MessageID = messageID,
                ReplyThreadID = replyThreadID,
                SenderID = UserID,
                Event = MessageEvent.Star
            };
            SerializeAndSendToServer(sendData, "Star");
        }

    }
}
