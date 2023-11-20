/******************************************************************************
 * Filename    = ChatMessageClient.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Class handling the sending of data to server based on 
 *               the type of chat events - New, Edit, Delete, Star. 
 *****************************************************************************/
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
        private readonly string _moduleIdentifier = "ContentServer";
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

        public string UserName { get; set; }

        // helper functions

        /// <summary>
        /// Converts 'SendChatData' object to 'ChatData' object. 
        /// </summary>
        /// <param name="sendChatData">Instance of the SendChatData class</param>
        /// <param name="eventType">Type of message event</param>
        /// <returns>Instance of ChatData class</returns>
        public ChatData ChatDataFromSendData(SendChatData sendChatData, MessageEvent eventType)
        {
            ChatData convertedData = new()
            {
                Type = sendChatData.Type,
                Data = sendChatData.Data,
                //ReceiverIDs = sendChatData.ReceiverIDs,
                ReplyMessageID = sendChatData.ReplyMessageID,
                ReplyThreadID = sendChatData.ReplyThreadID,
                SenderID = UserID,
                SenderName = UserName,
                SentTime = DateTime.Now,
                Starred = false,
                Event = eventType
            };
            Trace.WriteLine("[ChatClient] Converting 'SendChatData' to 'ChatData' object");
            return convertedData;
        }

        /// <summary>
        /// Serializes the ChatData object and sends it to the server via networking module. 
        /// </summary>
        /// <param name="chatData">Instance of SendChatData class</param>
        /// <param name="eventType">Type of message event as string</param>
        private void SerializeAndSendToServer(ChatData chatData, string eventType, string ip, int port)
        {
            try
            {
                string serializedStr = _serializer.Serialize(chatData);
                Trace.WriteLine($"[Chat Client] Setting event as '{eventType}' and sending object to server.");
                Debug.Assert(1 == 1, "debugg");
                _communicator.SendMessage(ip, port, _moduleIdentifier, serializedStr);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[Chat Client] Exception occurred while sending object.\n{e.GetType().Name} : {e.Message}");
            }
        }

        // chat client functions

        /// <summary>
        /// Converts the input SendChatData object, sets the event type as New, serializes and sends it to server.
        /// </summary>
        /// <param name="sendContent">Instance of the SendChatData class</param>
        /// <exception cref="ArgumentException"></exception>
        public void NewChat(SendChatData sendChat, string ip, int port)
        {
            if (string.IsNullOrEmpty(sendChat.Data))
            {
                throw new ArgumentException("Invalid message string.");
            }
            ChatData convertedData = ChatDataFromSendData(sendChat, MessageEvent.New);
            convertedData.MessageID = -1;
            SerializeAndSendToServer(convertedData, "New", ip, port);
        }

        /// <summary>
        /// Creates ChatData object, sets the event type as Edit, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="newMessage">Edited message string</param>
        /// <param name="replyThreadID">ID of thread to which the message belongs to</param>
        /// <exception cref="ArgumentException"></exception>
        public void EditChat(int messageID, string newMessage, int replyThreadID, string ip, int port)
        {
            if (string.IsNullOrEmpty(newMessage))
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
                SenderName = UserName,
                Event = MessageEvent.Edit
            };
            SerializeAndSendToServer(sendData, "Edit", ip, port);
        }

        /// <summary>
        /// Creates ChatData object, sets the event type as Delete, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="replyThreadID">ID of thread to which the message belongs to</param>
        public void DeleteChat(int messageID, int replyThreadID, string ip, int port)
        {
            ChatData sendData = new()
            {
                Type = MessageType.Chat,
                Data = "Message Deleted.",
                MessageID = messageID,
                ReplyThreadID = replyThreadID,
                SenderID = UserID,
                SenderName = UserName,
                Event = MessageEvent.Delete
            };
            SerializeAndSendToServer(sendData, "Delete", ip, port);
        }

        /// <summary>
        /// Creates ChatData object, sets the event type as Star, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="replyThreadID">ID of thread to which the message belongs to</param>
        public void StarChat(int messageID, int replyThreadID, string ip, int port)
        {
            ChatData sendData = new()
            {
                Type = MessageType.Chat,
                MessageID = messageID,
                ReplyThreadID = replyThreadID,
                SenderID = UserID,
                SenderName = UserName,
                Event = MessageEvent.Star
            };
            SerializeAndSendToServer(sendData, "Star", ip, port);
        }

    }
}
