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
using MessengerWhiteboard;
using MessengerScreenshare.Server;

namespace MessengerApp.ViewModels
{
    internal class ServerMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateServerDashboardCommand { get; }

        public ICommand NavigateServerWhiteboardCommand {  get; }

        public ICommand NavigateServerScreenshareCommand {  get; }


        private readonly IServerSessionController _server;

        private readonly DashboardInstructorViewModel _dashboardViewModel;
        private readonly MessengerWhiteboard.ViewModel _whiteboardViewModel;
        private readonly MessengerScreenshare.Server.ScreenshareServerViewModel _screenshareServerViewModel;

        public int Port { get; set; }
        public string IP { get; set; }

        private readonly NavigationStore _navigationStore;

        public object SubViewModel => _navigationStore.SubViewModel;

        public ServerMeetViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            _dashboardViewModel = new DashboardInstructorViewModel();
            _whiteboardViewModel = new MessengerWhiteboard.ViewModel();
            _screenshareServerViewModel = new ScreenshareServerViewModel(false);
            _server = DashboardFactory.GetServerSessionController();
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
            Port = _server.ConnectionDetails.Port;
            IP = _server.ConnectionDetails.IpAddress;
            NavigateServerDashboardCommand = new NavigateServerDashboardCommand(navigationStore, _dashboardViewModel);
            NavigateServerWhiteboardCommand = new NavigateServerWhiteboardCommand(navigationStore, _whiteboardViewModel);
            NavigateServerScreenshareCommand = new NavigateServerScreenshareCommand(navigationStore, _screenshareServerViewModel);

        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(_subViewModel));
        }
    }
}
