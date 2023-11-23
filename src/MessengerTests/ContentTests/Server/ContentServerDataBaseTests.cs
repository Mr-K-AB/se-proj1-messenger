/******************************************************************************
* Filename    = ContentServerDatabaseTests.cs
*
* Author      = Likhitha
*
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = Contains Tests for ContentDataBase
*****************************************************************************/

using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent.Server;
using MessengerContent;
using MessengerTests.ContentTests;


namespace MessengerTests.ContentTests
{
    public class ContentServerDatabaseTest
    {
        private readonly MockHelper _mockhelper = new();
        private readonly ContentDataBase _messageDatabase = new();

        [TestMethod]
        public void FilesFetch_StoringAndFetchingAFileFromMessageDatabase_FetchAppropriateFile()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? receivedMsg = _messageDatabase.FileStore(file1);
            Assert.AreEqual(file1.Data, receivedMsg.Data);
            receivedMsg = _messageDatabase.FilesFetch(file1.MessageID);
            if (receivedMsg != null)
            {
                Assert.AreSame(file1.Data, receivedMsg.Data);
            }


        }

        [TestMethod]
        public void FilesFetch_FetchAFileThatDoesNotExist_ReturnsNull()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();

            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? receivedMsg = _messageDatabase.FileStore(file1);
            Assert.AreSame(file1.Data, receivedMsg.Data);

            receivedMsg = _messageDatabase.FilesFetch(20);
            Assert.IsNull(receivedMsg);
        }

        [TestMethod]
        public void FileStore_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles()
        {
            string CurrentDirectory = Directory.GetCurrentDirectory();

            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";
            string pathB = path[0] + "\\MessengerTests\\ContentTests\\MockHelper.cs";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            _ = _messageDatabase.FileStore(file1);

            var file2 = new ChatData
            {
                Data = "MockHelper.cs",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };
            _ = _messageDatabase.FileStore(file2);

            var file3 = new ChatData
            {
                Data = "c.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = file1.ReplyThreadID,
                Event = MessageEvent.New
            };

            ChatData? receivedMsg = _messageDatabase.FileStore(file3);
            Assert.AreSame(file3.Data, receivedMsg.Data);

            receivedMsg = _messageDatabase.FilesFetch(file1.MessageID);
            if (receivedMsg != null)
            {
                Assert.AreSame(file1.Data, receivedMsg.Data);
            }


            receivedMsg = _messageDatabase.FilesFetch(file2.MessageID);
            if (receivedMsg != null)
            {
                Assert.AreSame(file2.Data, receivedMsg.Data);
            }

            receivedMsg = _messageDatabase.FilesFetch(file3.MessageID);
            if (receivedMsg != null)
            {
                Assert.AreSame(file3.Data, receivedMsg.Data);
            }
        }

        [TestMethod]
        public void GetChatContexts_ObtainChatThreads_returnsTrue()
        {
            List<ChatThread> allChatThreads = _messageDatabase.GetChatThreads();
            List<ChatThread> allChatThreads2 = _messageDatabase.GetChatThreads();
            Assert.AreSame(allChatThreads, allChatThreads2);
        }

        [TestMethod]
        public void MessageStore_StoringMultipleMessages_StoreAndFetchMultipleMessages()
        {
            ChatData message1 = _mockhelper.GenerateChatData(data: "Test Message", senderID: 1);
            ChatData receivedMsg = _messageDatabase.MessageStore(message1);
            Assert.AreSame(message1.Data, receivedMsg.Data);

            ChatData message2 = _mockhelper.GenerateChatData(data: "Test Message2", senderID: 1, replyThreadID: message1.ReplyThreadID);
            receivedMsg = _messageDatabase.MessageStore(message2);
            Assert.AreSame(message2.Data, receivedMsg.Data);

            ChatData message3 = _mockhelper.GenerateChatData(data: "Test Message3", senderID: 1);
            receivedMsg = _messageDatabase.MessageStore(message3);
            Assert.AreSame(message3.Data, receivedMsg.Data);

            ReceiveChatData? msg = _messageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);
            Assert.AreSame(message1.Data, msg.Data);

            msg = _messageDatabase.GetMessage(message2.ReplyThreadID, message2.MessageID);
            Assert.AreSame(message2.Data, msg.Data);

            msg = _messageDatabase.GetMessage(message3.ReplyThreadID, message3.MessageID);
            Assert.AreSame(message3.Data, msg.Data);
        }

        [TestMethod]
        public void GetMessage_StoringAndFetchingAMessag_FetchStoredMessage()
        {
            ChatData message1 = _mockhelper.GenerateChatData(data: "Test Message", senderID: 1);
            ChatData receivedMsg = _messageDatabase.MessageStore(message1);
            Assert.AreSame(message1.Data, receivedMsg.Data);
            Assert.IsNull(receivedMsg.FileData);

            ReceiveChatData? msg = _messageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);
            Assert.AreEqual(message1.Data, msg.Data);
            Assert.AreEqual(msg.MessageID, message1.MessageID);
            Assert.AreEqual(message1.Type, msg.Type);
            Assert.AreEqual(message1.SenderID, msg.SenderID);
            Assert.AreEqual(message1.Event, msg.Event);
            Assert.AreEqual(message1.ReplyThreadID, msg.ReplyThreadID);

        }
    }
}
