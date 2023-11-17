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

namespace MessengerApp.ViewModels
{
    internal class ClientMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateClientDashboardCommand { get; }

        public ICommand NavigateClientScreenshareCommand { get; }


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

            _navigationStore = navigationStore;
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            NavigateClientDashboardCommand = new NavigateClientDashboardCommand(navigationStore, _dashboardViewModel);
            NavigateClientScreenshareCommand = new NavigateClientScreenshareCommand(navigationStore, _screenshareViewModel);

        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(SubViewModel));
        }
    }
}
