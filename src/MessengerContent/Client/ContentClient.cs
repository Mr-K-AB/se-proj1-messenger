﻿/******************************************************************************
 * Filename    = ContentClient.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = 
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerNetworking.NotificationHandler;
using MessengerNetworking.Communicator;
using System.Security.Cryptography;
using MessengerNetworking.Factory;
using System.Security.Cryptography.X509Certificates;

namespace MessengerContent.Client
{
    public class ContentClient : IContentClient
    {
        /// <summary>
        /// Network related parameters
        /// </summary>
        private readonly INotificationHandler _notificationHandler;
        private ICommunicator _communicator;
        //private readonly IContentSerializer _serializer;

        /// <summary>
        /// List of subscribers
        /// </summary>
        private List<IMessageListener> _subscribers;

        /// <summary>
        /// Chat and File handlers
        /// </summary>
        private readonly ChatMessageClient _chatHandler;
        private readonly FileClient _fileHandler;

        /// <summary>
        /// ID of the user
        /// </summary>
        private int _userID;

        // Name and Id of the current client user
        private string? _name;

        private string _serverIP;
        private int _serverPort;

        /// <summary>
        /// Lock object for locking
        /// </summary>
        private readonly object _lock;

        /// <summary>
        /// List of threads containing all messages
        /// </summary>
        public List<ChatThread> AllMessages { get; private set; }

        /// <summary>
        /// Dictionary containing mapping from message ID to thread ID
        /// </summary>
        private Dictionary<int, int> _messageIDMap;

        /// <summary>
        /// Dictionary containing mapping from thread ID to index of thread ID 
        /// </summary>
        private Dictionary<int, int> _threadIDMap;

        /// <summary>
        /// Dictionary containing mapping from type of message event
        /// to the action associtated with the message event
        /// </summary>
        private readonly Dictionary<MessageEvent, Action<ChatData>> _messageEventHandler;

        /// <summary>
        /// Constructor that instantiates all requried parameters
        /// </summary>
        public ContentClient()
        {
            // instantiate requried network parameters
            _notificationHandler = new ContentClientNotificationHandler(this);
            _communicator = CommunicationFactory.GetCommunicator();
            //_serializer = new ContentSerializer();
            // subscribe to network module
            try
            {
                _communicator.Subscribe("ContentClient", _notificationHandler);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ContentClient] Error subscribing to networking module.\n{e.GetType().Name} : {e.Message}");
            }
            // instantiate subscribers list
            _subscribers = new List<IMessageListener>();
            // initialize chat and file handler
            _chatHandler = new ChatMessageClient(_communicator);
            _fileHandler = new FileClient(_communicator);
            // instantiate other parameters
            _userID = -1;   
            _lock = new object();
            AllMessages = new List<ChatThread>();
            _messageIDMap = new Dictionary<int, int>();
            _threadIDMap = new Dictionary<int, int>();
            // instantiate message event handler and add functions for each event
            _messageEventHandler = new Dictionary<MessageEvent, Action<ChatData>>
            {
                [MessageEvent.New] = NewMessageHandler,
                [MessageEvent.Edit] = EditMessageHandler,
                [MessageEvent.Delete] = DeleteMessageHandler, 
                [MessageEvent.Star] = StarMessageHandler,
                [MessageEvent.Download] = DownloadMessageHandler
            };
        }

        /// <summary>
        /// Communicator get and set functions
        /// </summary>
        public ICommunicator Communicator
        {
            get { return _communicator; }
            set
            {
                _communicator = value;
                try
                {
                    _communicator.Subscribe("ContentClient", _notificationHandler);
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"[ContentClient] Exception occured while subscribing to networking module.\n{e.GetType().Name} : {e.Message}");
                }
                _chatHandler.Communicator = value;
                _fileHandler.Communicator = value;
            }
        }
        /// <summary>
        /// User ID setter functions
        /// </summary>
        /// 
        public void SetUser(int id, string name, string ip, int port)
        {
            _userID = id;
            _chatHandler.UserID = id;
            _chatHandler.UserName = name;
            _fileHandler.UserID = id;
            _fileHandler.UserName = name;
            _name = name;
            _serverIP = ip;
            _serverPort = port;
        }
           

        /// <summary>
        /// Check for valid reply message ID 
        /// </summary>
        /// <param name="replyMessageID">ID of message being replied to</param>
        /// <param name="threadID">ID of thread the message belongs to</param>
        /// <exception cref="ArgumentException"></exception>
        private void ValidateReplyMessageID(int replyMessageID, int threadID)
        {
            // ensure message being replied to exists
            if (!_messageIDMap.ContainsKey(replyMessageID))
            {
                throw new ArgumentException("Message being replied to does not exist");
            }
            // if received message is part of existing thread, ensure message
            // being replied to is in same thread as the replied message
            if (threadID != -1)
            {
                if (_messageIDMap[replyMessageID] != threadID)
                {
                    throw new ArgumentException("Message being replied to is a part of different thread");
                }
            }
            // if message is part of a new thread, no check is requried
        }

        /// <summary>
        /// Gets message from data structures using message ID
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <returns>Message object implementing the ReceiveChatData class</returns>
        /// <exception cref="ArgumentException"></exception>
        private ReceiveChatData? GetMessage(int messageID)
        {
            // return null if message ID is not present in map
            if (_messageIDMap.TryGetValue(messageID, out int threadID) &&
        _threadIDMap.TryGetValue(threadID, out int threadIndex) &&
        threadIndex >= 0 && threadIndex < AllMessages.Count)
            {
                ChatThread thread = AllMessages[threadIndex];
                int messageIndex = thread.GetMessageIndex(messageID);

                if (messageIndex >= 0 && messageIndex < thread.MessageList.Count)
                {
                    return thread.MessageList[messageIndex];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the array of intersecting receiver IDs of an existing message and a new reply message 
        /// </summary>
        /// <param name="previousReceiverIDs">List of receiver IDs of existing message</param>
        /// <param name="currentReceiverIDs">List of receiver IDs of the new message</param>
        /// <param name="previousSenderID">ID of the sender of existing message</param>
        /// <returns>List of Receiver IDs</returns>
        /// <exception cref="ArgumentException"></exception>
        /*private int[] AllReceivers(int[] previousReceiverIDs, int[] currentReceiverIDs, int previousSenderID)
        {
            // exisiting message receiver ID array is empty implies message is a broadcast message
            // final receiver array is simply the receiver array of the new message
            if (!(previousReceiverIDs?.Length > 0))
            {
                return currentReceiverIDs;
            }
            // add the sender of existing message to the receiver array
            _ = previousReceiverIDs.Concat(new[] { previousSenderID }).ToArray();
            // final receiver array is existing message receiver array if current array is empty 
            if (!(currentReceiverIDs?.Length > 0))
            {
                return previousReceiverIDs;
            }
            // get intersecting array of receiver IDs
            int[] allReceivers = previousReceiverIDs.Intersect(currentReceiverIDs).ToArray();
            if (allReceivers.Length == 0)
            {
                throw new ArgumentException("Invalid array of receivers.");
            }
            return allReceivers;
        }*/

        /// <summary>
        /// Function to check if path is accessible and a file can be created
        /// </summary>
        /// <param name="path">File path</param>
        /// <exception cref="ArgumentException"></exception>
        private void CheckFilePath(string path)
        {
            try
            {
                // check if a file can be created and delete it on close
                using FileStream fs = File.Create(path, 1, FileOptions.DeleteOnClose);
            }
            catch
            {
                throw new ArgumentException("Invalid file path");
            }
        }

        // interface functions

        ///<inheritdoc/>
        public void ClientSendData(SendChatData chatData, string ip, int port)
        {
            // check if receiver ID list is not null
            /*if (chatData.ReceiverIDs is null)
            {
                throw new ArgumentException("List of receiver IDs given is null");
            }*/
            // check if message is part of thread
            if (chatData.ReplyThreadID != -1)
            {
                // make sure that thread exists
                if (!_threadIDMap.ContainsKey(chatData.ReplyThreadID))
                {
                    throw new ArgumentException($"Thread with given reply thread ID ({chatData.ReplyThreadID}) does not exist");
                }
            }
            // if message is a reply to an existing message
            if (chatData.ReplyMessageID != -1)
            {
                ValidateReplyMessageID(chatData.ReplyMessageID, chatData.ReplyThreadID);
                _ = GetMessage(chatData.ReplyMessageID) ?? throw new ArgumentException("Message being replied to does not exist");
                //chatData.ReceiverIDs = AllReceivers(existingMessage.ReceiverIDs, chatData.ReceiverIDs, existingMessage.SenderID);
            }
            // otherwise, use the respective message type handlers
            switch (chatData.Type)
            {
                case MessageType.Chat:
                    Trace.WriteLine("[ContentClient] Using chat handler to send event to server");
                    _chatHandler.NewChat(chatData, ip, port);
                    break;
                case MessageType.File:
                    Trace.WriteLine("[ContentClient] Using file handler to send event to server");
                    _fileHandler.SendFile(chatData, ip, port);
                    break;
                default:
                    throw new ArgumentException($"Invalid Message Field Type : {chatData.Type}");
            }
        }

        ///<inheritdoc/>
        public void ClientSubscribe(IMessageListener subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("Null subscriber input argument");
            }
            else
            {
                // add subscriber to the list of subscribers
                Trace.WriteLine("[ContentClient] Added new subscriber");
                _subscribers.Add(subscriber);
            }
        }

        ///<inheritdoc/>
        public void ClientEdit(int messageID, string newMessage, string ip, int port)
        {
            // get message and check if it is empty
            ReceiveChatData? message = GetMessage(messageID) ?? throw new ArgumentException("Message with given message ID does not exist");
            // check message type
            if (message.Type != MessageType.Chat)
            {
                throw new ArgumentException($"Invalid message type for editing : {message.Type}");
            }
            // check if user can edit the message
            if (message.SenderID != _userID)
            {
                throw new ArgumentException("Edit not allowed for messages from another sender");

            }
            Trace.WriteLine("[ContentClient] Using chat handler to send edit event to server");
            _chatHandler.EditChat(messageID, newMessage, message.ReplyThreadID, ip, port);
        }

        ///<inheritdoc/>
        public void ClientDelete(int messageID, string ip, int port)
        {
            // get message and check if it is empty
            ReceiveChatData message = GetMessage(messageID) ?? throw new ArgumentException("Message being replied to does not exist");
            // check message type
            if (message.Type != MessageType.Chat)
            {
                throw new ArgumentException($"Invalid message type for deleting : {message.Type}");
            }
            // check if user can delete the message
            if (message.SenderID != _userID)
            {
                throw new ArgumentException("Delete not allowed for messages from another sender");
            }
            Trace.WriteLine("[ContentClient] Using chat handler to send delete event to server");
            _chatHandler.DeleteChat(messageID, message.ReplyThreadID, ip, port);
        }

        /// <inheritdoc/>
        public void ClientDownload(int messageID, string savePath, string ip, int port)
        {
            // check save path and message 
            CheckFilePath(savePath);
            ReceiveChatData? message = GetMessage(messageID);
            if (message is null)
            {
                Trace.WriteLine("[ContentClient] File requested for download not found");
                throw new ArgumentException("File requested for download does not exist");
            }
            // check message type
            if (message.Type != MessageType.File)
            {
                throw new ArgumentException($"Invalid message type : {message.Type}");
            }
            Trace.WriteLine("[ContentClient] Using file handler to send download file event to server");
            _fileHandler.DownloadFile(messageID, savePath, ip, port);
        }

        ///<inheritdoc/>
        public void ClientStar(int messageID, string ip, int port)
        {
            // check if message is empty
            ReceiveChatData? message = GetMessage(messageID) ?? throw new ArgumentException("Message with given message ID to does not exist");
            // check message type
            if (message.Type != MessageType.Chat)
            {
                throw new ArgumentException($"Invalid message type : {message.Type}");
            }
            Trace.WriteLine("[ContentClient] Using chat handler to send star chat to server");
            _chatHandler.StarChat(messageID, message.ReplyThreadID, ip, port);
        }

        /// <inheritdoc/>
        public ChatThread ClientGetThread(int threadID)
        {
            // check if thread exists
            if (!_threadIDMap.ContainsKey(threadID))
            {
                throw new ArgumentException("Thread with given thread ID does not exist");

            }
            int index = _threadIDMap[threadID];
            Trace.WriteLine($"[ContentClient] Returning thread with ID = {threadID}");
            return AllMessages[index];
        }

        /// <inheritdoc/>
        public int GetUserID()
        {
            return _userID;
        }

        public string GetUserName()
        {
            return _name;
        }

        public string GetIP()
        {
            return _serverIP;
        }

        public int GetPort()
        {
            return _serverPort;
        }

        // event handler helper functions

        /// <summary>
        /// Notifies the subscribers of the received message
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void Notify(ReceiveChatData message)
        {
            Trace.WriteLine("[ContentClient] Notifying subscribers of new received message");
            foreach (IMessageListener subscriber in _subscribers)
            {
                _ = Task.Run(() => { subscriber.OnMessageReceived(message); });
            }
        }

        // event handler functions

        /// <summary>
        /// Handles the new message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void NewMessageHandler(ChatData message)
        {
            Trace.WriteLine("[ContentClient] Received message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveChatData receivedMessage = message;
            // check if message ID is unique 
            if (_messageIDMap.ContainsKey(receivedMessage.MessageID))
            {
                Debug.Print("{0}", receivedMessage.MessageID);
                throw new ArgumentException("Message ID is not unique");
            }
            // check if thread exists
            int key = receivedMessage.ReplyThreadID;
            int temp = receivedMessage.ReplyMessageID;            
            // if message is a reply, check if message being replied to exists
            if (receivedMessage.ReplyMessageID != -1)
            {
                ValidateReplyMessageID(receivedMessage.ReplyMessageID, receivedMessage.ReplyThreadID);
            }
            // using locks as the data structures may be shared across multiple system threads
            lock (_lock)
            {
                // add message ID to message ID map
                _messageIDMap.Add(receivedMessage.MessageID, key);
                // add message to all messages list if message belongs to a chat thread
                if (_threadIDMap.TryGetValue(key, out int index))
                {
                    AllMessages[index].AddMessage(receivedMessage);
                }
                else // create new thread if the message does not belong to any thread
                {
                    var newThread = new ChatThread();
                    newThread.AddMessage(receivedMessage);
                    AllMessages.Add(newThread);
                    // add entry into the thread ID map
                    _threadIDMap.Add(key, AllMessages.Count - 1);
                }
            }
            // notfiy subscribers of the new message
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the edit message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void EditMessageHandler(ChatData message)
        {
            Trace.WriteLine("[ContentClient] Received edited message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveChatData receivedMessage = message;
            int messageID = receivedMessage.MessageID;
            // check if message ID is present 
            if (!_messageIDMap.TryGetValue(messageID, out int threadID))
            {
                throw new ArgumentException("Message with message ID is not present");
            }
            // check if thread exists with the threadId extracted
            if (!_threadIDMap.TryGetValue(threadID, out int index))
            {
                throw new ArgumentException("No message thread with given ID exists");
            }
            lock (_lock)
            {
                string newMessage = receivedMessage.Data;
                AllMessages[index].EditMessage(messageID, newMessage);
            }
            // notfiy subscribers of the edited message
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the delete message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void DeleteMessageHandler(ChatData message)
        {
            Trace.WriteLine("[ContentClient] Received deleted message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveChatData receivedMessage = message;
            int messageID = receivedMessage.MessageID;
            // check if message ID is present 
            if (!_messageIDMap.TryGetValue(messageID, out int threadID))
            {
                throw new ArgumentException("Message with message ID is not present");
            }
            // check if thread exists with the threadId extracted
            if (!_threadIDMap.TryGetValue(threadID, out int index))
            {
                throw new ArgumentException("No message thread with given ID exists");
            }
            
            lock (_lock)
            {
                AllMessages[index].DeleteMessage(messageID);
            }
            // notfiy subscribers of the edited message
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the star message event - checks the message data and 
        /// makes updates accordingly in the internal data structures. 
        /// Also notifies all the subscribers about the received message.
        /// </summary>
        /// <param name="message">Received message object from server</param>
        /// <exception cref="ArgumentException"></exception>
        private void StarMessageHandler(ChatData message)
        {
            Trace.WriteLine("[ContentClient] Received starred message from server");
            // ensure file data is null
            message.FileData = null;
            ReceiveChatData receivedMessage = message;
            int messageID = receivedMessage.MessageID;
            // check if message ID is present 
            if (!_messageIDMap.TryGetValue(messageID, out int threadID))
            {
                throw new ArgumentException("Message with message ID is not present");
            }
            // check if thread exists with the threadId extracted
            if (!_threadIDMap.TryGetValue(threadID, out int index))
            {
                throw new ArgumentException("No message thread with given ID exists");
            }
            
            lock (_lock)
            {
                AllMessages[index].StarMessage(messageID);
            }
            Notify(receivedMessage);
        }

        /// <summary>
        /// Handles the download file event - gets the save path from the message
        /// data and writes the data onto the file on the path
        /// </summary>
        /// <param name="message">Received message object from server</param>
        private void DownloadMessageHandler(ChatData message)
        {
            Trace.WriteLine("[ContentClient] Received requested file from server");
            string savePath = message.Data;
            Trace.WriteLine($"[ContentClient] Saving file to path : {savePath}");
            File.WriteAllBytes(savePath, message.FileData.Data);
        }

        /// <summary>
        /// Reset class parameters and data structures
        /// </summary>
        public void Reset()
        {
            _userID = -1;
            _subscribers = new List<IMessageListener>();
            lock (_lock)
            {
                AllMessages = new List<ChatThread>();
                _threadIDMap = new Dictionary<int, int>();
                _messageIDMap = new Dictionary<int, int>();
            }
        }
        /// <summary>
        /// Handles received messages from network
        /// </summary>
        /// <param name="receivedMessage">Received message object from network</param>
        /// <exception cref="ArgumentException"></exception>
        public void OnReceive(ChatData receivedMessage)
        {
            if (receivedMessage is null)
            {
                throw new ArgumentException("Received null message!");
            }
            Trace.WriteLine("[ContentClient] Received message from server");
            _messageEventHandler[receivedMessage.Event](receivedMessage);
        }
    }
}
