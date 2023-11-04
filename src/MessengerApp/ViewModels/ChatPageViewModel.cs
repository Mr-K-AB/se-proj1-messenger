using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.ComponentModel;
using MessengerContent.DataModels;
using System.Windows.Threading;
using System.Windows;
using MessengerContent;
using MessengerContent.Client;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.ViewModels;

namespace MessengerApp.ViewModels
{

    public class ChatPageViewModel ///: INotifyPropertyChanged, INotificationListener, IClientSessionNotifications
    {
        public IDictionary<int, string> Users;
        public IDictionary<int, string> Messages;
        public IDictionary<int, int> ThreadIds;

        ///public SendContentData MsgToSend { get; private set; }
    }
}


/*
/// <summary>
        ///     Content Client Data Model
        /// </summary>
        private readonly IContentClient _model;

/// <summary>
///     Dictionary mapping Messages IDs to their ThreadIds
/// </summary>
public IDictionary<int, int> ThreadIds;


/// <summary>
///     Constructor for ViewModel
/// </summary>
/// <param name="production">true for production mode</param>
public ChatPageViewModel()
{
    Trace.WriteLine("[ChatPageViewModel] ViewModel setup");
    ThreadIds = new Dictionary<int, int>();
}

/// <summary>
///     Message to be sent
/// </summary>
public SendContentData MsgToSend { get; private set; }

/// <summary>
///     Sends the message to content module. Message type is determined by messageType parameter
/// </summary>
/// <param name="message"> The string containing the file path </param>
/// <param name="replyMsgId"> Either the reply ID of the mesage being replied to or -1 denoting its not a reply message </param>
/// <param name="messageType"> File or Chat </param>
public void SendMessage(string message, int replyMsgId, string messageType)
{

    // Creating a SendContentData object
    MsgToSend = new SendContentData();
    // Setting message type field
    if (messageType == "File")
    {
        MsgToSend.Type = MessageType.File;
    }
    else if (messageType == "Chat")
    {
        MsgToSend.Type = MessageType.Chat;
    }

    // Setting the remaining fields of the SendContentData object
    MsgToSend.ReplyMessageID = replyMsgId;
    MsgToSend.Data = message;
    MsgToSend.ReplyThreadID = replyMsgId != -1 ? ThreadIds[replyMsgId] : -1;

    // Empty list denotes it's broadcast message
    MsgToSend.ReceiverIDs = new int[] { };

    if (messageType == "File")
    {
        Trace.WriteLine("[ChatPageViewModel] I am Sending a File Message");
    }
    else if (messageType == "Chat")
    {
        Trace.WriteLine("[ChatPageViewModel] I am Sending a Chat Message");
    }
    _model.ClientSendData(MsgToSend); 
        }*/
