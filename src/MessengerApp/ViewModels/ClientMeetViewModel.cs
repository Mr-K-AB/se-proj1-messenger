using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Server;
using MessengerApp.Commands;
using MessengerApp.Stores;
using System.Windows.Input;
using MessengerDashboard.Client;
using MessengerDashboard;
using MessengerDashboard.UI.ViewModels;
using MessengerScreenshare.Client;
using MessengerWhiteboard;


namespace MessengerApp.ViewModels
{
    internal class ClientMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateClientDashboardCommand { get; }
        public ICommand NavigateServerWhiteboardCommand { get; }

        public ICommand NavigateClientScreenshareCommand { get; }

        private readonly MessengerWhiteboard.ViewModel _whiteboardViewModel;


        public object SubViewModel => _navigationStore.SubViewModel;

        private readonly DashboardMemberViewModel _dashboardViewModel;
        private readonly ScreenshareClientViewModel _screenshareViewModel;



        private readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();
        public int Port { get; set; }
        public string IP { get; set; }

        private readonly NavigationStore _navigationStore;

        public ClientMeetViewModel(NavigationStore navigationStore)
        {
            _dashboardViewModel = new DashboardMemberViewModel();
            _screenshareViewModel = new ScreenshareClientViewModel();
            _whiteboardViewModel = new MessengerWhiteboard.ViewModel();


            _whiteboardViewModel = MessengerWhiteboard.ViewModel.Instance;
            _whiteboardViewModel.SetUserID(1);

            _navigationStore = navigationStore;
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
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
