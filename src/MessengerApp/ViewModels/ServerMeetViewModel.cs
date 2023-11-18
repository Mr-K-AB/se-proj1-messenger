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
using MessengerDashboard.UI.ViewModels;

namespace MessengerApp.ViewModels
{
    internal class ServerMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateServerDashboardCommand { get; }

        private readonly IServerSessionController _server;

        private readonly DashboardInstructorViewModel _dashboardViewModel;
        public int Port { get; set; }
        public string IP { get; set; }

        private readonly NavigationStore _navigationStore;

        private DashboardViewModel _subViewModel => _navigationStore.SubViewModel;

        public ServerMeetViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            _dashboardViewModel = new DashboardInstructorViewModel();
            _server = DashboardFactory.GetServerSessionController();
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
            Port = _server.ConnectionDetails.Port;
            IP = _server.ConnectionDetails.IpAddress;
            NavigateServerDashboardCommand = new NavigateServerDashboardCommand(navigationStore, _dashboardViewModel);
        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(_subViewModel));
        }
    }
}
