/******************************************************************************
* Filename    = ClientMeetViewModel.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = ViewModel for Client Meet View.
* *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Server;
using MessengerViewModels.Commands;
using MessengerViewModels.Stores;
using System.Windows.Input;
using MessengerDashboard.Client;
using MessengerDashboard;
using MessengerDashboard.UI.ViewModels;
using MessengerScreenshare.Client;
using MessengerWhiteboard;
using TraceLogger;

namespace MessengerViewModels.ViewModels
{
    public class ClientMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateClientDashboardCommand { get; }

        public ICommand NavigateClientScreenshareCommand { get; }

        public ICommand NavigateServerWhiteboardCommand { get; }

        public object SubViewModel => _navigationStore.SubViewModel;

        private readonly DashboardMemberViewModel _dashboardViewModel;
        private readonly ScreenshareClientViewModel _screenshareViewModel;
        private readonly MessengerWhiteboard.ViewModel _whiteboardViewModel;


        private readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();
        public int Port { get; set; }
        public string IP { get; set; }

        private readonly NavigationStore _navigationStore;

        public ClientMeetViewModel(NavigationStore navigationStore)
        {
            Logger.Inform($"[ClientMeetViewModel] IP: {IP}, Port: {Port}");
            _navigationStore = navigationStore;
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;

            _dashboardViewModel = new DashboardMemberViewModel();
            _screenshareViewModel = new ScreenshareClientViewModel();
            _whiteboardViewModel = MessengerWhiteboard.ViewModel.Instance;
            _whiteboardViewModel.SetUserID(1);

            _navigationStore.SubViewModel = _dashboardViewModel;

            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            NavigateClientDashboardCommand = new NavigateClientDashboardCommand(navigationStore, _dashboardViewModel);
            NavigateClientScreenshareCommand = new NavigateClientScreenshareCommand(navigationStore, _screenshareViewModel);
            NavigateServerWhiteboardCommand = new NavigateServerWhiteboardCommand(navigationStore, _whiteboardViewModel);

        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(SubViewModel));
        }
    }
}
