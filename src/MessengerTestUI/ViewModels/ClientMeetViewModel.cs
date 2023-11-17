using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Server;
using MessengerTestUI.Commands;
using MessengerTestUI.Stores;
using System.Windows.Input;
using MessengerDashboard.Client;

using System.Runtime.CompilerServices;
using MessengerTestUI.Views;

using MessengerDashboard;

namespace MessengerTestUI.ViewModels
{
    internal class ClientMeetViewModel : ViewModelBase
    {

        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateClientDashboardCommand { get; }

        private readonly NavigationStore _navigationStore;

        public ViewModelBase SubViewModel => _navigationStore.SubViewModel;

        private readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();


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
