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

namespace MessengerTestUI.ViewModels
{
    internal class ClientMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        private readonly ClientSessionController _client;
        public int Port { get; set; }
        public string IP { get; set; }

        public ClientMeetViewModel(NavigationStore navigationStore, ClientSessionController client)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);

            _client = client;
        }
    }
}
