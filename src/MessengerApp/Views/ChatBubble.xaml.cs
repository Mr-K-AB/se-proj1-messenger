/******************************************************************************
* Filename    = ChatBubble.xaml.cs
*
* Author      = M V Nagasurya
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = Helper functions
*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Maps;
using MessengerApp.DataModel;
using MessengerApp.ViewModels;
using MessengerContent;
using MessengerContent.DataModels;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using TraceLogger;

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for ChatBubble.xaml
    /// </summary>
    public partial class ChatBubble : UserControl
    {
        // All the messages are added into this Collection.
        // Whenever a message is received from the ViewModel, this collection gets updated which inturn updates the UI.
        private readonly ObservableCollection<ChatMessage> _msgCollection;

        public ChatBubble()
        {
            InitializeComponent();

            var viewModel = new ChatPageViewModel();
            viewModel.PropertyChanged += MsgListener; // Subscribe to PropertyChangedEvent
            DataContext = viewModel;

            _msgCollection = new ObservableCollection<ChatMessage>();

            MainChat.ItemsSource = _msgCollection; // Binding all the messages to the MainChat (ListBox)
        }

        /// <summary>
        ///     Replied message's Message ID 
        /// </summary>
        public int ReplyMsgId
        {
            get;
            set;
        }

        /// <summary>
        /// ReplyToggleButton reference for enabling toggling.
        /// If we press on the reply button, msg will appear on the ReplyTextBox
        /// If we toggle it, ReplytextBox will become clear
        /// </summary>
        public ToggleButton ReplyObject
        {
            get;
            set;
        }

        /// <summary>
        ///     Updates the display with new messages
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> PropertyChangedEvent </param>
        private void MsgListener(object? sender, PropertyChangedEventArgs e)
        {

            string? propertyName = e.PropertyName; // Changed Property Name
            var viewModel = DataContext as ChatPageViewModel;

            if (propertyName == "NewReceivedMsg") // When the user receive a new message
            {
                if (viewModel.ReceivedMsg.ReplyMessage == "")
                {
                    viewModel.ReceivedMsg.ReplyMessage = null;
                }
                Logger.Log("[ChatBubble] New message received", LogLevel.INFO);
                if(viewModel.ReceivedMsg.ReplyMessage != null)
                {
                    viewModel.ReceivedMsg.ReplyMessage = "@ "+ viewModel.ReceivedMsg.Sender + ": " + viewModel.ReceivedMsg.ReplyMessage;
                }
                _msgCollection.Add(viewModel.ReceivedMsg); // Adding the received message into the collection (_msgCollection)
                UpdateScrollBar(MainChat);
            }
            else if (propertyName == "HistoryMsgs")
            {
                // Adding all the messages for the new user to our collection(_msgCollection)
                if (viewModel.ReceivedMsg.ReplyMessage == "")
                {
                    viewModel.ReceivedMsg.ReplyMessage = null;
                }
                Logger.Log("[ChatBubble] Chat History received", LogLevel.INFO);
                _msgCollection.Add(viewModel.ReceivedMsg);
                UpdateScrollBar(MainChat);
            }
            else if (propertyName == "Edited/Deleted")
            {
                
                string replyMsgUpd = ""; // for storing the edited message
                ChatMessage? result = null;

                for (int i = 0; i < _msgCollection.Count; i++) // find the msg which has been triggered
                {
                    ChatMessage message = _msgCollection[i];
                    if (message.MessageID == viewModel.ReceivedMsg.MessageID)
                    {
                        
                        replyMsgUpd = viewModel.ReceivedMsg.MsgData;
                        ChatMessage updatedMessage = new()
                        {
                            isCurrentUser = message.isCurrentUser,
                            ReplyMessage = message.ReplyMessage,
                            Sender = message.Sender,
                            Time = message.Time,
                            MessageID = message.MessageID,
                            MessageType = message.MessageType,
                            MsgData = replyMsgUpd,
                            ReplyMessageId = message.ReplyMessageId,
                        };

                        _msgCollection[i] = updatedMessage; // Updating the messageCollection to automatically update in the UI
                        result = _msgCollection[i];
                    }
                }

                //Find all the messages which have replied to the modified message, and change that message in that bubble
                for (int i = 0; i < _msgCollection.Count; i++)
                {
                    ChatMessage message = _msgCollection[i];
                    
                    if (message.ReplyMessageId == result.MessageID)
                    {
                        
                        ChatMessage updatedMessage = new()
                        {
                            isCurrentUser = message.isCurrentUser,
                            ReplyMessage = "@ " + message.Sender + ": " + replyMsgUpd,
                            Sender = message.Sender,
                            Time = message.Time,
                            MessageID = message.MessageID,
                            MessageType = message.MessageType,
                            MsgData = message.MsgData,
                            ReplyMessageId = message.ReplyMessageId,
                        };

                        _msgCollection[i] = updatedMessage; // updating all the messages which replied to the message which is modified
                    }
                }
                Logger.Log($"[ChatBubble] {result.MessageID}'s data has been changed to {result.MsgData}", LogLevel.INFO);
            }

            else if (propertyName == "Starred")
            {
                int indOfMsg = 0;
                for (int i = 0; i < _msgCollection.Count; i++) // find the msg which has been triggered
                {
                    ChatMessage message = _msgCollection[i];
                    if (message.MessageID == viewModel.ReceivedMsg.MessageID)
                    {
                        indOfMsg = i;
                    }
                }
                ListBoxItem item = (ListBoxItem)MainChat.ItemContainerGenerator.ContainerFromIndex(indOfMsg);
                if (item != null)
                {
                    List<ToggleButton> textBlocks = FindVisualChildren<ToggleButton>(item);
                    ToggleButton toggleBtn = textBlocks[1];
                    if (toggleBtn != null)
                    {
                        toggleBtn.IsChecked = true;
                        
                    }
                }
            }
            else if(propertyName == "NotStarred")
            {
                int indOfMsg = 0;
                for (int i = 0; i < _msgCollection.Count; i++) // find the msg which has been triggered
                {
                    ChatMessage message = _msgCollection[i];
                    if (message.MessageID == viewModel.ReceivedMsg.MessageID)
                    {
                        indOfMsg = i;
                    }
                }
                ListBoxItem item = (ListBoxItem)MainChat.ItemContainerGenerator.ContainerFromIndex(indOfMsg);
                if (item != null)
                {
                    List<ToggleButton> textBlocks = FindVisualChildren<ToggleButton>(item);
                    ToggleButton toggleBtn = textBlocks[1];
                    if (toggleBtn != null)
                    {
                        toggleBtn.IsChecked = false;
                    }
                }
            }
            return;
        }

        // Helper method to find a child of a specified type in the visual tree
        // Helper method to find all children of a specified type in the visual tree
        private List<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            List<T> children = new();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is not null and T)
                {
                    children.Add((T)child);
                }

                children.AddRange(FindVisualChildren<T>(child));
            }

            return children;
        }

        /// <summary>
        ///     Handler for the Send Button
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> Routed Event Data </param>
        private void SendHandler(object sender, RoutedEventArgs e)
        {
            string msg = SendTextBox.Text;
            msg = msg.Trim();

            if (!string.IsNullOrEmpty(msg)) // Character Limit in the TextBox
            {
                if (msg.Length > 300)
                {
                    Logger.Log("Please enter less than 300 characters!", LogLevel.WARNING);
                    return;
                }
                var viewModel = DataContext as ChatPageViewModel;


                if (string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendMessage(msg, -1, "Chat");
                }
                else // If the replyTextBox has some msg, then pass the MsgId
                {
                    viewModel.SendMessage(msg, ReplyMsgId, "Chat");
                    ReplyObject.IsChecked = false;
                }
                SendTextBox.Text = string.Empty; // clear the textbox
                ReplyTextBox.Text = string.Empty;
                Logger.Log("[ChatBubble] Sending Message", LogLevel.INFO);
            }
            return;
        }

        /// <summary>
        ///     Handler for the ReplyButton
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event</param>
        /// <param name="e"> Routed Event Data </param>
        private void ReplyHandler(object sender, RoutedEventArgs e)
        {
            if ((sender is ToggleButton senderButton) && (senderButton.DataContext is ChatMessage msg))
            {
                string? message = msg.MsgData;
                if (message != "Message Deleted.") // Can't reply to the deleted messages
                {
                    if (message.Length > 10) // Showing only the short view of the msg
                    {
                        message = message.Substring(0, 10);
                        message += "...";
                    }
                    // Displaying the Reply Message and it's sender in the ReplyTextBox
                    string senderBox = "@" + msg.Sender + ": " + message;
                    ReplyTextBox.Text = senderBox;
                    ReplyMsgId = msg.MessageID;
                    ReplyObject = senderButton;
                }
                Logger.Log("[ChatBubble] Replying to Message", LogLevel.INFO);
            }
            return;
        }

        /// <summary>
        ///     Handler for edit button
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> </param>
        private void EditHandler(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SendTextBox.Text)) // Only if there is some text in the textbox, we can edit the msg.
            {
                if ((sender is Button senderButton) && (senderButton.DataContext is ChatMessage msg)) // Getting the message from the send button
                {
                    var viewModel = DataContext as ChatPageViewModel;
                    if (msg.MsgData != "Message Deleted.") // can't edit deleted message
                    {
                        string? EditedMsg = SendTextBox.Text;
                        viewModel.EditChatMsg(msg.MessageID, EditedMsg);
                        SendTextBox.Text = string.Empty;
                    }
                }
                Logger.Log("[ChatBubble] Editing Message", LogLevel.INFO);
            }
            return;
        }

        /// <summary>
        ///     Handler for the delete button
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> Routed Event Data </param>
        private void DeleteHandler(object sender, RoutedEventArgs e)
        {
            if ((sender is Button senderButton) && (senderButton.DataContext is ChatMessage msg))
            {

                var viewModel = DataContext as ChatPageViewModel;
                viewModel.DeleteChatMsg(msg.MessageID);
                Logger.Log("[ChatBubble] Deleting Message", LogLevel.INFO);
            }
            return;
        }

        /// <summary>
        ///     Handler for Clearing reply box
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> </param>
        private void UndoReplyHandler(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton)
            {
                ReplyTextBox.Text = null;
                Logger.Log("[ChatBubble] Undo reply Message", LogLevel.INFO);
            }
            return;
        }

        /// <summary>
        ///     Upload File Button Handler
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> Routed Event Data </param>
        private void UploadHandler(object sender, RoutedEventArgs e)
        {

            var viewModel = DataContext as ChatPageViewModel;

            var openFileDialog = new OpenFileDialog();

            bool? isFileSelected = openFileDialog.ShowDialog();
            long size; // size of the file
            try
            {
                size = new FileInfo(openFileDialog.FileName).Length;
            }
            catch // If the user didnt select any file
            {
                Logger.Log("File not selected!", LogLevel.WARNING);
                return;
            }

            if (isFileSelected == true)
            {
                // Cannot upload files greater than 2MB
                if (size > 2048000)
                {
                    Logger.Log("File size greater than 2MB!", LogLevel.WARNING);
                    return;
                }

                if (string.IsNullOrEmpty(ReplyTextBox.Text))
                {
                    viewModel.SendMessage(openFileDialog.FileName, -1, "File");
                }
                else
                {
                    viewModel.SendMessage(openFileDialog.FileName, ReplyMsgId, "File");
                }

                SendTextBox.Text = string.Empty;  // After the message has been sent, clearing the textbox
                ReplyTextBox.Text = string.Empty; // and the ReplyTextBox
            }
            Logger.Log("[ChatBubble] Uploading file", LogLevel.INFO);
        }

        /// <summary>
        ///     Download Button Event Handler on Click
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> </param>
        private void DownloadHandler(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button senderBtn)
            {
                var viewModel = DataContext as ChatPageViewModel;

                SaveFileDialog dialogFile = new();

                // Get the message through the Download button
                if (senderBtn.DataContext is ChatMessage message)
                {
                    string? messageText = message.MsgData;
                    if (messageText == "Message Deleted.")
                    {
                        // Cannot download deleted files
                        Logger.Log("[ChatBubble] Can't Download Deleted file", LogLevel.WARNING);

                    }
                    else
                    {
                        // Set the File name and the Extension of the file from the message
                        dialogFile.FileName = System.IO.Path.GetFileNameWithoutExtension(message.MsgData);
                        dialogFile.DefaultExt = System.IO.Path.GetExtension(message.MsgData);

                        // Show the Dialog box to let the user choose a destination folder in which the file can be downloaded
                        bool? isDownloadOk = dialogFile.ShowDialog();

                        if (isDownloadOk == true)
                        {
                            viewModel.DownloadFile(dialogFile.FileName, message.MessageID);
                        }
                        Logger.Log("[ChatBubble] Downloading file", LogLevel.INFO);
                    }
                        
                }
            }
            return;
        }
         
        /// <summary>
        ///     Handler for Star Radio Button
        /// </summary>
        /// <param name="sender"> The Sender which is notifying the event </param>
        /// <param name="e"> </param>
        private void StarHandler(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ChatPageViewModel;
            if ((sender is ToggleButton senderRadioBtn) && (senderRadioBtn.DataContext is ChatMessage msg))
            {
                viewModel.StarChatMsg(msg.MessageID);
                Logger.Log("[ChatBubble] Starring Message", LogLevel.INFO);
            }
            return;
        }


        /// <summary>
        ///     Gets the bottom-most message in the ListBox and scrolls the listbox down to that message
        /// </summary>
        /// <param name="messages"> ListBox of messages </param>
        private void UpdateScrollBar(ListBox messages)
        {
            int countOfChildren = VisualTreeHelper.GetChildrenCount(messages);
            if ((messages != null) && (countOfChildren != 0))
            {
                var border = (Border)VisualTreeHelper.GetChild(messages, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
                Logger.Log("[ChatBubble] Scrollbar Updated", LogLevel.INFO);
            }
            return;
        }
    }
}
