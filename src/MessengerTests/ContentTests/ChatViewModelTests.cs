/******************************************************************************
* Filename    = ChatViewModelTests.cs
*
* Author      = M V Nagasurya
*
* Product     = MessengerApp
* 
* Project     = MessengerTests
*
* Description = All Unit Tests for Chat ViewModel
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.ViewModels;
using MessengerDashboard;
using MessengerContent.Enums;
using MessengerContent.DataModels;
using static MessengerTests.ContentTests.DispatchUtils;
using MessengerDashboard.Client;
using MessengerContent.Client;
using System.ComponentModel;
using MessengerApp;

namespace MessengerTests.ContentTests
{
    [TestClass]
    public class ChatViewModelTests
    {
        private ChatPageViewModel? _viewModel;

        /// <summary>
        /// Validating that the UserIdToNames dictionary gets updates whenever the users join and leave the meeting
        /// </summary>
        [TestMethod]
        public void TestOnUserSessionChange()
        {
            _viewModel = new ChatPageViewModel();

            var session = new SessionInfo();
            // Add two users into the session
            var user1 = new UserInfo("Surya", 1);
            var user2 = new UserInfo("M V Nagasurya", 2);
            session.Users.Add(user1);
            session.Users.Add(user2);

            // Validate whether the users are added into UserIdToNames
            _viewModel.OnUserSessionChange(session);

            UiDispatcherHelper.ProcessUiEvents();
            Assert.AreEqual(2, _viewModel.UserIdToNames.Count);
            Assert.AreEqual("Surya", _viewModel.UserIdToNames[1]);
            Assert.AreEqual("M V Nagasurya", _viewModel.UserIdToNames[2]);
        }

        /// <summary>
        /// Validates that the view model processes the new received message
        /// </summary>
        [TestMethod]
        public void TestOnNewMessageReceived()
        {
            _viewModel = new ChatPageViewModel();

            var session = new SessionInfo();
            // Add two users into the session
            var user1 = new UserInfo("surya", 1);
            var user2 = new UserInfo("M V Nagasurya", 2);
            session.Users.Add(user1);
            session.Users.Add(user2);

            // Create a received chat message
            ReceiveChatData receivedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            UiDispatcherHelper.ProcessUiEvents();
            _viewModel.OnUserSessionChange(session);
            UiDispatcherHelper.ProcessUiEvents();

            // process the new received message
            _viewModel.OnMessageReceived(receivedChat);

            UiDispatcherHelper.ProcessUiEvents();
            // Validate the message receiving functionality
            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Yo! Surya", _viewModel.ReceivedMsg.MsgData);
            Assert.AreEqual(null, _viewModel.ReceivedMsg.ReplyMessage);
            Assert.IsTrue(_viewModel.ReceivedMsg.MessageType);
        }

        /// <summary>
        /// Validates that the view model processes the chat message History
        /// </summary>
        [TestMethod]
        public void TestOnAllMessagesReceived()
        {
            _viewModel = new ChatPageViewModel(true);
            
            // Create new session instance and user instances
            var session = new SessionInfo();
            var user1 = new UserInfo("surya", 1);
            var user2 = new UserInfo("M V Nagasurya", 2);
            // Add the users to the session
            session.Users.Add(user1);
            session.Users.Add(user2);

            // Create a new ChatThread and two messages
            List<ChatThread> chatThreads = new();
            List<ReceiveChatData> Messages = new();
            ReceiveChatData testingChatData1 = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                ReplyThreadID = 1,
                SenderID = 3,
                SenderName = "Siddhu",
                SentTime = DateTime.Now,
                Starred = false
            };
            ReceiveChatData testingChatData2 = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Hello! Surya",
                MessageID = 2,
                ReplyMessageID = 1,
                ReplyThreadID = 1,
                SenderID = 2,
                SenderName = "M V Nagasurya",
                SentTime = DateTime.Now,
                Starred = false
            };
            // Add these two messages to the chat thread
            Messages.Add(testingChatData1);
            Messages.Add(testingChatData2);

            Dictionary<int, int> IdToIndex = new()
            {
                { 1, 0 },
                { 2, 1 }
            };

            ChatThread chatThread1 = new()
            {
                ThreadID = 1,
                CreationTime = DateTime.Now,
                MessageList = Messages,
                MessageIDToIndex = IdToIndex
            };
            
            chatThreads.Add(chatThread1);

            UiDispatcherHelper.ProcessUiEvents();
            
            // Update the session users information
            _viewModel.OnUserSessionChange(session);
            UiDispatcherHelper.ProcessUiEvents();

            // process all the chat messages
            _viewModel.OnAllMessagesReceived(chatThreads);

            UiDispatcherHelper.ProcessUiEvents();

            // Validation of the Chat Messages received
            Assert.AreEqual("Siddhu", _viewModel.TestChatMessages[0].Sender);
            Assert.AreEqual("M V Nagasurya", _viewModel.TestChatMessages[1].Sender);
        }

        /// <summary>
        /// Validates that the view model processes the edited received message
        /// </summary>
        [TestMethod]
        public void TestOnEditMessageReceived()
        {
            _viewModel = new ChatPageViewModel();

            var session = new SessionInfo();
            // Add two users into the session
            var user1 = new UserInfo("surya", 1);
            var user2 = new UserInfo("M V Nagasurya", 2);
            session.Users.Add(user1);
            session.Users.Add(user2);

            // Create a received chat message
            ReceiveChatData receivedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            UiDispatcherHelper.ProcessUiEvents();
            _viewModel.OnUserSessionChange(session);
            UiDispatcherHelper.ProcessUiEvents();

            _viewModel.OnMessageReceived(receivedChat);

            UiDispatcherHelper.ProcessUiEvents();
            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Yo! Surya", _viewModel.ReceivedMsg.MsgData);

            // edit the message with MessageID = 1
            ReceiveChatData editedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.Edit,
                Data = "Hello! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            UiDispatcherHelper.ProcessUiEvents();

            // process the edited message
            _viewModel.OnMessageReceived(editedChat);
            UiDispatcherHelper.ProcessUiEvents();

            // Validation of the edited message
            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Hello! Surya", _viewModel.ReceivedMsg.MsgData);
            Assert.AreEqual(null, _viewModel.ReceivedMsg.ReplyMessage);
            Assert.IsTrue(_viewModel.ReceivedMsg.MessageType);
        }

        /// <summary>
        /// Validates that the view model processes the deleted received message
        /// </summary>
        [TestMethod]
        public void TestOnDeleteMessageReceived()
        {
            _viewModel = new ChatPageViewModel();

            var session = new SessionInfo();
            // Add two users into the session
            var user1 = new UserInfo("surya", 1);
            var user2 = new UserInfo("M V Nagasurya", 2);
            session.Users.Add(user1);
            session.Users.Add(user2);

            // Create a received chat message
            ReceiveChatData receivedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            _viewModel.OnUserSessionChange(session);

            UiDispatcherHelper.ProcessUiEvents();
            _viewModel.OnMessageReceived(receivedChat);
            UiDispatcherHelper.ProcessUiEvents();
            
            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Yo! Surya", _viewModel.ReceivedMsg.MsgData);

            // delete the message with MessageID = 1
            ReceiveChatData deletedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.Delete,
                Data = "Message Deleted.",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            UiDispatcherHelper.ProcessUiEvents();
            // process the deleted message
            _viewModel.OnMessageReceived(deletedChat);
            UiDispatcherHelper.ProcessUiEvents();

            // Validation of the deleted message
            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Message Deleted.", _viewModel.ReceivedMsg.MsgData);
            Assert.AreEqual(null, _viewModel.ReceivedMsg.ReplyMessage);
            Assert.IsTrue(_viewModel.ReceivedMsg.MessageType);
        }

        [TestMethod]
        public void TestOnStarMessageReceived()
        {
            _viewModel = new ChatPageViewModel();

            var session = new SessionInfo();
            // Add two users into the session
            var user1 = new UserInfo("surya", 1);
            var user2 = new UserInfo("M V Nagasurya", 2);
            session.Users.Add(user1);
            session.Users.Add(user2);

            // Create a received chat message
            ReceiveChatData receivedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.Star,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            _viewModel.OnUserSessionChange(session);

            UiDispatcherHelper.ProcessUiEvents();
            _viewModel.OnMessageReceived(receivedChat);
            UiDispatcherHelper.ProcessUiEvents();

            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Yo! Surya", _viewModel.ReceivedMsg.MsgData);

            // star the message with MessageID = 1
            ReceiveChatData starredChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.Star,
                Data = "Message",
                MessageID = 1,
                ReplyMessageID = -1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = true
            };

            UiDispatcherHelper.ProcessUiEvents();
            // process the starred message
            _viewModel.OnMessageReceived(starredChat);
            UiDispatcherHelper.ProcessUiEvents();

            // Validation of the deleted message
            Assert.AreEqual("surya", _viewModel.ReceivedMsg.Sender);
            Assert.AreEqual(1, _viewModel.ReceivedMsg.MessageID);
            Assert.AreEqual("Message", _viewModel.ReceivedMsg.MsgData);
            Assert.AreEqual(null, _viewModel.ReceivedMsg.ReplyMessage);
            Assert.IsTrue(_viewModel.ReceivedMsg.MessageType);
        }

        /// <summary>
        /// Validating that the propertyChangedEvent is triggered whenever a property is changed
        /// </summary>
        [TestMethod]
        public void TestOnPropertyChangedEvent()
        {
            _viewModel = new ChatPageViewModel();
            string? propertyName = string.Empty;

            // Subscribe a listener to the propertChangedEvent
            _viewModel.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                propertyName = e.PropertyName;
            };

            // Trigger the propertyChanged event
            _viewModel.OnPropertyChanged("ReceivedMsg");

            // Validation of the property
            Assert.IsNotNull(propertyName);
            Assert.AreEqual("ReceivedMsg", propertyName);
        }

        /// <summary>
        /// Validating the chat message sending functionality of the view model
        /// </summary>
        [TestMethod]
        public void TestSendChatMessageWithNoReply()
        {
            _viewModel = new ChatPageViewModel();
            string chatMsg = "Chat Message!";
            int replyMsgId = -1;
            string chatMsgType = "Chat";

            // Send the message
            _viewModel.SendMessage(chatMsg, replyMsgId, chatMsgType);

            // Validation of the sent message
            Assert.AreEqual("Chat Message!", _viewModel.MsgToSend.Data);
            Assert.AreEqual(-1, _viewModel.MsgToSend.ReplyThreadID);
        }

        /// <summary>
        /// Validating the chat message(with reply) sending functionality of the view model
        /// </summary>
        [TestMethod]
        public void TestSendChatMessageWithReply()
        {
            _viewModel = new ChatPageViewModel(true);
            Assert.IsTrue(_viewModel.testingMode);
            var session = new SessionInfo();
            var user1 = new UserInfo("surya", 1);
            session.Users.Add(user1);

            ReceiveChatData receivedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                ReplyThreadID = 1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            UiDispatcherHelper.ProcessUiEvents();
            _viewModel.OnUserSessionChange(session);
            UiDispatcherHelper.ProcessUiEvents();

            _viewModel.OnMessageReceived(receivedChat);

            UiDispatcherHelper.ProcessUiEvents();

            // message which is a reply to the message of MessageID=1
            string chatMsg = "Chat Message!";
            int replyMsgId = 1;
            string chatMsgType = "Chat";

            // send the message
            _viewModel.SendMessage(chatMsg, replyMsgId, chatMsgType);

            // Validation of the sent message
            Assert.AreEqual("Chat Message!", _viewModel.MsgToSend.Data);
            Assert.AreEqual(1, _viewModel.MsgToSend.ReplyThreadID);
        }

        /// <summary>
        /// Validating the file message(no reply) sending functionality of the view model
        /// </summary>
        [TestMethod]
        public void TestSendFileMessageWithNoReply()
        {
            _viewModel = new ChatPageViewModel(true);
            string currentDirectory = Directory.GetCurrentDirectory();
            
            // file message
            string[] filepath = currentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string chatMsg = filepath[0] + "\\MessengerTests\\ContentTests\\test_txt_file.txt";
            int replyMsgId = -1;
            string chatMsgType = "File";

            // send message
            _viewModel.SendMessage(chatMsg, replyMsgId, chatMsgType);

            // Validation of the sent message
            Assert.AreEqual(-1, _viewModel.MsgToSend.ReplyThreadID);
            Assert.AreEqual(chatMsg, _viewModel.MsgToSend.Data);
        }

        /// <summary>
        /// Validating the file message(with reply) sending functionality of the view model
        /// </summary>
        [TestMethod]
        public void TestSendFileMessageWithReply()
        {
            _viewModel = new ChatPageViewModel(true);
            Assert.IsTrue(_viewModel.testingMode);
            var session = new SessionInfo();
            var user1 = new UserInfo("surya", 1);
            session.Users.Add(user1);

            ReceiveChatData receivedChat = new()
            {
                Type = MessengerContent.MessageType.Chat,
                Event = MessageEvent.New,
                Data = "Yo! Surya",
                MessageID = 1,
                ReplyMessageID = -1,
                ReplyThreadID = 1,
                SenderID = 1,
                SenderName = "surya",
                SentTime = DateTime.Now,
                Starred = false
            };

            UiDispatcherHelper.ProcessUiEvents();
            _viewModel.OnUserSessionChange(session);
            UiDispatcherHelper.ProcessUiEvents();

            
            _viewModel.OnMessageReceived(receivedChat);

            UiDispatcherHelper.ProcessUiEvents();

            string currentDirectory = Directory.GetCurrentDirectory();
            string[] filepath = currentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            // message which is a reply to the message of MessageID=1
            string chatMsg = filepath[0] + "\\MessengerTests\\ContentTests\\test_txt_file.txt";
            int replyMsgId = 1;
            string chatMsgType = "File";

            // send the message
            _viewModel.SendMessage(chatMsg, replyMsgId, chatMsgType);

            // Validation of the sent message
            Assert.AreEqual(1, _viewModel.MsgToSend.ReplyThreadID);
            Assert.AreEqual(chatMsg, _viewModel.MsgToSend.Data);
        }
    }
}
