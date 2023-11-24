/******************************************************************************
* Filename    = ServerMeetViewModel.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = ViewModel for Server Meet View.
* *****************************************************************************/

using MessengerViewModels.Stores;
using MessengerViewModels.Commands;
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

namespace MessengerViewModels.ViewModels
{
    public class ServerMeetViewModel : ViewModelBase
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
            _screenshareServerViewModel = ScreenshareServerViewModel.GetInstance() ;
            _server = DashboardFactory.GetServerSessionController();
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
            Port = _server.ConnectionDetails.Port;
            IP = _server.ConnectionDetails.IpAddress;

            _whiteboardViewModel = MessengerWhiteboard.ViewModel.Instance;
            _whiteboardViewModel.SetUserID(0);

            NavigateServerDashboardCommand = new NavigateServerDashboardCommand(navigationStore, _dashboardViewModel);
            NavigateServerWhiteboardCommand = new NavigateServerWhiteboardCommand(navigationStore, _whiteboardViewModel);
            NavigateServerScreenshareCommand = new NavigateServerScreenshareCommand(navigationStore, _screenshareServerViewModel);
            _navigationStore.SubViewModel = _dashboardViewModel;
        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(SubViewModel));
        }
    }
}
