/******************************************************************************
 * Filename    = ContentTests.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = tests
 *****************************************************************************/

using MessengerContent.Enums;
using MessengerContent;
using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerTests.ContentTests;
using MessengerApp;
using SharpDX;

namespace MessengerTests.ContentTests.Client
{
    [TestClass]
    public class ContentClientTests
    {
        private MockCommunicator _communicator;
        private MockMessageListener _listener;
        private ContentClient _contentClient;

        private MockHelper _mockHelper;
        private IContentSerializer _serializer;
        private string _path;

        private readonly int _userID = 42;

        // max valid message and thread IDs accepted
        private int _maxValidMessageID;
        private int _maxValidThreadID;
        // 
        private readonly int _sleepTime = 50;
        private ChatData _chatMessage;
        private ChatData _fileMessage;
        private ChatData _userMessage;

        /// <summary>
        /// Sets up the content client instance and mock components requried for testing
        /// </summary>
        public void Setup()
        {
            _mockHelper = new MockHelper();
            _communicator = new MockCommunicator();
            _listener = new MockMessageListener();
            _contentClient = (ContentClient)ContentClientFactory.GetInstance();
            _serializer = new ContentSerializer();

            _contentClient.SetUser(_userID, null);
            _contentClient.Communicator = _communicator;
            _contentClient.ClientSubscribe(_listener);

            string currentDirectory = Directory.GetCurrentDirectory();
            string[] pathArray = currentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            _path = pathArray[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";

            // chat message sent by some user with ID = 1
            _chatMessage = _mockHelper.GenerateChatData(
                type: MessageType.Chat,
                @event: MessageEvent.New,
                data: "This is a sample message string",
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID,
                senderID: 1
            );

            // file message sent by some user with ID = 2
            _fileMessage = _mockHelper.GenerateChatData(
                type: MessageType.File,
                @event: MessageEvent.New,
                data: _path,
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID,
                senderID: 2
            );
            _fileMessage.FileData = new SendFileData(_path);

            // a chat message sent by the user (with ID = 42)
            _userMessage = _chatMessage.Copy();
            _userMessage.SenderID = _userID;
            _userMessage.MessageID = ++_maxValidMessageID;
            _userMessage.ReplyThreadID = ++_maxValidThreadID;

            // sleep after each OnReceive call as notifying subscribers may
            // take some time (as new threads are created for each subscriber)
            _contentClient.OnReceive(_chatMessage);
            Thread.Sleep(_sleepTime);

            _contentClient.OnReceive(_fileMessage);
            Thread.Sleep(_sleepTime);

            _contentClient.OnReceive(_userMessage);
            Thread.Sleep(_sleepTime);
        }

        /// <summary>
        /// Resets the content client instance and other variables
        /// </summary>
        public void Dispose()
        {
            _contentClient.Reset();
            _maxValidMessageID = 0;
            _maxValidThreadID = 0;
        }

        [TestMethod]
        public void ClientSendData_ChatNotReply_ReturnsValidDataAtServer()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData();

            _contentClient.ClientSendData(sendChatData);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(sendChatData.Type, deserializedData.Type);
            Assert.AreEqual(MessageEvent.New, deserializedData.Event);
            Assert.AreEqual(sendChatData.Data, deserializedData.Data);
            Assert.AreEqual(sendChatData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.AreEqual(_userID, deserializedData.SenderID);
            Assert.IsFalse(deserializedData.Starred);

            Dispose();
        }

        [TestMethod]
        public void ClientSendData_ChatReplyExistingThread_ReturnsValidDataAtServer()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(
                replyMessageID: _chatMessage.MessageID,
                replyThreadID: _chatMessage.ReplyThreadID
            );

            _contentClient.ClientSendData(sendChatData);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(sendChatData.Type, deserializedData.Type);
            Assert.AreEqual(MessageEvent.New, deserializedData.Event);
            Assert.AreEqual(sendChatData.Data, deserializedData.Data);
            Assert.AreEqual(sendChatData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.AreEqual(_userID, deserializedData.SenderID);
            Assert.IsFalse(deserializedData.Starred);

            Dispose();
        }

        [TestMethod]
        public void ClientSendData_ChatReplyNewThread_ReturnsValidDataAtServer()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(
                replyMessageID: _chatMessage.ReplyMessageID
            );

            _contentClient.ClientSendData(sendChatData);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(sendChatData.Type, deserializedData.Type);
            Assert.AreEqual(MessageEvent.New, deserializedData.Event);
            Assert.AreEqual(sendChatData.Data, deserializedData.Data);
            Assert.AreEqual(sendChatData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.AreEqual(_userID, deserializedData.SenderID);
            Assert.IsFalse(deserializedData.Starred);

            Dispose();
        }

        [TestMethod]
        public void ClientSendData_ValidFile_ReturnsValidDataAtServer()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(MessageType.File, _path);

            _contentClient.ClientSendData(sendChatData);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);
            var fileData = new SendFileData(_path);

