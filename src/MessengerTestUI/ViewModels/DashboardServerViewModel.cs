﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessengerDashboard;
using MessengerDashboard.Server;
using MessengerTestUI.Commands;
using MessengerTestUI.Stores;

namespace MessengerTestUI.ViewModels
{
    public class DashboardServerViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }

        public IServerSessionController Server { get; } = DashboardFactory.GetServerSessionController();
        public int Port { get; set; }
        public string IP { get; set; }

        public DashboardServerViewModel(NavigationStore navigationStore)
        {
            Server.SetDetails(navigationStore.AuthResult.UserName, navigationStore.AuthResult.UserEmail, navigationStore.AuthResult.UserImage);

            Port = Server.ConnectionDetails.Port;
            IP = Server.ConnectionDetails.IpAddress;

            Server.SessionUpdated += Server_SessionUpdated;

            SwitchModeCommand = new SwitchModeCommand(this);

            RefreshCommand = new RefreshCommand(this);
            EndMeetCommand = new EndMeetCommand(this);

            List<User> users = new();
            Server._sessionInfo.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = (Server._sessionInfo.SessionMode == SessionMode.Exam) ? "Exam" : "Lab";
        }

        private void Server_SessionUpdated(object? sender, MessengerDashboard.Server.Events.SessionUpdatedEventArgs e)
        {
            List<User> users = new();
            e.Session.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = Server._sessionInfo.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
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
