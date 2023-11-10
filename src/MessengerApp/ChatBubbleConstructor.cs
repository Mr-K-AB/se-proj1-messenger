﻿/******************************************************************************
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

namespace MessengerApp
{
    public class ChatBubbleConstructor : DataTemplateSelector
    {
        /// <summary>
        /// Data Template for sent chat bubble.
        /// </summary>
        public DataTemplate? SentChatMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for received chat bubble.
        /// </summary>
        public DataTemplate? ReceivedChatMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for sent file bubble.
        /// </summary>
        public DataTemplate? SentFileMessageTemplate { get; set; }

        /// <summary>
        /// Data Template for received file bubble.
        /// </summary>
        public DataTemplate? ReceivedFileMessageTemplate { get; set; }

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
                return message.MessageType ? SentChatMessageTemplate : SentFileMessageTemplate;
            }
            else
            {
                return message.MessageType ? ReceivedChatMessageTemplate : ReceivedFileMessageTemplate;
            }
        }
    }
}