            Assert.AreEqual(sendChatData.Type, deserializedData.Type);
            Assert.AreEqual(MessageEvent.New, deserializedData.Event);
            //Assert.AreEqual(sendChatData.Data, deserializedData.Data);
            Assert.AreEqual(System.Convert.ToBase64String(fileData.Data), System.Convert.ToBase64String(deserializedData.FileData.Data));
            Assert.AreEqual(fileData.Name, deserializedData.FileData.Name);
            Assert.AreEqual(fileData.Size, deserializedData.FileData.Size);
            Assert.AreEqual(sendChatData.ReplyMessageID, deserializedData.ReplyMessageID);
            Assert.AreEqual(_userID, deserializedData.SenderID);
            Assert.IsFalse(deserializedData.Starred);

            Dispose();
        }

        [TestMethod]
        public void ClientSendData_ChatInvalidReplyThreadID_ReturnsArgumentException()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(replyThreadID: _maxValidThreadID + 1);

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientSendData(sendChatData));

            Dispose();
        }
        
        [TestMethod]
        public void ClientSendData_ChatInvalidReplyMessageID_ReturnsArgumentException()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(replyThreadID: _maxValidThreadID+1);

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientSendData(sendChatData));

            Dispose();
        }

        [TestMethod]
        public void ClientSendData_InValidReplyThreadID_ReturnsArgumentException()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(replyMessageID: _maxValidMessageID + 1);

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientSendData(sendChatData));

            Dispose(); 
        }

        [TestMethod]
        public void ClientSendData_InvalidType_ReturnsArgumentException()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData();
            sendChatData.Type += 4;

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientSendData(sendChatData));

            Dispose();
        }
        /*
        [TestMethod]
        public void ClientSendData_HistoryType_ReturnsArgumentException()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData();
            sendChatData.Type = MessageType.HistoryRequest;
            _contentClient.ClientSendData(sendChatData);

            Assert.AreEquals(_contentClient.chatData,);

            Dispose();
        }*/

        [TestMethod]
        public void ClientSendData_InvalidFilePath_ReturnsFileNotFoundException()
        {
            Setup();
            SendChatData sendChatData = _mockHelper.GenerateSendChatData(
                type: MessageType.File,
                data: "This is supposed to be a file path"
            );

            Assert.ThrowsException<FileNotFoundException>(() => _contentClient.ClientSendData(sendChatData));

            Dispose();
        }

        [TestMethod]
        public void ClientSubscribe_NullSubscriber_ReturnsArgumentException()
        {
            Setup();
            Messenger.Client.IMessageListener subscriber = null;

            Assert.ThrowsException<ArgumentNullException>(() => _contentClient.ClientSubscribe(subscriber));

            Dispose();
        }

        [TestMethod]
        public void ClientEdit_ValidEdit_ReturnsValidDataAtServer()
        {
            Setup();
            string newMessage = "This is the edited message string";
            _contentClient.ClientEdit(_userMessage.MessageID, newMessage);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(MessageType.Chat, deserializedData.Type);
            Assert.AreEqual(MessageEvent.Edit, deserializedData.Event);
            Assert.AreEqual(newMessage, deserializedData.Data);
            Assert.AreEqual(_userMessage.MessageID, deserializedData.MessageID);
            Assert.AreEqual(_userMessage.ReplyThreadID, deserializedData.ReplyThreadID);

            Dispose();
        }

        [TestMethod]
        public void ClientEdit_InvalidMessageID_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "This is the edited message string";

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientEdit(_maxValidMessageID + 1, newMessage));

            Dispose();
        }

        [TestMethod]
        public void ClientEdit_OtherUserEdit_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "This is the edited message string";

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientEdit(_chatMessage.MessageID, newMessage));

            Dispose();
        }

        [TestMethod]
        public void ClientEdit_EditFile_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "This is the edited message string";

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientEdit(_fileMessage.MessageID, newMessage));

            Dispose();
        }

        [TestMethod]
        public void ClientEdit_NullEditedMessage_ReturnsArgumentException()
        {
            Setup();
            string newMessage = null;

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientEdit(_userMessage.MessageID, newMessage));

            Dispose();
        }

        [TestMethod]
        public void ClientEdit_EmptyEditedMessage_ReturnsArgumentException()
        {
            Setup();
            string newMessage = "";

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientEdit(_userMessage.MessageID, newMessage));

            Dispose();
        }

        [TestMethod]
        public void ClientDelete_ValidDelete_ReturnsValidDataAtServer()
        {
            Setup();
            _contentClient.ClientDelete(_userMessage.MessageID);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(MessageType.Chat, deserializedData.Type);
            Assert.AreEqual(MessageEvent.Delete, deserializedData.Event);
            Assert.AreEqual("Message Deleted.", deserializedData.Data);
            Assert.AreEqual(_userMessage.MessageID, deserializedData.MessageID);
            Assert.AreEqual(_userMessage.ReplyThreadID, deserializedData.ReplyThreadID);

            Dispose();
        }

        [TestMethod]
        public void ClientDelete_InvalidMessageID_ReturnsArgumentException()
        {
            Setup();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientDelete(_maxValidMessageID + 1));

            Dispose();
        }

        [TestMethod]
        public void ClientDelete_OtherUserDelete_ReturnsArgumentException()
        {
            Setup();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientDelete(_chatMessage.MessageID));

            Dispose();
        }

        [TestMethod]
        public void ClientDelete_DeleteFile_ReturnsArgumentException()
        {
            Setup();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientDelete(_fileMessage.MessageID));

            Dispose();
        }

        [TestMethod]
        public void ClientDownload_ValidDownload_ReturnsValidDataAtServer()
        {
            Setup();
            string savePath = Path.GetRandomFileName();
            _contentClient.ClientDownload(_fileMessage.MessageID, savePath);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(MessageType.File, deserializedData.Type);
            Assert.AreEqual(MessageEvent.Download, deserializedData.Event);
            Assert.AreEqual(_fileMessage.MessageID, deserializedData.MessageID);
            Assert.AreEqual(savePath, deserializedData.Data);

            Dispose();
        }

        [TestMethod]
        public void ClientDownload_InvalidPath_ReturnsArgumentException()
        {
            Setup();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientDownload(_fileMessage.MessageID, ""));

            Dispose();
        }

        [TestMethod]
        public void ClientDownload_DownloadChatMessage_ReturnsArgumentException()
        {
            Setup();
            string savePath = Path.GetRandomFileName();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientDownload(_chatMessage.MessageID, savePath));

            Dispose();
        }

        [TestMethod]
        public void ClientStar_ValidStar_ReturnsValidDataAtServer()
        {
            Setup();
            _contentClient.ClientStar(_userMessage.MessageID);
            string sentData = _communicator.GetSendData();
            ChatData deserializedData = _serializer.Deserialize<ChatData>(sentData);

            Assert.AreEqual(MessageType.Chat, deserializedData.Type);
            Assert.AreEqual(MessageEvent.Star, deserializedData.Event);
            Assert.AreEqual(_userMessage.MessageID, deserializedData.MessageID);
            Assert.AreEqual(_userMessage.ReplyThreadID, deserializedData.ReplyThreadID);

            Dispose();
        }

        [TestMethod]
        public void ClientStar_InvalidMessageID_ReturnsArgumentException()
        {
            Setup();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientStar(_maxValidMessageID + 1));

            Dispose();
        }

        [TestMethod]
        public void ClientStar_StarFile_ReturnsArgumentException()
        {
            Setup();

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientStar(_fileMessage.MessageID));

            Dispose();
        }

        [TestMethod]
        public void ClientGetThread_ValidThreadID_ReturnsValidThread()
        {
            Setup();
            int threadID = _chatMessage.ReplyThreadID;
            ChatThread thread = _contentClient.ClientGetThread(threadID);

            Assert.AreEqual(1, thread.MessageCount);
            Assert.AreEqual(threadID, thread.ThreadID);

            Dispose();
        }

        [TestMethod]
        public void ClientGetThread_InvalidThreadID_ReturnsArgumentException()
        {
            Setup();
            int threadID = _maxValidThreadID + 1;

            Assert.ThrowsException<ArgumentException>(() => _contentClient.ClientGetThread(threadID));

            Dispose();
        }

        [TestMethod]
        public void GetUserID_SingleInstance_ReturnsValidUserID()
        {
            Setup();
            int userID = _contentClient.GetUserID();

            Assert.AreEqual(42, userID);

            Dispose();
        }

        [TestMethod]
        public void Reset_SingleInstance_ReturnsEmptyInstance()
        {
            Setup();
            _contentClient.Reset();

            Assert.AreEqual(-1, _contentClient.UserID);
            Assert.AreEqual(_contentClient.AllMessages.Count, 0);

            Dispose();
        }

        [TestMethod]
        public void OnReceive_ValidNewMessageNotReplyNewThread_StoresMessageAndInformsSubscribers()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID
            );

            _contentClient.OnReceive(message);
            Thread.Sleep(_sleepTime);
            ChatThread messageThread = _contentClient.ClientGetThread(message.ReplyThreadID);
            int index = messageThread.GetMessageIndex(message.MessageID);
            ReceiveChatData storedMessage = messageThread.MessageList[index];
            ReceiveChatData receivedMessage = _listener.GetReceivedMessage();

            _mockHelper.CheckReceiveChatData(message, storedMessage);
            _mockHelper.CheckReceiveChatData(message, receivedMessage);

            Dispose();

        }

        [TestMethod]
        public void OnReceive_ValidNewMessageNotReplyExistingThread_StoresMessageAndInformsSubscribers()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                messageID: ++_maxValidMessageID,
                replyThreadID: _chatMessage.ReplyThreadID
            );

            _contentClient.OnReceive(message);
            Thread.Sleep(_sleepTime);
            ChatThread messageThread = _contentClient.ClientGetThread(message.ReplyThreadID);
            int index = messageThread.GetMessageIndex(message.MessageID);
            ReceiveChatData storedMessage = messageThread.MessageList[index];
            ReceiveChatData receivedMessage = _listener.GetReceivedMessage();

            _mockHelper.CheckReceiveChatData(message, storedMessage);
            _mockHelper.CheckReceiveChatData(message, receivedMessage);

            Dispose();

        }



        [TestMethod]
        public void OnReceive_ValidNewReplyMessageExistingThread_StoresMessageAndInformsSubscribers()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                messageID: ++_maxValidMessageID,
                replyThreadID: _chatMessage.ReplyThreadID,
                replyMessageID: _chatMessage.MessageID
            );

            _contentClient.OnReceive(message);
            Thread.Sleep(_sleepTime);
            ChatThread messageThread = _contentClient.ClientGetThread(message.ReplyThreadID);
            int index = messageThread.GetMessageIndex(message.MessageID);
            ReceiveChatData storedMessage = messageThread.MessageList[index];
            ReceiveChatData receivedMessage = _listener.GetReceivedMessage();

            _mockHelper.CheckReceiveChatData(message, storedMessage);
            _mockHelper.CheckReceiveChatData(message, receivedMessage);

            Dispose();
        }

        [TestMethod]
        public void OnReceive_NewMessageDuplicateMessageID_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(messageID: _chatMessage.MessageID);

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_NewMessageInvalidThreadID_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(messageID: ++_maxValidMessageID);

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_MessageNotPassed_ReturnsArgumentException()
        {
            Setup();
            ChatData receivedMessage = null;

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(receivedMessage));

            Dispose();
        }

        [TestMethod]
        public void OnReceivePolymorphed_MessageNotPassed_ReturnsArgumentException()
        {
            Setup();
            List<ChatThread> allMessages = null;

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(allMessages));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_NewEmptyMessage_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                data: "",
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_NewNullMessage_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                data: null,
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_NewMessageInvalidReplyMessageID_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                messageID: ++_maxValidMessageID,
                replyThreadID: ++_maxValidThreadID,
                replyMessageID: ++_maxValidMessageID
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_ValidEditMessage_StoresMessageAndInformsSubscribers()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Edit,
                messageID: _chatMessage.MessageID,
                replyThreadID: _chatMessage.ReplyThreadID,
                data: "This is a sample edited message"
            );

            _contentClient.OnReceive(message);
            Thread.Sleep(_sleepTime);
            ChatThread messageThread = _contentClient.ClientGetThread(message.ReplyThreadID);
            int index = messageThread.GetMessageIndex(message.MessageID);
            ReceiveChatData storedMessage = messageThread.MessageList[index];
            ReceiveChatData receivedMessage = _listener.GetReceivedMessage();

            Assert.AreEqual(message.Data, storedMessage.Data);
            Assert.AreEqual(message.Data, receivedMessage.Data);

            Dispose();
        }

        [TestMethod]
        public void OnReceive_EditMessageInvalidMessageID_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Edit,
                messageID: ++_maxValidMessageID,
                data: "This is a sample edited message"
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_EditFileMessage_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Edit,
                messageID: _fileMessage.MessageID,
                data: "This is a sample edited message"
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_ValidDeleteMessage_StoresMessageAndInformsSubscribers()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Delete,
                messageID: _chatMessage.MessageID,
                replyThreadID: _chatMessage.ReplyThreadID
            );

            _contentClient.OnReceive(message);
            Thread.Sleep(_sleepTime);
            ChatThread messageThread = _contentClient.ClientGetThread(message.ReplyThreadID);
            int index = messageThread.GetMessageIndex(message.MessageID);
            ReceiveChatData storedMessage = messageThread.MessageList[index];
            ReceiveChatData receivedMessage = _listener.GetReceivedMessage();

            Assert.AreEqual("Message Deleted.", storedMessage.Data);
            Assert.AreEqual(message.MessageID, receivedMessage.MessageID);

            Dispose();
        }

        [TestMethod]
        public void OnReceive_DeleteMessageInvalidMessageID_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(@event: MessageEvent.Delete, messageID: ++_maxValidMessageID);

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }
        [TestMethod]
        public void Getting_And_SettingUserID()
        {
            Setup();
            _contentClient.UserID = 1;
            Assert.AreEqual(_contentClient.UserID,1);

            Dispose();
        }

        [TestMethod]
        public void OnReceive_ValidStarMessage_StoresMessageAndInformsSubscribers()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Star,
                messageID: _chatMessage.MessageID,
                replyThreadID: _chatMessage.ReplyThreadID,
                starred: _chatMessage.Starred
            );
            bool isStarred = message.Starred;

            _contentClient.OnReceive(message);
            Thread.Sleep(_sleepTime);
            ChatThread messageThread = _contentClient.ClientGetThread(message.ReplyThreadID);
            int index = messageThread.GetMessageIndex(message.MessageID);
            ReceiveChatData storedMessage = messageThread.MessageList[index];
            ReceiveChatData receivedMessage = _listener.GetReceivedMessage();

            Assert.AreEqual(!isStarred, storedMessage.Starred);
            Assert.AreEqual(isStarred, receivedMessage.Starred);

            Dispose();
        }

        [TestMethod]
        public void OnReceive_StarMessageInvalidMessageID_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Star,
                messageID: ++_maxValidMessageID
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_StarFileMessage_ReturnsArgumentException()
        {
            Setup();
            ChatData message = _mockHelper.GenerateChatData(
                @event: MessageEvent.Star,
                messageID: _fileMessage.MessageID
            );

            Assert.ThrowsException<ArgumentException>(() => _contentClient.OnReceive(message));

            Dispose();
        }

        [TestMethod]
        public void OnReceive_ValidChatThreadList_SetsAllMessageAndInformsSubscribers()
        {
            Setup();
            List<ChatThread> chatThreads = _contentClient.AllMessages;
            chatThreads.RemoveAt(0);

            _contentClient.OnReceive(chatThreads);
            Thread.Sleep(_sleepTime);
            List<ChatThread> newChatThreads = _contentClient.AllMessages;
            List<ChatThread> receivedChatThreads = _listener.GetAllMessages();

            _mockHelper.CheckChatThreadLists(chatThreads, newChatThreads);
            _mockHelper.CheckChatThreadLists(chatThreads, receivedChatThreads);

            Dispose();
        }
    }
}
