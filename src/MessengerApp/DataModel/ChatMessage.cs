/******************************************************************************
* Filename    = ChatMessage.cs
*
* Author      = M V Nagasurya
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = The following implementation is the Message datatype to store the message metadata.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerApp.DataModel
{
    public class ChatMessage
    {
        /// <summary>
        /// Message ID
        /// </summary>
        public int MessageID { get; set; }

        /// <summary>
        /// Name of the message Sender
        /// </summary>
        public string? Sender { get; set; }

        /// <summary>
        /// Time when the message is sent
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Type of the message, whether it is Chat or a File.
        /// True if the Type is Chat, else False
        /// </summary>
        public bool MessageType { get; set; }

        /// <summary>
        /// Stores the Message String of the chat, or the name of the file
        /// </summary>
        public string? MsgData { get; set; }

        /// <summary>
        /// If the message is a reply to some other message, then it stores that replied message
        /// Else, it stores NULL
        /// </summary>
        public string? ReplyMessage { get; set; }

        /// <summary>
        /// If the message sent by the current user, then value is True. Else it is False
        /// </summary>
        public bool isCurrentUser { get; set; }
        public ChatMessage()
        {
            MessageID = -1;
            Sender = null;
            Time = DateTime.Now.ToShortTimeString();
            MessageType = true;
            ReplyMessage = null;
            MsgData = null;
            isCurrentUser = false;
        }
    }
}
