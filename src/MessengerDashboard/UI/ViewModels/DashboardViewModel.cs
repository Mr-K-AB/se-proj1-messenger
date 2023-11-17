using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using MessengerCloud;
using MessengerDashboard.Client;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;
using MessengerDashboard.UI.Commands;
using MessengerDashboard.UI.DataModels;

namespace MessengerDashboard.UI.ViewModels
{
    public class DashboardViewModel : ViewModel
    {
        protected readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();
        
        protected readonly string _cloudUrl = @"http://localhost:7166/api/entity"; 

        protected List<User> _users;

        public List<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        protected string _summary;

        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

        protected string _mode;

        public string Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        protected int _positiveChatCount;

        public int PositiveChatCount
        {
            get => _positiveChatCount;
            protected set => SetProperty(ref _positiveChatCount, value);
        }

        protected int _negativeChatCount;

        public int NegativeChatCount
        {
            get => _negativeChatCount;
            protected set => SetProperty(ref _negativeChatCount, value);
        }

        protected bool _isOverAllSentimentPositive;

        public bool IsOverAllSentimentPositive
        {
            get => _isOverAllSentimentPositive;
            protected set => SetProperty(ref _isOverAllSentimentPositive, value);
        }

        protected int _totalChatCount;

        public int TotalChatCount
        {
            get => _totalChatCount;
            protected set => SetProperty(ref _totalChatCount, value);
        }

        protected List<DateTime> _dateTimes;

        public List<DateTime> DateTimes
        {
            get => _dateTimes;
            protected set => SetProperty(ref _dateTimes, value);
        }

        protected List<int> _userCounts;

        public List<int> UserCounts
        {
            get => _userCounts;
            protected set => SetProperty(ref _userCounts, value);
        }

        protected List<UserActivityEntry> _userActivities;
        public List<UserActivityEntry> UserActivities
        {
            get => _userActivities;
            protected set => SetProperty(ref _userActivities, value);
        }

        protected List<string> _userNames;

        public List<string> UserNames
        {
            get => _userNames;
            protected set => SetProperty(ref _userNames, value);
        }

        protected List<int> _userChatCounts;

        public List<int> UserChatCounts
        {
            get => _userChatCounts;
            protected set => SetProperty(ref _userChatCounts, value);
        }



        public ICommand EndMeetCommand { get; } = new EndMeetCommand();

        public ICommand RefreshCommand { get; } = new RefreshCommand();


        protected void HandleSentimentChanged(object? sender, Client.Events.SentimentChangedEventArgs e)
        {
            IsOverAllSentimentPositive = e.Sentiment.IsOverallSentimentPositive;
            PositiveChatCount = e.Sentiment.PositiveChatCount;
            NegativeChatCount = e.Sentiment.NegativeChatCount;
        }

        protected void HandleSummaryChanged(object? sender, Client.Events.SummaryChangedEventArgs e)
        {
            Summary = string.Join(Environment.NewLine, e.Summary.Sentences);
        }

        protected void HandleTelemetryAnalysisChanged(object? sender, Client.Events.AnalysisChangedEventArgs e)
        {
            TotalChatCount = e.AnalysisResults.TotalChatCount;
            List<DateTime> dateTimes = new();
            List<int> userCounts = new();
            foreach (KeyValuePair<DateTime, int> item in e.AnalysisResults.TimeStampToUserCountMap)
            {
                dateTimes.Add(item.Key);
                userCounts.Add(item.Value);
            }
            DateTimes = dateTimes;
            UserCounts = userCounts;
            List<UserActivityEntry> userActivities = new();
            List<string> userNames = new();
            List<int> userChatCounts = new();
            foreach (KeyValuePair<int, UserActivity> item in e.AnalysisResults.UserIdToUserActivityMap)
            {
                userNames.Add(item.Value.UserName);
                userChatCounts.Add(item.Value.UserChatCount);

                userActivities.Add(new(item.Key, item.Value.UserChatCount, item.Value.UserName, item.Value.UserEmail,
                                item.Value.EntryTime, item.Value.ExitTime));
            }
            UserActivities = userActivities;
            UserNames = userNames;
            UserChatCounts = userChatCounts;
        }

        protected void HandleSessionChanged(object? sender, Client.Events.ClientSessionChangedEventArgs e)
        {
            List<User> users = new();
            e.Session.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = e.Session.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
        }
    }
}
