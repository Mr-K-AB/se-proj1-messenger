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

        public  ServerSessionController Server { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }

        public ServerMeetViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);
            Server = new();
            Server.SetDetails(navigationStore.AuthResult.UserName, navigationStore.AuthResult.UserEmail, navigationStore.AuthResult.UserImage);

            Port = Server.ConnectionDetails.Port;
            IP = Server.ConnectionDetails.IpAddress;


            SwitchModeCommand = new SwitchModeCommand(this, typeof(ServerMeetViewModel));

            RefreshCommand = new RefreshCommand(this, typeof(ServerMeetViewModel));
            EndMeetCommand = new EndMeetCommand(this, typeof(ServerMeetViewModel));
        }


        private List<User> _users;
        public List<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }
        private string _summary;
        public string Summary
        {
            get => _summary;
            set
            {
                _summary = value;
                OnPropertyChanged(nameof(Summary));
            }
        }
        private string _mode;
        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        public ICommand SwitchModeCommand { get; set; }

        public ICommand EndMeetCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
    }
}
