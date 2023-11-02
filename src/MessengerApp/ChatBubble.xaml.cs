using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for ChatBubble.xaml
    /// </summary>
    public partial class ChatBubble : UserControl
    {
        public ChatBubble()
        {
            InitializeComponent();
        }

        private void ClearReplyBox(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button senderButton = (Button)sender;
                ReplyTextBox.Text = null;
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            string msg = SendTextBox.Text;
            msg = msg.Trim();
            // We send a message only when the text box is not empty
            if (!string.IsNullOrEmpty(msg))
            {
                // Character limit set to avoid long paragraphs
                if (msg.Length > 300)
                {
                    MessageBox.Show("Too many chars, limit of 300!");
                    return;
                }

                // Clearing the TextBoxes
                SendTextBox.Text = string.Empty;
                ReplyTextBox.Text = string.Empty;
            }
        }
    }
}
