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

namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for ScreenShareServer.xaml
    /// </summary>
    public partial class ScreenShareServer : Page
    {
        public ScreenShareServer()
        {
            InitializeComponent();
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance();
            this.DataContext = viewModel;

            Trace.WriteLine(Utils.GetDebugMessage("Created the ScreenshareServerView Component", withTimeStamp: true));

            Debug.WriteLine(viewModel.CurrentWindowClients.Count);
        }

        private void PinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button pinButton)
            {
                ScreenshareServerViewModel? viewModel = this.DataContext as ScreenshareServerViewModel;

                Debug.Assert(pinButton != null, Utils.GetDebugMessage("Pin Button is not created properly"));
                Debug.Assert(pinButton.CommandParameter != null, "ClientId received to pin does not exist");
                Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));

                viewModel.OnPin(pinButton.CommandParameter.ToString()!);
            }

            Trace.WriteLine(Utils.GetDebugMessage("Pin Button Clicked", withTimeStamp: true));
        }

        private void UnpinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button someButton)
            {
                ScreenshareServerViewModel? viewModel = this.DataContext as ScreenshareServerViewModel;

                Debug.Assert(someButton != null, Utils.GetDebugMessage("Unpin Button is not created properly"));
                Debug.Assert(someButton.CommandParameter != null, "ClientId received to unpin does not exist");
                Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));

                viewModel.OnUnpin(someButton.CommandParameter.ToString()!);
            }

            Trace.WriteLine(Utils.GetDebugMessage("Unpin Button Clicked", withTimeStamp: true));
        }
    }
    }
}
