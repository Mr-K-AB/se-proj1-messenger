using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void changeResourceButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the existing resource by its key
            var brushResource = Resources["brushResource"] as SolidColorBrush;

            if (brushResource != null)
            {
                // Change the color of the SolidColorBrush
                brushResource.Color = (brushResource.Color == Colors.Red) ? Colors.Blue : Colors.Red; // Change to the desired color

                // Optionally, you can update the resource in the Resources dictionary
                Resources["brushResource"] = brushResource;
            }
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
