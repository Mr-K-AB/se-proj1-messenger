using MessengerApp.Stores;
using MessengerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;
using MessengerDashboard.Server;

namespace MessengerApp.ViewModels
{
    internal class ServerMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateServerDashboardCommand { get; }

        private readonly NavigationStore _navigationStore;
        private readonly DashboardServerViewModel _dashboardViewModel;
        private ViewModelBase SubViewModel => _navigationStore.SubViewModel;
        private readonly ServerSessionController _server = DashboardFactory.GetServerSessionController();
        public int Port { get; set; }
        public string IP { get; set; }

        public ServerMeetViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            _dashboardViewModel = new DashboardServerViewModel(navigationStore);
            _server.SetDetails(navigationStore.AuthResult.UserName, navigationStore.AuthResult.UserEmail, navigationStore.AuthResult.UserImage);
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
            NavigateServerDashboardCommand = new NavigateServerDashboardCommand(navigationStore, _dashboardViewModel);

            Port = _server.ConnectionDetails.Port;
            IP = _server.ConnectionDetails.IpAddress;

        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(SubViewModel));
        }
    }
}
