/******************************************************************************
 * Filename    = ChatServer.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = To handle chat messages and its functionalities
 *****************************************************************************/

using MessengerContent.DataModels;
using MessengerContent.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MessengerContent.Server
{
    public class ChatServer
    {
        private readonly ContentDataBase _contentDataBase;

        /// <summary>
        /// Constructor to initialize the content Database.
        /// </summary>
        public ChatServer(ContentDataBase db)
        {
            _contentDataBase = db;
        }

        /// <summary>
        /// function to fetch all the messages stored in the content DB.
        /// </summary>
        public List<ChatThread> GetMessages()
        {
            return _contentDataBase.GetChatThreads();
        }

        /// <summary>
        /// function to process the chat based on event type
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>Returns the new message</returns>
        public ChatData? Receive(ChatData msg)
        {
            ReceiveChatData receivedMsg;
            Trace.WriteLine("[ChatServer] Message received from ContentServer");
            if (msg.Event == MessageEvent.New)
            {
                Trace.WriteLine("[ChatServer] MessageEvent is NewMessage, Adding message to existing Thread");
                return _contentDataBase.MessageStore(msg);
            }
            else if (msg.Event == MessageEvent.Star)
            {
                Trace.WriteLine("[ChatServer] MessageEvent is Star, Starring message in existing Thread");
                receivedMsg = StarMessage(msg.ReplyThreadID, msg.MessageID);
            }
            else if (msg.Event == MessageEvent.Edit)
            {
                Trace.WriteLine("[ChatServer] MessageEvent is Update, Updating message in existing Thread");
                receivedMsg = UpdateMessage(msg.ReplyThreadID, msg.MessageID,
                    msg.Data);
            }
            else if (msg.Event == MessageEvent.Delete)
            {
                Trace.WriteLine("[ChatServer] MessageEvent is Update, Updating message in existing Thread");
                receivedMsg = DeleteMessage(msg.ReplyThreadID, msg.MessageID);
            }
            //If messageevent is corrupted
            else
            {
                Trace.WriteLine($"[ChatServer] Invalid MessageEvent");
                return null;
            }
            if (receivedMsg == null)
            {
                return null;
            }
            //Create a ChatData object and return
            var notifyChatData = new ChatData(receivedMsg)
            {
                Event = msg.Event
            };
            return notifyChatData;
        }

        /// <summary>
        /// function to update a message with a new message.
        /// </summary>
        public ReceiveChatData? UpdateMessage(int replyId, int _msgId, string updatedMsg)
        {
            ReceiveChatData message = _contentDataBase.GetMessage(replyId, _msgId);

            //if message doesn't exists in database, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found in the given replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // Update the message and return the updated message
            message.Data = updatedMsg;
            return message;
        }

        /// <summary>
        /// function to star a message.
        /// </summary>
        public ReceiveChatData? StarMessage(int replyId, int _msgId)
        {
            ReceiveChatData msg = _contentDataBase.GetMessage(replyId, _msgId);

            //if message doesn't exists in database, return null
            if (msg == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found in the given replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // Star the message and return the starred message
            msg.Starred = !msg.Starred;
            return msg;
        }

        /// <summary>
        /// function to Delete an exsisting message.
        /// </summary>
        public ReceiveChatData? DeleteMessage(int replyId, int _msgId)
        {
            ReceiveChatData message = _contentDataBase.GetMessage(replyId, _msgId);

            // if Message doesn't exists in database, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found in the given replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // The data of the message now becomes "Message Deleted." string.
            message.Data = "Message Deleted.";
            return message;
        }
    }
}
