﻿/******************************************************************************
 * Filename    = ChatServer.cs
 *
 * Author      = 
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = 
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
        private readonly ContentDB _contentDB;

        /// <summary>
        ///     Constructor to initialize the content Database.
        /// </summary>
        public ChatServer(ContentDB db)
        {
            _contentDB = db;
        }

        /// <summary>
        ///     This function returns all the messages stored.
        /// </summary>
        public List<ChatThread> GetMessages()
        {
            return _contentDB.GetChatThreads();
        }

        /// <summary>
        ///     This event is used to process the chat based on the type of event that occurred.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>Returns the new message</returns>
        public ChatData Receive(ChatData msg)
        {
            ReceiveChatData receivedMsg;
            Trace.WriteLine("[ChatServer] Received message from ContentServer");
            if (msg.Event == MessageEvent.New)
            {
                Trace.WriteLine("[ChatServer] Event is NewMessage, Adding message to existing Thread");
                return _contentDB.MessageStore(msg);
            }
            else if (msg.Event == MessageEvent.Star)
            {
                Trace.WriteLine("[ChatServer] Event is Star, Starring message in existing Thread");
                receivedMsg = StarMessage(msg.ReplyThreadID, msg.MessageID);
            }
            else if (msg.Event == MessageEvent.Edit)
            {
                Trace.WriteLine("[ChatServer] Event is Update, Updating message in existing Thread");
                receivedMsg = UpdateMessage(msg.ReplyThreadID, msg.MessageID,
                    msg.Data);
            }
            else if (msg.Event == MessageEvent.Delete)
            {
                Trace.WriteLine("[ChatServer] Event is Update, Updating message in existing Thread");
                receivedMsg = DeleteMessage(msg.ReplyThreadID, msg.MessageID);
            }
            else
            {
                Trace.WriteLine($"[ChatServer] invalid event");
                return null;
            }
            if (receivedMsg == null)
            {
                return null;
            }
            //Create a MessageData object and return this notify object.
            var notifyMsgData = new ChatData(receivedMsg)
            {
                Event = msg.Event
            };
            return notifyMsgData;
        }

        /// <summary>
        ///     This function is used to update a message with a new updated message.
        /// </summary>
        public ReceiveChatData UpdateMessage(int replyId, int _msgId, string updatedMsg)
        {
            var message = _contentDB.GetMessage(replyId, _msgId);

            //message doesn't exists in database, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // Update the message and return the updated message
            message.Data = updatedMsg;
            return message;
        }

        /// <summary>
        ///     This function is used to star a message.
        /// </summary>
        public ReceiveChatData StarMessage(int replyId, int _msgId)
        {
            var msg = _contentDB.GetMessage(replyId, _msgId);

            //message doesn't exists in database, return null
            if (msg == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // Star the message and return the starred message
            msg.Starred = !msg.Starred;
            return msg;
        }

        /// <summary>
        ///     This function is used to Delete a message.
        /// </summary>
        public ReceiveChatData DeleteMessage(int replyId, int _msgId)
        {
            var message = _contentDB.GetMessage(replyId, _msgId);

            // Message doesn't exists in database, return null
            if (message == null)
            {
                Trace.WriteLine($"[ChatServer] Message not found replyThreadID: {replyId}, messageId: {_msgId}.");
                return null;
            }

            // The data of the message now becomes "Message Deleted.".
            message.Data = "Message Deleted.";
            return message;
        }

    }
}
