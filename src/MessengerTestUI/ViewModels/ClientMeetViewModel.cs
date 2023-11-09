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
using MessengerDashboard;

namespace MessengerTestUI.ViewModels
{
    internal class ClientMeetViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        public  ClientSessionController Client { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }


        public ClientMeetViewModel(NavigationStore navigationStore, ClientSessionController client)
        {
            NavigateHomeCommand = new NavigateHomeCommand(navigationStore);

            Client = client;

            SwitchModeCommand = new SwitchModeCommand(this, typeof(ClientMeetViewModel));

            RefreshCommand = new RefreshCommand(this, typeof(ClientMeetViewModel));
            EndMeetCommand = new EndMeetCommand(this, typeof(ClientMeetViewModel));
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
