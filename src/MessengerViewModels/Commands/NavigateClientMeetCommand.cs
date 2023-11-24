

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using MessengerDashboard;
using MessengerDashboard.Client;
using System.Windows;

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
