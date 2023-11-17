using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.Commands;


namespace MessengerDashboard.UI.ViewModels
{
    public class SessionsViewModel : ViewModel
    {
        public SessionsViewModel()
        {
            SessionRefreshCommand = new SessionRefreshCommand(this);
        }
        private List<SessionEntry> _entry;
        public List<SessionEntry> Enteries
        {
            get => _entry;
            set => SetProperty(ref _entry, value);
        }
        private string _sessionsummary;
        public string SessionSummary
        {
            get => _sessionsummary;
            set => SetProperty(ref _sessionsummary, value);
        }
        public ICommand SessionRefreshCommand { get; set; }
    }
}
