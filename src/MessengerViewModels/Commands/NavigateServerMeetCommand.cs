using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerDashboard.Server;

namespace MessengerViewModels.Commands
{
    public class NavigateServerMeetCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;

        private readonly IClientSessionController _client;

        private readonly IServerSessionController _server;

        private bool _connected = false;

        public NavigateServerMeetCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _client = DashboardFactory.GetClientSessionController();
            _server = DashboardFactory.GetServerSessionController();
        }
        public override async void Execute(object parameter)
        {
            string ip = _server.ConnectionDetails.IpAddress;
            int port = _server.ConnectionDetails.Port;
            _connected = _client.ConnectToServer(
                ip,
                port,
                15000,
                _navigationStore.AuthResult.UserName,
                _navigationStore.AuthResult.UserEmail,
                _navigationStore.AuthResult.UserImage);

            if (_connected)
            {
                _navigationStore.CurrentViewModel = new ServerMeetViewModel(_navigationStore);
            }
        }
    }
}
