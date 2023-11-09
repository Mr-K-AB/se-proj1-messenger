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
    /// Interaction logic for ClientMeetingView.xaml
    /// </summary>
    public partial class ClientMeetingView : UserControl
    {
        public ClientMeetingView()
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
            WhiteboardControl whiteboardControl = new(1);
            MainContent.Content = whiteboardControl;
        }

        //private void Screenshare_Click(object sender, RoutedEventArgs e)
        //{
        //    ClientScreenshareControl screenshareControl = new();
        //    MainContent.Content = screenshareControl;
        //}
    }
}
