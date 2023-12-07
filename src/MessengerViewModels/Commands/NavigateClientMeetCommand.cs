/******************************************************************************
* Filename    = NavigateClientMeetCommand.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to switch ViewModel to Client.
* *****************************************************************************/

using System.Windows;
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using TraceLogger;

namespace MessengerViewModels.Commands
{
    public class NavigateClientMeetCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly HomeViewModel _homeViewModel;

        private IClientSessionController _client;
        private bool _connected = false;

        public NavigateClientMeetCommand(NavigationStore navigationStore, HomeViewModel homeViewModel)
        {
            _navigationStore = navigationStore;
            _homeViewModel = homeViewModel;
        }
        public override async void Execute(object parameter)
        {

            _client = DashboardFactory.GetClientSessionController();

            _connected = _client.ConnectToServer(_homeViewModel.JoinMeetIP,
                _homeViewModel.JoinMeetPort,
                _navigationStore.AuthResult.UserName,
                _navigationStore.AuthResult.UserEmail,
                _navigationStore.AuthResult.UserImage);

            if (_connected)
            {
                Logger.Inform($"Client is connected to server. _connected={_connected}");
                _navigationStore.CurrentViewModel = new ClientMeetViewModel(_navigationStore);

            }
            else
            {
                MessageBox.Show("Failed to connect. Check meeting details and try again", "MessengerApp", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                //_navigationStore.CurrentViewModel = _homeViewModel;
            }
        }
    }
}
