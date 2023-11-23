/******************************************************************************
 * Filename    = FileServerTests.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContentTests
 *
 * Description = Tests for FileServer
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent;
using MessengerContent.DataModels;
using MessengerContent.Enums;
using MessengerContent.Server;

namespace MessengerTests.ContentTests.Server
{
    [TestClass]
    public class FileServerTests
    {
        private ContentDataBase _database;
        private FileServer _fileServer;

        public void Setup()
        {
            _database = new ContentDataBase();
            _fileServer = new FileServer(_database);
        }

        [TestMethod]
        public void Receive_StoringFile_ShouldBeAbleToStoreFile()
        {
            Setup();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? recv = _fileServer.Receive(file1);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);
        }

        [TestMethod]
        public void Receive_FetchingFile_ShouldBeAbleToFetchAStoredFile()
        {
            Setup();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? recv = _fileServer.Receive(file1);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var file = new ChatData
            {
                Data = "Test_File.pdf",
                SenderID = 1,
                MessageID = recv.MessageID,
                Type = MessageType.File,
                Event = MessageEvent.Download
            };

            recv = _fileServer.Receive(file);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.MessageID, recv.MessageID);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(MessageEvent.Download, recv.Event);
            Assert.AreEqual(file1.FileData.Size, recv.FileData.Size);
            Assert.AreEqual(file1.FileData.Name, recv.FileData.Name);
            Assert.AreEqual(file1.FileData.Data, recv.FileData.Data);
            Assert.AreEqual(file1.ReplyThreadID, recv.ReplyThreadID);
        }

        [TestMethod]
        public void Receive_GivingInvalidEventForFileType_NullShouldBeReturned()
        {
            Setup();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? recv = _fileServer.Receive(file1);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var file = new ChatData
            {
                MessageID = 0,
                Type = MessageType.File,
                Event = MessageEvent.Star
            };

            recv = _fileServer.Receive(file);
            Assert.IsNull(recv);

            file = new ChatData
            {
                MessageID = 0,
                Type = MessageType.File,
                Event = MessageEvent.Edit
            };

            recv = _fileServer.Receive(file);
            Assert.IsNull(recv);
        }

        [TestMethod]
        public void Receive_FetchingAFilesThatDoesNotExist_NullShouldBeReturned()
        {
            Setup();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? recv = _fileServer.Receive(file1);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var file = new ChatData
            {
                MessageID = 10,
                Type = MessageType.File,
                Event = MessageEvent.Download
            };

            recv = _fileServer.Receive(file);
            Assert.IsNull(recv);
        }

        [TestMethod]
        public void Receive_StoringAndFetchingMultipleFiles_ShouldBeAbleToStoreFilesAndFetchThem()
        {
            Setup();
            string CurrentDirectory = Directory.GetCurrentDirectory();
            string[] path = CurrentDirectory.Split(new[] { "\\MessengerTests" }, StringSplitOptions.None);
            string pathA = path[0] + "\\MessengerTests\\ContentTests\\TestFile.txt";
            string pathB = path[0] + "\\MessengerTests\\ContentTests\\MockCommunicator.cs";

            var file1 = new ChatData
            {
                Data = "Test_File.pdf",
                Type = MessageType.File,
                FileData = new SendFileData(pathA),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            ChatData? recv = _fileServer.Receive(file1);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var file2 = new ChatData
            {
                Data = "Utility.cs",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = -1,
                Event = MessageEvent.New
            };

            recv = _fileServer.Receive(file2);

            Assert.AreEqual(file2.Data, recv.Data);
            Assert.AreNotEqual(recv.MessageID, file1.MessageID);
            Assert.AreNotEqual(recv.ReplyThreadID, file1.ReplyThreadID);
            Assert.AreEqual(file2.Type, recv.Type);
            Assert.AreEqual(file2.SenderID, recv.SenderID);
            Assert.AreEqual(file2.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            var file3 = new ChatData
            {
                Data = "c.txt",
                Type = MessageType.File,
                FileData = new SendFileData(pathB),
                SenderID = 1,
                ReplyThreadID = file1.ReplyThreadID,
                Event = MessageEvent.New
            };

            recv = _fileServer.Receive(file3);

            Assert.AreEqual(file3.Data, recv.Data);
            Assert.AreNotEqual(recv.MessageID, file1.MessageID);
            Assert.AreNotEqual(recv.MessageID, file2.MessageID);
            Assert.AreEqual(recv.ReplyThreadID, file1.ReplyThreadID);
            Assert.AreEqual(file3.Type, recv.Type);
            Assert.AreEqual(file3.SenderID, recv.SenderID);
            Assert.AreEqual(file3.Event, recv.Event);
            Assert.IsNull(recv.FileData);

            file1.Event = MessageEvent.Download;
            recv = _fileServer.Receive(file1);

            Assert.AreEqual(file1.Data, recv.Data);
            Assert.AreEqual(file1.MessageID, recv.MessageID);
            Assert.AreEqual(file1.Type, recv.Type);
            Assert.AreEqual(file1.SenderID, recv.SenderID);
            Assert.AreEqual(file1.Event, recv.Event);
            Assert.AreEqual(file1.FileData.Size, recv.FileData.Size);
            Assert.AreEqual(file1.FileData.Name, recv.FileData.Name);
            Assert.AreEqual(file1.FileData.Data, recv.FileData.Data);
            Assert.AreEqual(file1.ReplyThreadID, recv.ReplyThreadID);

            file2.Event = MessageEvent.Download;
            recv = _fileServer.Receive(file2);

            Assert.AreEqual(file2.Data, recv.Data);
            Assert.AreEqual(file2.MessageID, recv.MessageID);
            Assert.AreEqual(file2.Type, recv.Type);
            Assert.AreEqual(file2.SenderID, recv.SenderID);
            Assert.AreEqual(file2.Event, recv.Event);
            Assert.AreEqual(file2.FileData.Size, recv.FileData.Size);
            Assert.AreEqual(file2.FileData.Name, recv.FileData.Name);
            Assert.AreEqual(file2.FileData.Data, recv.FileData.Data);
            Assert.AreEqual(file2.ReplyThreadID, recv.ReplyThreadID);

            file3.Event = MessageEvent.Download;
            recv = _fileServer.Receive(file3);

            Assert.AreEqual(file3.Data, recv.Data);
            Assert.AreEqual(file3.MessageID, recv.MessageID);
            Assert.AreEqual(file3.Type, recv.Type);
            Assert.AreEqual(file3.SenderID, recv.SenderID);
            Assert.AreEqual(file3.Event, recv.Event);
            Assert.AreEqual(file3.FileData.Size, recv.FileData.Size);
            Assert.AreEqual(file3.FileData.Name, recv.FileData.Name);
            Assert.AreEqual(file3.FileData.Data, recv.FileData.Data);
            Assert.AreEqual(file3.ReplyThreadID, recv.ReplyThreadID);
        }
    }
}
