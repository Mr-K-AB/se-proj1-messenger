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

        private readonly IClientSessionController _client;
        public int Port { get; set; }
        public string IP { get; set; }


        public ClientMeetViewModel(NavigationStore navigationStore, IClientSessionController client)
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
