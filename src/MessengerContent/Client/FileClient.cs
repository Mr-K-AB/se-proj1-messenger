/******************************************************************************
 * Filename    = FileClient.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Handles the client side of sending and downloading files
 *****************************************************************************/
using MessengerContent.DataModels;
using System.Diagnostics;
using System;
using System.IO;
using MessengerContent.Enums;
using MessengerNetworking.Communicator;

namespace MessengerContent.Client
{
    public class FileClient
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
        public FileClient(ICommunicator communicator)
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

        /// <summary>
        /// Serializes the ChatData object and send it to the server via networking module. 
        /// </summary>
        /// <param name="
        /// ">Instance of SendChatData class</param>
        /// <param name="eventType">Type of message event as string</param>
        private void SerializeAndSendToServer(ChatData chatData, string eventType)
        {
            try
            {
                string xml = _serializer.Serialize(chatData);
                Trace.WriteLine($"[File Client] Setting event as '{eventType}' and sending object to server.");
                _communicator.Send(xml, _moduleIdentifier, null);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[File Client] Exception occurred while sending object.\n{e.GetType().Name} : {e.Message}");
            }
        }

        /// <summary>
        /// Converts the input SendChatData object, sets the event type as New, serializes and send it to server.
        /// </summary>
        /// <param name="sendContent">Instance of the SendChatData class</param>
        /// <exception cref="ArgumentException"></exception>
        public void SendFile(SendChatData sendContent)
        {
            // check message type
            if (sendContent.Type != MessageType.File)
            {
                throw new ArgumentException("Message is not of type - 'File'");
            }
            // check if file exists
            if (!File.Exists(sendContent.Data))
            {
                throw new FileNotFoundException($"File at {sendContent.Data} not found");
            }
            string[] path = sendContent.Data.Split(new char[] { '\\' }, StringSplitOptions.None);
            ChatData sendData = new()
            {
                Type = sendContent.Type,
                Data = path.Last(),
                MessageID = -1,
                //ReceiverIDs = sendContent.ReceiverIDs,
                ReplyThreadID = sendContent.ReplyThreadID,
                ReplyMessageID = sendContent.ReplyMessageID,
                SenderID = UserID,
                SenderName = UserName,
                SentTime = DateTime.Now,
                Starred = false,
                Event = MessageEvent.New,
                FileData = new SendFileData(sendContent.Data)
            };
            SerializeAndSendToServer(sendData, "New");
        }

        /// <summary>
        /// Creates ChatData object, sets the event type as Download, serializes and sends it to server.
        /// </summary>
        /// <param name="messageID">ID of the message</param>
        /// <param name="savePath">Path to which the file will be downloaded</param>
        public void DownloadFile(int messageID, string savePath)
        {
            // check for savePath
            if (savePath == null || savePath == "")
            {
                throw new ArgumentException("Invalid save path input argument");
            }
            ChatData sendData = new()
            {
                Type = MessageType.File,
                Data = savePath,
                MessageID = messageID,
                SenderID = UserID,
                Event = MessageEvent.Download,
                FileData = null
            };
            SerializeAndSendToServer(sendData, "Download");
        }
    }
}
