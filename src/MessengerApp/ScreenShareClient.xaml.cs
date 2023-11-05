/// <author> Harsh Kanani </author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MessengerScreenshare;
using MessengerScreenshare.Client;

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for ScreenShareClient.xaml
    /// </summary>
    public partial class ScreenShareClient : Page
    {
        public ScreenShareClient()
        {
            InitializeComponent();
            ScreenshareClientViewModel viewModel = new();
            DataContext = viewModel;
        }
   
        private void StartScreenShare_Click( object sender , RoutedEventArgs e )
        {
            if (DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = true;
            }

            Trace.WriteLine(Utils.GetDebugMessage("Start Share Button Clicked", withTimeStamp: true));
        }

        private void StopScreenShare_Click( object sender , RoutedEventArgs e )
        {
            if (DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = false;
            }

            Trace.WriteLine(Utils.GetDebugMessage("Stop Share Button Clicked", withTimeStamp: true));
        }
    }
}
