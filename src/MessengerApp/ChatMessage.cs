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

namespace MessengerApp
{
    class ChatMessage
    {
        public int MessageID { get; set; }

        public string? Sender { get; set; }

        public string Time { get; set; }

        public bool MessageType { get; set; }

        public string? IncomingMessage { get; set; }

        public string? ReplyMessage { get; set; }

        public bool isCurrentUser { get; set; }
        public ChatMessage()
        {
            MessageID = -1;
            Sender = null;
            Time = DateTime.Now.ToShortTimeString();
            MessageType = true;
            ReplyMessage = null;
            IncomingMessage = null;
            isCurrentUser = false;
        }
    }
}
