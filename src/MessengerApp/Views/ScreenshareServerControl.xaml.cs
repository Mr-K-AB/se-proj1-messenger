/******************************************************************************
* Filename    = ScreenshareServerControl.xaml.cs
*
* Author      = Harsh Kanani
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = Interaction logic for ScreenshareServerControl.xaml.
*****************************************************************************/

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
        /// <summary>
        /// Creates an instance of the ScreenshareServerControl.
        /// </summary>
        public ScreenshareServerControl()
        {
            InitializeComponent();

            // Create the ViewModel and set as data context.
            ScreenshareServerViewModel viewModel = ScreenshareServerViewModel.GetInstance();
            DataContext = viewModel;

            Trace.WriteLine(Utils.GetDebugMessage("Created the ScreenshareServerView Component", withTimeStamp: true));

            Debug.WriteLine(viewModel.CurrentClients.Count);
        }

        /// <summary>
        /// This function calls the viewModel's OnPin function, which pins the tile that the user clicked. 
        /// The user's ClientID, which is kept in the Command Parameter, is the input passed to OnPin. 
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
        /// This function calls the viewModel's OnUnPin function, which unpins the tile that the user clicked. 
        /// The user's ClientID, which is kept in the Command Parameter, is the input passed to OnUnPin. 
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
