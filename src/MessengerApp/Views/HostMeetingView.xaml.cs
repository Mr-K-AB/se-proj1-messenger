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

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for HostMeetingView.xaml
    /// </summary>
    public partial class HostMeetingView : UserControl
    {
        public HostMeetingView()
        {
            InitializeComponent();
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardControl dashboardControl = new();
            MainContent.Content = dashboardControl;
        }

        private void WhiteBoard_Click(object sender, RoutedEventArgs e)
        {
            WhiteboardControl whiteboardControl = new(0);
            MainContent.Content = whiteboardControl;
        }

        private void HostScreenshare_Click(object sender, RoutedEventArgs e)
        {
            ScreenshareServerControl screenshareserverControl = new();
            MainContent.Content = screenshareserverControl;
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            ChatBubble chatBubbleControl = new();
            OverlayContent.Content = chatBubbleControl;
            OverlayPanel.Visibility = OverlayPanel.IsVisible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
