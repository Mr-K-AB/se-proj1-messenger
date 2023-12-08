/******************************************************************************
* Filename    = ScreenshareClientControl.xaml.cs
*
* Author      = Harsh Kanani
*
* Product     = Messenger
* 
* Project     = MessengerApp
*
* Description = Interaction logic for ScreenshareClientControl.xaml.
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
using MessengerScreenshare.Client;
using TraceLogger;


namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for ScreenshareClientControl.xaml
    /// </summary>
    public partial class ScreenshareClientControl : UserControl
    {
        /// <summary>
        /// Creates an instance of the ScreenshareClientControl.
        /// </summary>
        public ScreenshareClientControl()
        {
            InitializeComponent();

            // Create the ViewModel and set as data context.
            ScreenshareClientViewModel viewModel = new();
            DataContext = viewModel;
        }

        /// <summary>
        /// This function starts sharing the screen
        /// It marks the SharingScreen component of the viewModel as True.
        /// </summary>
        /// <param name="sender"> default </param>
        /// <param name="e"> default </param>
        private void StartScreenShare_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = true;
            }

            Logger.Log("Start Share Button Clicked", LogLevel.INFO);
        }

        /// <summary>
        /// This function stops sharing the screen
        /// It marks the SharingScreen component of the viewModel as False.
        /// </summary>
        /// <param name="sender"> default </param>
        /// <param name="e"> default </param>
        private void StopScreenShare_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ScreenshareClientViewModel viewModel)
            {
                viewModel.SharingScreen = false;
            }

            Logger.Log("Stop Share Button Clicked", LogLevel.INFO);
        }
    }
}
