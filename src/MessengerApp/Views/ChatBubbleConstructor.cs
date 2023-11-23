/******************************************************************************
* Filename    = ChatBubbleConstructor.cs
*
* Author      = M V Nagasurya
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = Selects Data Templates for the chatbubbles based on whether the message is sent from user or received to user.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessengerApp.DataModel;

namespace MessengerApp.Views
{
    public class ChatBubbleConstructor : DataTemplateSelector
    {
        /// <summary>
        /// Select DataTemplate for the messages which are sent by us and received to us accordingly.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns> DataTemplate </returns>
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            var message = item as ChatMessage;
            if (message.isCurrentUser == true)
            {
                if (message.MessageType == true)
                {
                    return (message.ReplyMessage != null && message.ReplyMessage != "") ? SentChatMessageTemplate : SentChatMessageNotReplyTemplate;
                }
                return (message.ReplyMessage != null && message.ReplyMessage != "") ? SentFileMessageTemplate : SentFileMessageNotReplyTemplate;
            }
            else
            {
                if (message.MessageType == true)
                {
                    return (message.ReplyMessage != null && message.ReplyMessage != "") ? ReceivedChatMessageTemplate : ReceivedChatMessageNotReplyTemplate;
                }
                return (message.ReplyMessage != null && message.ReplyMessage != "") ? ReceivedFileMessageTemplate : ReceivedFileMessageNotReplyTemplate;
            }
        }

        /// <summary>
        /// Data Template for sent chat bubble which is a reply.
        /// </summary>
        public DataTemplate? SentChatMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for received chat bubble which is a reply.
        /// </summary>
        public DataTemplate? ReceivedChatMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for sent file bubble which is a reply.
        /// </summary>
        public DataTemplate? SentFileMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for received file bubble which is a reply.
        /// </summary>
        public DataTemplate? ReceivedFileMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for sent chat bubble which is not a reply.
        /// </summary>
        public DataTemplate? SentChatMessageNotReplyTemplate { get; set; }

        /// <summary>
        /// Data Template for received chat bubble which is not a reply.
        /// </summary>
        public DataTemplate? ReceivedChatMessageNotReplyTemplate { get; set; }

        /// <summary>
        /// Data Template for sent file bubble which is not a reply.
        /// </summary>
        public DataTemplate? SentFileMessageNotReplyTemplate { get; set; }

        /// <summary>
        /// Data Template for received file bubble which is not a reply.
        /// </summary>
        public DataTemplate? ReceivedFileMessageNotReplyTemplate { get; set; }

    }
}
