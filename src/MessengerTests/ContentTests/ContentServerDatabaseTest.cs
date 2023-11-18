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

namespace MessengerTests.ContentTests
{
    public class ContentServerDatabaseTest
    {
        private MockHelper mockhelper = new MockHelper();
        private ContentDataBase MessageDatabase = new ContentDataBase();

        [TestMethod]
        public void FilesFetch_StoringAndFetchingAFileFromMessageDatabase_FetchAppropriateFile()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();
            var path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var receivedMsg = MessageDatabase.FileStore(file1);
            Assert.Same(file1.Data, receivedMsg.Data);

            receivedMsg = MessageDatabase.FilesFetch(file1.MessageID);
            Assert.Same(file1.Data, receivedMsg.Data);

        }

        [TestMethod]
        public void FilesFetch_FetchAFileThatDoesNotExist_ReturnsNull()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();

            var path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var receivedMsg = MessageDatabase.FileStore(file1);
            Assert.Same(file1.Data, receivedMsg.Data);

            receivedMsg = MessageDatabase.FilesFetch(20);
            Assert.Null(receivedMsg);
        }

        [TestMethod]
        public void FileStore_StoringMultipleFiles_ShouldBeAbleToStoreAndFetchMultipleFiles()
        {
            var CurrentDirectory = Directory.GetCurrentDirectory();

            var path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            var pathA = path[0] + "\\MessengerTests\\ContentTests\\Test_File.pdf";
            var pathB = path[0] + "\\MessengerTests\\ContentTests\\MockHelper.cs";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            var receivedMsg = MessageDatabase.FileStore(file1);

            var file2 = new ChatData
            {
                Data = "MockHelper.cs",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            receivedMsg = MessageDatabase.FileStore(file2);

            var file3 = new ChatData
            {
                Data = "c.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = file1.ReplyThreadID,
                Event = MessageEvent.New
            };

            receivedMsg = MessageDatabase.FileStore(file3);
            Assert.Same(file3.Data, receivedMsg.Data);

            receivedMsg = MessageDatabase.FilesFetch(file1.MessageID);
            Assert.Same(file1.Data, receivedMsg.Data);

            receivedMsg = MessageDatabase.FilesFetch(file2.MessageID);
            Assert.Same(file2.Data, receivedMsg.Data);

            receivedMsg = MessageDatabase.FilesFetch(file3.MessageID);
            Assert.Same(file3.Data, receivedMsg.Data);
        }

        [TestMethod]
        public void GetChatContexts_ObtainChatThreads_returnsTrue()
        {
            List<ChatThread> allChatThreads = MessageDatabase.GetChatThreads();
            List<ChatThread> allChatThreads2 = MessageDatabase.GetChatThreads();
            Assert.Same(allChatThreads, allChatThreads2);
        }

        [TestMethod]
        public void MessageStore_StoringMultipleMessages_StoreAndFetchMultipleMessages()
        {
            var message1 = mockhelper.GenerateChatData(data: "Test Message", senderID: 1);
            var receivedMsg = MessageDatabase.MessageStore(message1);
            Assert.Same(message1.Data, receivedMsg.Data);

            var message2 = mockhelper.GenerateChatData(data: "Test Message2", senderID: 1, replyThreadID: message1.ReplyThreadID);
            receivedMsg = MessageDatabase.MessageStore(message2);
            Assert.Same(message2.Data, receivedMsg.Data);

            var message3 = mockhelper.GenerateChatData(data: "Test Message3", senderID: 1);
            receivedMsg = MessageDatabase.MessageStore(message3);
            Assert.Same(message3.Data, receivedMsg.Data);

            var msg = MessageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);
            Assert.Same(message1.Data, msg.Data);

            msg = MessageDatabase.GetMessage(message2.ReplyThreadID, message2.MessageID);
            Assert.Same(message2.Data, msg.Data);

            msg = MessageDatabase.GetMessage(message3.ReplyThreadID, message3.MessageID);
            Assert.Same(message3.Data, msg.Data);
        }

        [TestMethod]
        public void GetMessage_StoringAndFetchingAMessag_FetchStoredMessage()
        {
            var message1 = mockhelper.GenerateChatData(data: "Test Message", senderID: 1);
            var receivedMsg = MessageDatabase.MessageStore(message1);
            Assert.Same(message1.Data, receivedMsg.Data);
            Assert.Null(receivedMsg.FileData);

            var msg = MessageDatabase.GetMessage(message1.ReplyThreadID, message1.MessageID);
            Assert.Equal(message1.Data, msg.Data);
            Assert.Equal(msg.MessageID, message1.MessageID);
            Assert.Equal(message1.Type, msg.Type);
            Assert.Equal(message1.SenderID, msg.SenderID);
            Assert.Equal(message1.Event, msg.Event);
            Assert.Equal(message1.ReplyThreadID, msg.ReplyThreadID);

        }
    }
}
