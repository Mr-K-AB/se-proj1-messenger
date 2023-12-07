/******************************************************************************
* Filename    = NavigateServerMeetCommand.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger 
* 
* Project     = ViewModels
*
* Description = Command to switch ViewModel to Server.
* *****************************************************************************/

using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerDashboard.Server;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;
using TraceLogger;

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
                _navigationStore.AuthResult.UserName,
                _navigationStore.AuthResult.UserEmail,
                _navigationStore.AuthResult.UserImage);

            if (_connected)
            {
                _navigationStore.CurrentViewModel = new ServerMeetViewModel(_navigationStore);
                Logger.Debug("[NavigateServerMeetCommand] change current viewmodel to ServerMeetViewModel");
            }
        }
    }
}
