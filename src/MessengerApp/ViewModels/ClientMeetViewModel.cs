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

namespace MessengerApp.ViewModels
{
    internal class ClientMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateClientDashboardCommand { get; }

        private readonly ClientSessionController _client = DashboardFactory.GetClientSessionController();

        private readonly NavigationStore _navigationStore;
        private ViewModelBase SubViewModel => _navigationStore.SubViewModel;
        public int Port { get; set; }
        public string IP { get; set; }


        public ClientMeetViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            NavigateClientDashboardCommand = new NavigateClientDashboardCommand(navigationStore);
        }

        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(SubViewModel));
        }
    }
}
