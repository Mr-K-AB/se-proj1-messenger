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
using MessengerScreenshare.Server;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for ScreenshareServerControl.xaml
    /// </summary>
    public partial class ScreenshareServerControl : UserControl
    {
        public ScreenshareServerControl()
        {
            InitializeComponent();
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance();
            DataContext = viewModel;

            Trace.WriteLine(Utils.GetDebugMessage("Created the ScreenshareServerView Component", withTimeStamp: true));

            Debug.WriteLine(viewModel.CurrentWindowClients.Count);
        }
        private void OnNextPageButtonClicked(object sender, RoutedEventArgs e)
        {
            ScreenshareServerViewModel? viewModel = DataContext as ScreenshareServerViewModel;
            Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));
            viewModel.RecomputeCurrentWindowClients(viewModel.CurrentPage + 1);

            Trace.WriteLine(Utils.GetDebugMessage("Next Page Button Clicked", withTimeStamp: true));
        }

        /// <summary>
        /// This function decreases the current page number by 1
        /// If on the first page, previous button is not accessible and so is this function 
        /// </summary>
        /// <param name="sender"> default </param>
        /// <param name="e"> default </param>
        private void OnPreviousPageButtonClicked(object sender, RoutedEventArgs e)
        {
            ScreenshareServerViewModel? viewModel = DataContext as ScreenshareServerViewModel;
            Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));
            viewModel.RecomputeCurrentWindowClients(viewModel.CurrentPage - 1);

            Trace.WriteLine(Utils.GetDebugMessage("Previous Page Button Clicked", withTimeStamp: true));
        }

        /// <summary>
        /// This function calls the OnPin function of the viewModel which pins the tile on which the user has clicked 
        /// The argument given to OnPin is the ClientID of user which has to be pinned, stored in Command Parameter 
        /// </summary>
        /// <param name="sender"> default </param>
        /// <param name="e"> default </param>
        private void OnPinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button pinButton)
            {
                ScreenshareServerViewModel? viewModel = DataContext as ScreenshareServerViewModel;

                Debug.Assert(pinButton != null, Utils.GetDebugMessage("Pin Button is not created properly"));
                Debug.Assert(pinButton.CommandParameter != null, "ClientId received to pin does not exist");
                Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));

                viewModel.OnPin((int)pinButton.CommandParameter!);
            }

            Trace.WriteLine(Utils.GetDebugMessage("Pin Button Clicked", withTimeStamp: true));
        }

        /// <summary>
        /// This function calls the OnUnpin function of the ViewModel which will unpin the tile the user clicked on
        /// The argument given to Unpin function is the Client ID which has to be unpinned, stored in the Command Parameter 
        /// </summary>
        /// <param name="sender"> default </param>
        /// <param name="e"> default </param>
        public void OnUnpinButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button someButton)
            {
                ScreenshareServerViewModel? viewModel = DataContext as ScreenshareServerViewModel;

                Debug.Assert(someButton != null, Utils.GetDebugMessage("Unpin Button is not created properly"));
                Debug.Assert(someButton.CommandParameter != null, "ClientId received to unpin does not exist");
                Debug.Assert(viewModel != null, Utils.GetDebugMessage("View Model could not be created"));

                viewModel.OnUnpin((int)someButton.CommandParameter!);
            }

            Trace.WriteLine(Utils.GetDebugMessage("Unpin Button Clicked", withTimeStamp: true));
        }
    }
}
