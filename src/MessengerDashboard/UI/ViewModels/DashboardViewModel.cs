/******************************************************************************
* Filename    = DashboardViewModel.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the implementation of the DashboardViewModel class, 
*               which serves as the ViewModel for the Messenger Dashboard application.  
*****************************************************************************/

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
    /// <summary>
    /// ViewModel for the Messenger Dashboard application.
    /// </summary>
    public class DashboardViewModel : ViewModel
    {
        protected readonly IClientSessionController _client = DashboardFactory.GetClientSessionController();

        protected readonly string _cloudUrl = @"http://localhost:7166/api/entity";

        /// <summary>
        /// Default constructor for DashboardViewModel.
        /// </summary>
        public DashboardViewModel()
        {
            SetViewModel();
        }

        /// <summary>
        /// Constructor for DashboardViewModel with a specified client.
        /// </summary>
        /// <param name="client">The client session controller.</param>
        public DashboardViewModel(IClientSessionController client)
        {
            _client = client;
            SetViewModel();
        }

        /// <summary>
        /// Sets up the ViewModel with initial data and event handlers.
        /// </summary>
        protected void SetViewModel()
        {
            _client.SessionChanged += HandleSessionChanged;
            _client.Refreshed += HandleRefreshed;
            List<User> users = new();
            _client.SessionInfo.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
            Users = users;
            Mode = _client.SessionInfo.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
            IsLocalSavingEnabled = true;
        }

        /// <summary>
        /// List of users in the session.
        /// </summary>
        protected List<User> _users;

        /// <summary>
        /// Gets or sets the list of users in the session.
        /// </summary>
        public List<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        /// <summary>
        /// Summary of the session.
        /// </summary>
        protected string _summary;

        /// <summary>
        /// Gets or sets the summary of the session.
        /// </summary>
        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

        /// <summary>
        /// Mode of the session (Exam or Lab).
        /// </summary>
        protected string _mode;

        /// <summary>
        /// Gets or sets the mode of the session (Exam or Lab).
        /// </summary>
        public string Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        /// <summary>
        /// Positive chat count for charting.
        /// </summary>
        protected ChartValues<int> _positiveChatCount = new() { 0 };

        /// <summary>
        /// Gets or sets the positive chat count for charting.
        /// </summary>
        public ChartValues<int> PositiveChatCount
        {
            get => _positiveChatCount;
            set => SetProperty(ref _positiveChatCount, value);
        }

        /// <summary>
        /// Negative chat count for charting.
        /// </summary>
        protected ChartValues<int> _negativeChatCount = new() { 0 };

        /// <summary>
        /// Gets or sets the Negative chat count for charting.
        /// </summary>
        public ChartValues<int> NegativeChatCount
        {
            get => _negativeChatCount;
            protected set => SetProperty(ref _negativeChatCount, value);
        }

        protected ChartValues<int> _neutralChatCount = new() { 0 };


        public ChartValues<int> NeutralChatCount
        {
            get => _neutralChatCount;
            protected set => SetProperty(ref _negativeChatCount, value);
        }

        public Func<ChartPoint, string> LabelPoint => chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

        protected string _positiveLabel = "Hello";
        public string PositiveLabel
        {
            get =>  _positiveLabel;
            set => SetProperty(ref  _positiveLabel, value);
        }

        protected string  _negativeLabel = "Hello";

        public string NegativeLabel
        {
            get => _negativeLabel;
            protected set => SetProperty(ref _negativeLabel, value);
        }

        protected string _neutralLabel = "Hello";

        public string NeutralLabel 
        {
            get => _neutralLabel;
            protected set => SetProperty(ref _neutralLabel, value);
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

        int _maxUserCount = 5;
        public int MaxUserCount 
        {
            get => _maxUserCount ;
            set => SetProperty(ref _maxUserCount , value);
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

        /// <summary>
        /// Command to end the meeting.
        /// </summary>
        public ICommand EndMeetCommand { get; } = new EndMeetCommand();

        /// <summary>
        /// Command to refresh the dashboard.
        /// </summary>
        public ICommand RefreshCommand { get; } = new RefreshCommand();

        /// <summary>
        /// Event handler for the Refreshed event.
        /// </summary>
        protected void HandleRefreshed(object? sender, Client.Events.RefreshedEventArgs e)
        {
            lock (this)
            {
                TotalChatCount = e.TelemetryAnalysis.TotalChatCount;
                List<string> dateTimes = new();
                ChartValues<int> userCounts = new();
                int maxUserCount = 5;
                foreach (KeyValuePair<DateTime, int> item in e.TelemetryAnalysis.TimeStampToUserCountMap)
                {
                    dateTimes.Add(item.Key.ToString("HH:mm:ss"));
                    userCounts.Add(item.Value);
                    maxUserCount = Math.Max(maxUserCount, item.Value + 3);
                }
                DateTimes = dateTimes;
                UserCounts = userCounts;
                MaxUserCount = maxUserCount;

                List<UserActivityEntry> userActivities = new();
                List<string> userNames = new();
                ChartValues<int> userChatCounts = new();
                foreach (KeyValuePair<int, Telemetry.UserActivity> item in e.TelemetryAnalysis.UserIdToUserActivityMap)
                {
                    userNames.Add(item.Value.UserName);
                    userChatCounts.Add(item.Value.UserChatCount);

                    userActivities.Add(new(item.Key, item.Value.UserChatCount, item.Value.UserName, item.Value.UserEmail,
                                    item.Value.EntryTime, item.Value.ExitTime));
                }

                UserActivities = userActivities;
                UserNames = userNames;
                UserChatCounts = userChatCounts;

                Summary = string.Join(Environment.NewLine, e.Summary.Sentences);

                OverallSentiment = e.Sentiment.OverallSentiment;
                PositiveChatCount =  new () { e.Sentiment.PositiveChatCount };
                NegativeChatCount = new () { e.Sentiment.NegativeChatCount };
                NeutralChatCount = new () { e.Sentiment.NeutralChatCount };
                double total = e.Sentiment.PositiveChatCount + e.Sentiment.NegativeChatCount + e.Sentiment.NeutralChatCount;
                PositiveLabel = e.Sentiment.PositiveChatCount.ToString() + " " + (e.Sentiment.PositiveChatCount / total).ToString(".0");
                NegativeLabel = e.Sentiment.NegativeChatCount.ToString() + " " + (e.Sentiment.NegativeChatCount / total).ToString(".0");
                NeutralLabel = e.Sentiment.NeutralChatCount.ToString() + " " + (e.Sentiment.NeutralChatCount / total).ToString(".0");

                List<User> users = new();
                e.SessionInfo.Users.ForEach(user => { users.Add(new User(user.UserName, user.UserPhotoUrl)); });
                Users = users;
                Mode = e.SessionInfo.SessionMode == SessionMode.Exam ? "Exam" : "Lab";
            }
        }

        /// <summary>
        /// Event handler for the SessionChanged event.
        /// </summary>
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

        /// <summary>
        /// Creates the data to be saved to the cloud.
        /// </summary>
        /// <param name="textSummary">Text summary of the session.</param>
        /// <param name="sentimentResult">Sentiment analysis result.</param>
        /// <param name="analysis">Telemetry analysis data.</param>
        /// <returns>An EntityInfoWrapper containing the session data.</returns>
        public EntityInfoWrapper CreateSessionSaveData(TextSummary textSummary, SentimentResult sentimentResult, Telemetry.Analysis analysis)
        {
            foreach(KeyValuePair<int, Telemetry.UserActivity> x in analysis.UserIdToUserActivityMap)
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
                                               sentimentResult.NegativeChatCount, sentimentResult.NeutralChatCount, sentimentResult.OverallSentiment,
                                               Guid.NewGuid().ToString(), ConvertToCloudObject(analysis));
            return entityInfo;
        }

        /// <summary>
        /// Saves the session data to local storage.
        /// </summary>
        /// <param name="entityInfo">Entity information to be saved.</param>
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

        /// <summary>
        /// Converts Telemetry.Analysis to MessengerCloud.Analysis.
        /// </summary>
        /// <param name="analysis">Telemetry analysis data.</param>
        /// <returns>Converted MessengerCloud.Analysis object.</returns>
        protected MessengerCloud.Analysis ConvertToCloudObject(Telemetry.Analysis analysis)
        {
            Dictionary<int, MessengerCloud.UserActivity> userIdToUserActivityMap = new();
            foreach(KeyValuePair<int, Telemetry.UserActivity> keyValuePair in analysis.UserIdToUserActivityMap)
            {
                MessengerCloud.UserActivity userActivity = new()
                {
                    ExitTime = keyValuePair.Value.ExitTime,
                    EntryTime = keyValuePair.Value.EntryTime,
                    UserEmail = keyValuePair.Value.UserEmail,
                    UserChatCount = keyValuePair.Value.UserChatCount,
                    UserName = keyValuePair.Value.UserName
                };
                userIdToUserActivityMap.Add(keyValuePair.Key, userActivity);
            }
            MessengerCloud.Analysis analysisCloud = new(userIdToUserActivityMap, analysis.TimeStampToUserCountMap, analysis.TotalUserCount, analysis.TotalChatCount);
            return analysisCloud;
        }
    }
}
