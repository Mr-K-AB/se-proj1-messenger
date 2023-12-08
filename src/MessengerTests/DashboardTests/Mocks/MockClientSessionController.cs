/******************************************************************************
* Filename    = MockClientSessionController.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class for mocking the client session controller.
*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client;
using MessengerDashboard.Client.Events;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;
using MessengerDashboard;

namespace MessengerTests.DashboardTests.Mocks
{
    internal class MockClientSessionController : IClientSessionController
    {
        public Analysis? AnalysisResults { get; set; } = new();

        public TextSummary? ChatSummary  { get; set; } = new();

        public bool IsConnectedToServer => true;

        public SessionInfo SessionInfo { get; set; } = new ();

        public SentimentResult SentimentResult { get; set; } = new ();

        public UserInfo UserInfo => new ("a", 1, "b", "c");

        public event EventHandler<RefreshedEventArgs> Refreshed;
        public event EventHandler<ClientSessionChangedEventArgs> SessionChanged;
        public event EventHandler<SessionExitedEventArgs> SessionExited;
        public event EventHandler<SessionModeChangedEventArgs> SessionModeChanged;

        public bool ConnectToServer(string serverIpAddress, int serverPort, string clientUsername, string clientEmail, string clientPhotoUrl)
        {
            SessionInfo.Users.Add(new(clientUsername, 1, clientEmail, clientPhotoUrl));
            return true;
        }

        public void SendExamModeRequestToServer()
        {
            SessionInfo.SessionMode = SessionMode.Exam;
            SessionChanged?.Invoke(this, new(SessionInfo));
        }

        public void SendExitSessionRequestToServer()
        {
            UserActivity userActivity = new ()
            {
                UserChatCount = 15,
                UserName = "John Doe",
                UserEmail = "john.doe@example.com",
                EntryTime = DateTime.Now.AddMinutes(-10),
                ExitTime = DateTime.MinValue
            };

            AnalysisResults = new Analysis
            {
                UserIdToUserActivityMap = new Dictionary<int, UserActivity>
            {
                { 1, userActivity },
            },
                TimeStampToUserCountMap = new Dictionary<DateTime, int>
            {
                { DateTime.Now.AddMinutes(-30), 5 },
            },
                TotalUserCount = 15,
                TotalChatCount = 100
            };
            ChatSummary.Sentences.Add("Hi");
            ChatSummary.Sentences.Add("Hi");
            ChatSummary.Sentences.Add("Hi");
            ChatSummary.Sentences.Add("Hi");
            ChatSummary.Sentences.Add("Hi");
            SentimentResult.NegativeChatCount = 2;
            SentimentResult.PositiveChatCount = 1;
            SentimentResult.NeutralChatCount = 2;
            SentimentResult.OverallSentiment = "Negative";
            SessionExited?.Invoke(this, new(ChatSummary, SentimentResult, AnalysisResults));
        }

        public void SendLabModeRequestToServer()
        {
            SessionInfo.SessionMode = SessionMode.Lab;
            SessionChanged?.Invoke(this, new(SessionInfo));
        }

        public void SendRefreshRequestToServer()
        {
            var userActivity = new UserActivity
            {
                UserChatCount = 10,
                UserName = "John Doe",
                UserEmail = "john.doe@example.com",
                EntryTime = DateTime.Now.AddMinutes(-30),
                ExitTime = DateTime.MinValue
            };

            AnalysisResults = new ()
            {
                UserIdToUserActivityMap = new Dictionary<int, UserActivity>
            {
                { 1, userActivity },
            },
                TimeStampToUserCountMap = new Dictionary<DateTime, int>
            {
                { DateTime.Now.AddMinutes(-30), 5 },
            },
                TotalUserCount = 15,
                TotalChatCount = 100
            };
            ChatSummary.Sentences.Add("Hello");
            SentimentResult.NeutralChatCount = 1;
            SentimentResult.PositiveChatCount = 0;
            SentimentResult.NegativeChatCount = 0;
            SentimentResult.OverallSentiment = "Neutral";
            Refreshed?.Invoke(this, new(AnalysisResults, SentimentResult, ChatSummary, SessionInfo));
        }
    }
}
