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
        protected readonly string _cloudUrl = @"http://localhost:7166/api/entity"; 

        public SessionsViewModel() 
        {
            LocalCommand = new LocalCommand(this);
            CloudCommand = new CloudCommand(_cloudUrl, this);
        }

        private List<TimeStampChatCountEntry> _timeStampChatCountEntries;

        public List<TimeStampChatCountEntry> TimeStampChatCountEntries
        {
            get => _timeStampChatCountEntries;
            set => SetProperty(ref _timeStampChatCountEntries, value);
        }

        private List<UserActivityEntry> _userActivities;

        public List<UserActivityEntry> UserActivities
        {
            get => _userActivities;
            set => SetProperty(ref _userActivities, value);
        }

        private List<SessionEntry> _sessions;

        public List<SessionEntry> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        private string _sessionsummary;

        public string SessionSummary
        {
            get => _sessionsummary;
            set => SetProperty(ref _sessionsummary, value);
        }

        private int _totalUserCount;

        public int TotalUserCount
        {
            get => _totalUserCount;
            set => SetProperty(ref _totalUserCount, value);
        }

        private int _totalChatCount;

        public int TotalChatCount
        {
            get => _totalChatCount;
            set => SetProperty(ref _totalChatCount, value);
        }
        
        private int _positiveChatCount;

        public int PositiveChatCount
        {
            get => _positiveChatCount;
            set => SetProperty(ref _positiveChatCount, value);
        }

        private int _negativeChatCount;

        public int NegativeChatCount
        {
            get => _negativeChatCount;
            set => SetProperty(ref  _negativeChatCount, value);
        }

        private int _neutralChatCount;

        public int NeutralChatCount
        {
            get => _neutralChatCount;
            set => SetProperty(ref  _neutralChatCount, value);
        }

        private string _overallSentiment;
        public string OverallSentiment
        {
            get => _overallSentiment;
            set => SetProperty(ref _overallSentiment, value);
        }

        public ICommand LocalCommand { get; set; }

        public ICommand CloudCommand { get; set; }
    }
}
