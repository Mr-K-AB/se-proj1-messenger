/******************************************************************************
* Filename    = FileServer.cs
*
* Author      = Manikanta Gudipudi
* 
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = This class handles various functionalities associated with the files.
*****************************************************************************/

using MessengerContent.DataModels;
using MessengerContent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraceLogger;

namespace MessengerContent.Server
{
    public class FileServer
    {
        private readonly ContentDataBase _db;

        /// <summary>
        /// Constructor to initializes the content Database.
        /// </summary>
        public FileServer(ContentDataBase contentDB)
        {
            _db = contentDB;
        }

        /// <summary>
        /// This function is used to preocess the file based on the type of event occured.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the new message</returns>
        public ChatData? Receive(ChatData msg)
        {
            Logger.Log("[FileServer] Message received from ContentServer", LogLevel.INFO);
            if (msg.Event == MessageEvent.New)
            {
                Logger.Log("[FileServer] MessageEvent is New, Saving File", LogLevel.INFO);
                return StoreFile(msg);
            }
            else if (msg.Event == MessageEvent.Download)
            {
                Logger.Log("[FileServer] MessageEvent is Download, Proceeding to download", LogLevel.INFO);
                return FileDownload(msg);
            }
            else
            {
                Logger.Log($"[ChatServer] Invalid MessageEvent", LogLevel.WARNING);
                return null;
            }
        }

        /// <summary>
        /// function to save file on Database.
        /// </summary>
        public ChatData StoreFile(ChatData msg)
        {
            msg = _db.FileStore(msg).Copy();
            // the object is going to be typecasted to ReceiveChatData
            // to be sent to clients, so make filedata null because the filedata
            // will continue to be in memory despite the typecasting
            msg.FileData = null;
            return msg;
        }

        /// <summary>
        /// This function is used to download the file on download event.
        /// </summary>
        public ChatData? FileDownload(ChatData msg)
        {
            ChatData? receivedMsg = _db.FilesFetch(msg.MessageID);
            // if doesn't exist on database, return null
            if (receivedMsg == null)
            {
                Logger.Log($"[FileServer] No File found with given messageId: {msg.MessageID}.", LogLevel.WARNING);
                return null;
            }
            // Clone the object and add the required fields
            ChatData downloadedFile = receivedMsg.Copy();
            // store file path on which the file will be downloaded on the client's system
            downloadedFile.Data = msg.Data;
            downloadedFile.Event = MessageEvent.Download;
            downloadedFile.SenderID = msg.SenderID;
            return downloadedFile;
        }
    }
}
