/******************************************************************************
 * Filename    = FileClientTests.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerTests
 *
 * Description = tests
 *****************************************************************************/

using MessengerContent.Client;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent;

namespace MessengerTests.ContentTests.Client
{
    [TestClass]
    public class FileClientTests
    {
        [TestMethod]
        public void SendFile_ValidInput_ReturnsValidChatData()
        {
            // send valid file message and deserialize it from mock communicator
            var utility = new MockHelper();
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] pathArray = currentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string path = pathArray[0] + "\\MessengerTests\\ContentTests\\MockHelper.cs";
            int userID = 5;
            SendChatData sendChatData = utility.GenerateSendChatData(MessageType.File, path);
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            IContentSerializer serializer = new ContentSerializer();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };
            var fileData = new SendFileData(path);

            fileClient.SendFile(sendChatData);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            //Assert.IsType<ChatData>(deserializedData);
            Assert.AreEqual(sendChatData.Type, deserializedData.Type);
            Assert.AreEqual(sendChatData.Data, deserializedData.Data);
            Assert.AreEqual(sendChatData.ReplyThreadID, deserializedData.ReplyThreadID);
            //returns bytes so we need to convert them to string to use AreEqual
            Assert.AreEqual(System.Convert.ToBase64String(fileData.Data), System.Convert.ToBase64String(deserializedData.FileData.Data));
            Assert.AreEqual(fileData.Name, deserializedData.FileData.Name);
            Assert.AreEqual(fileData.Size, deserializedData.FileData.Size);
            Assert.AreEqual(userID, deserializedData.SenderID);
            Assert.AreEqual(MessageEvent.New, deserializedData.Event);
        }

        [TestMethod]
        public void SendFile_InvalidMessageType_ReturnsArgumentException()
        {
            var utility = new MockHelper();
            int userID = 5;
            SendChatData sendChatData = utility.GenerateSendChatData(MessageType.Chat, "");
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<ArgumentException>(() => fileClient.SendFile(sendChatData));
        }

        [TestMethod]
        public void SendFile_EmptyFilePath_ReturnsFileNotFoundException()
        {
            var utility = new MockHelper();
            int userID = 5;
            SendChatData sendChatData = utility.GenerateSendChatData(MessageType.File, "");
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<FileNotFoundException>(() => fileClient.SendFile(sendChatData));
        }

        [TestMethod]
        public void SendFile_NullFilePath_ReturnsFileNotFoundException()
        {
            var utility = new MockHelper();
            int userID = 5;
            SendChatData sendChatData = utility.GenerateSendChatData(MessageType.File, null);
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<FileNotFoundException>(() => fileClient.SendFile(sendChatData));
        }

        [TestMethod]
        public void DownloadFile_ValidInput_ReturnsValidChatData()
        {
            var utility = new MockHelper();
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] pathArray = currentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string path = pathArray[0] + "\\MessengerTests\\ContentTests\\MockHelper.cs";
            int userID = 5;
            int messageID = 6;
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            IContentSerializer serializer = new ContentSerializer();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            fileClient.DownloadFile(messageID, path);
            string serializedData = mockCommunicator.GetSendData();
            ChatData deserializedData = serializer.Deserialize<ChatData>(serializedData);

            //Assert.IsType<ChatData>(deserializedData);
            Assert.AreEqual(MessageType.File, deserializedData.Type);
            Assert.AreEqual(path, deserializedData.Data);
            Assert.AreEqual(userID, deserializedData.SenderID);
            Assert.AreEqual(messageID, deserializedData.MessageID);
            Assert.AreEqual(MessageEvent.Download, deserializedData.Event);
        }

        [TestMethod]
        public void DownloadFile_EmptySavePath_ReturnsFileNotFoundException()
        {
            var utility = new MockHelper();
            int userID = 5;
            int messageID = 6;
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<ArgumentException>(() => fileClient.DownloadFile(messageID, ""));
        }

        [TestMethod]
        public void DownloadFile_NullSavePath_ReturnsFileNotFoundException()
        {
            var utility = new MockHelper();
            int userID = 5;
            int messageID = 6;
            MockCommunicator mockCommunicator = utility.GetMockCommunicator();
            var fileClient = new FileClient(mockCommunicator)
            {
                UserID = userID,
                Communicator = mockCommunicator
            };

            Assert.ThrowsException<ArgumentException>(() => fileClient.DownloadFile(messageID, null));
        }

    }
}
