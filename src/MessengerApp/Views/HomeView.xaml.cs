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
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void Meeting_Click(object sender, RoutedEventArgs e)
        {
            MeetingView meetingView = new();
            MainContent.Content = meetingView;
        }

        private void Session_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SessionControl();
        }
    }
}
