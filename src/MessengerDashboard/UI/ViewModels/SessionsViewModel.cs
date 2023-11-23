/*****************************************************************************
*Filename = SessionsViewModel.cs
*
*Author = Satish Patidar
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the definition of the SessionsViewModel class, which is a ViewModel
* responsible for managing data related to user sessions in the Messenger Session.
* It includes properties for various session-related data such as time stamps, user activities,
*session entries, and counts of positive, negative, and neutral chats.The class also defines
* commands for local operations, cloud operations, and deleting all data. Additionally, it uses
*               a RestClient to interact with the Messenger API.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.Commands;
using MessengerCloud;


namespace MessengerDashboard.UI.ViewModels
{
    public class SessionsViewModel : ViewModel
    {


        public SessionsViewModel() 
        {
            LocalCommand = new LocalCommand(this);
            CloudCommand = new CloudCommand(RestClient, this);
            DeleteAllCommand = new DeleteAllCommand(RestClient, this);
        }
        public RestClient RestClient { get; set; } = new(@"http://localhost:7166/api/entity");

        private List<TimeStampUserCountEntry> _timeStampChatCountEntries;

        public List<TimeStampUserCountEntry> TimeStampUserCountEntries
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

        private string _sessionSummary;

        public string SessionSummary
        {
            get => _sessionSummary;
            set => SetProperty(ref _sessionSummary, value);
        }

        private int _totalUserCount;

        public int TotalUserCount
        {
            get => _totalUserCount;
            set => SetProperty(ref _totalUserCount, value);
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

        public bool? IsLocalClicked { get; set; } = null;

        public ICommand LocalCommand { get; set; }

        public ICommand CloudCommand { get; set; }

        public ICommand DeleteAllCommand { get; set; }
    }
}
