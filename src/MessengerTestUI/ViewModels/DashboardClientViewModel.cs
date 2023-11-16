using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;
using MessengerDashboard.Client;
using MessengerTestUI.Commands;
using MessengerTestUI.Stores;

namespace MessengerTestUI.ViewModels
{
    internal class DashboardClientViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateDashboardCommand { get; }

        private readonly NavigationStore _navigationStore;

        public ViewModelBase SubViewModel => _navigationStore.SubViewModel;

        private readonly ClientSessionController _client;


        public ClientSessionController Client { get; } = DashboardFactory.GetClientSessionController();
        public int Port { get; set; }
        public string IP { get; set; }

        public DashboardClientViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            navigationStore.SubViewModelChanged += NavigationStore_SubViewModelChanged;


            //SwitchModeCommand = new SwitchModeCommand(this);

            RefreshCommand = new RefreshCommand(this);
            EndMeetCommand = new EndMeetCommand(this);

            Client.SessionChanged += Client_SessionChanged;

            List<User> users = new();
            Client.SessionInfo.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = Client.SessionInfo.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
        }

        private void Client_SessionChanged(object? sender, MessengerDashboard.Client.Events.ClientSessionChangedEventArgs e)
        {
            List<User> users = new();
            e.Session.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = Client.SessionInfo.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
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


        private void NavigationStore_SubViewModelChanged()
        {
            OnPropertyChanged(nameof(SubViewModel));
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
