using MessengerTestUI.Stores;
using MessengerTestUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;
using MessengerDashboard.Server;

namespace MessengerTestUI.ViewModels
{
    internal class ServerMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        private readonly ServerSessionController _server;
        public int Port { get; set; }
        public string IP { get; set; }

        public ServerMeetViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            _server = new();

        }
    }
}
