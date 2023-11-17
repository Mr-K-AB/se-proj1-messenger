using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LiveCharts;
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

        public DashboardViewModel()
        {
            _client.SessionChanged += HandleSessionChanged;
            _client.TelemetryAnalysisChanged += HandleTelemetryAnalysisChanged;
            _client.SummaryChanged += HandleSummaryChanged;
            _client.SentimentChanged += HandleSentimentChanged;
            List<User> users = new();
            _client.SessionInfo.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = _client.SessionInfo.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
            IsLocalSavingEnabled = true;
        }

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
            set => SetProperty(ref _positiveChatCount, value);
        }

        protected int _negativeChatCount;

        public int NegativeChatCount
        {
            get => _negativeChatCount;
            protected set => SetProperty(ref _negativeChatCount, value);
        }

        protected string _overallSentiment;

        public string OverallSentiment
        {
            get => _overallSentiment;
            protected set => SetProperty(ref _overallSentiment, value);
        }

        protected int _totalChatCount;

        public int TotalChatCount
        {
            get => _totalChatCount;
            set => SetProperty(ref _totalChatCount, value);
        }

        protected List<string> _dateTimes;

        public List<string> DateTimes
        {
            get => _dateTimes;
            set => SetProperty(ref _dateTimes, value);
        }

        protected ChartValues<int> _userCounts;

        public ChartValues<int> UserCounts
        {
            get => _userCounts;
            set => SetProperty(ref _userCounts, value);
        }

        protected List<UserActivityEntry> _userActivities;
        public List<UserActivityEntry> UserActivities
        {
            get => _userActivities;
            set => SetProperty(ref _userActivities, value);
        }

        protected List<string> _userNames;

        public List<string> UserNames
        {
            get => _userNames;
            set => SetProperty(ref _userNames, value);
        }

        protected ChartValues<int> _userChatCounts;

        public ChartValues<int> UserChatCounts
        {
            get => _userChatCounts;
            set => SetProperty(ref _userChatCounts, value);
        }

        protected bool _isLocalSavingEnabled;

        public bool IsLocalSavingEnabled
        {
            get => _isLocalSavingEnabled;
            set => SetProperty(ref _isLocalSavingEnabled, value);
        }

        public ICommand EndMeetCommand { get; } = new EndMeetCommand();

        public ICommand RefreshCommand { get; } = new RefreshCommand();


        protected void HandleSentimentChanged(object? sender, Client.Events.SentimentChangedEventArgs e)
        {
            lock (this)
            {
                OverallSentiment = e.Sentiment.IsOverallSentimentPositive ? "Positive" : "Negative";
                PositiveChatCount = e.Sentiment.PositiveChatCount;
                NegativeChatCount = e.Sentiment.NegativeChatCount;
            }
       }

        protected void HandleSummaryChanged(object? sender, Client.Events.SummaryChangedEventArgs e)
        {
            lock (this)
            {
                Summary = string.Join(Environment.NewLine, e.Summary.Sentences);
            }
        }

        protected void HandleTelemetryAnalysisChanged(object? sender, Client.Events.AnalysisChangedEventArgs e)
        {
            lock (this)
            {
                TotalChatCount = e.AnalysisResults.TotalChatCount;
                List<string> dateTimes = new();
                ChartValues<int> userCounts = new();
                foreach (KeyValuePair<DateTime, int> item in e.AnalysisResults.TimeStampToUserCountMap)
                {
                    dateTimes.Add(item.Key.ToString("HH:mm:ss"));
                    userCounts.Add(item.Value);
                }
                DateTimes = dateTimes;
                UserCounts = userCounts;
                List<UserActivityEntry> userActivities = new();
                List<string> userNames = new();
                ChartValues<int> userChatCounts = new();
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
        }

        protected void HandleSessionChanged(object? sender, Client.Events.ClientSessionChangedEventArgs e)
        {
            lock (this)
            {
                List<User> users = new();
                e.Session.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
                Users = users;
                Mode = e.Session.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
            }
       }

        public EntityInfoWrapper CreateSessionSaveData(TextSummary textSummary, SentimentResult sentimentResult, Analysis analysis)
        {
            foreach(KeyValuePair<int, UserActivity> x in analysis.UserIdToUserActivityMap)
            {
                if(x.Value.EntryTime <= DateTime.MinValue || x.Value.EntryTime >= DateTime.MaxValue)
                {
                    x.Value.EntryTime = DateTime.Now;
                }

                if(x.Value.ExitTime <= DateTime.MinValue || x.Value.ExitTime >= DateTime.MaxValue)
                {
                    x.Value.ExitTime = DateTime.Now;
                }
            }
            EntityInfoWrapper entityInfo = new(textSummary.Sentences, sentimentResult.PositiveChatCount,
                                               sentimentResult.NegativeChatCount, sentimentResult.IsOverallSentimentPositive,
                                               Guid.NewGuid().ToString(), ConvertToCloudObject(analysis));
            return entityInfo;
        }

        protected void SaveSessionToLocalStorage(EntityInfoWrapper entityInfo)
        {
            try
            {
                LocalSave.AddEntity(entityInfo);
            }
            catch(Exception e)
            {
                Trace.WriteLine($"{e.Message}");
            }
        }

        protected AnalysisCloud ConvertToCloudObject(Analysis analysis)
        {
            Dictionary<int, UserActivityCloud> userIdToUserActivityMap = new();
            foreach(KeyValuePair<int, UserActivity> keyValuePair in analysis.UserIdToUserActivityMap)
            {
                UserActivityCloud userActivity = new()
                {
                    ExitTime = keyValuePair.Value.ExitTime,
                    EntryTime = keyValuePair.Value.EntryTime,
                    UserEmail = keyValuePair.Value.UserEmail,
                    UserChatCount = keyValuePair.Value.UserChatCount,
                    UserName = keyValuePair.Value.UserName
                };
                userIdToUserActivityMap.Add(keyValuePair.Key, userActivity);
            }
            AnalysisCloud analysisCloud = new(userIdToUserActivityMap, analysis.TimeStampToUserCountMap, analysis.TotalUserCount, analysis.TotalChatCount);
            return analysisCloud;
        }

    }
}
