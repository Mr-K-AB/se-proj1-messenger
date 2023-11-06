using System;
using System.Collections.Generic;
using System.Diagnostics;
using MessengerDashboard.Client;

namespace MessengerDashboard.Telemetry
{
    /// <summary>
    /// This class manages user data and telemetry analytics.
    /// </summary>

    public class TelemetryManager : ITelemetry
    {
        private readonly int _timeThreshold = 30;

        public Dictionary<DateTime, int> UserCountHistory = new();
        public Dictionary<UserInfo, DateTime> UserJoinTime = new();
        public Dictionary<UserInfo, DateTime> UserExitTime = new();
        public Dictionary<int, int> UserChatCount = new();
        public List<int> ActiveMembers = new();
        public Dictionary<int, string> UserIdToEmail = new();
        public Dictionary<string, string> EmailToUsername = new();
        public Dictionary<string, int> UsernameToChatCount = new();

        private readonly DateTime _sessionStartTime = DateTime.Now;

        public event EventHandler AnalyticsChanged;

        public void UpdateUsernameToChatCount()
        {
            UsernameToChatCount.Clear();

            foreach (KeyValuePair<int, int> chatCountEntry in UserChatCount)
            {
                string userEmail = UserIdToEmail[chatCountEntry.Key];
                string username = EmailToUsername[userEmail];

                if (!UsernameToChatCount.ContainsKey(username))
                {
                    UsernameToChatCount[username] = chatCountEntry.Value;
                }
                else
                {
                    UsernameToChatCount[username] += chatCountEntry.Value;
                }
            }
        }

        public SessionAnalytics GetTelemetryAnalytics(List<string> chatData)
        {
            DateTime currentTime = DateTime.Now;
            GetUserChatCounts(chatData);
            UpdateUsernameToChatCount();
            GetActiveMembers(currentTime);

            int totalChatCount = 0;
            int totalUsers = 0;

            foreach (KeyValuePair<int, int> userChatCount in UserChatCount)
            {
                totalChatCount += userChatCount.Value;
                totalUsers++;
            }

            SessionAnalytics sessionAnalytics= new()
            {
                ChatCountPerUserID = UserChatCount,
                UserCountAtTimeStamp = UserCountHistory,
                UserNameToChatCount = UsernameToChatCount,
                InsincereMembers = ActiveMembers,
            };

            SessionSummary sessionSummary = new()
            {
                ChatCount = totalChatCount,
                Score = totalChatCount * totalUsers,
                UserCount = totalUsers
            };

            sessionAnalytics.SessionSummary = sessionSummary;

            return sessionAnalytics;
        }

        public void SaveAnalytics(List<string> chatMessages)
        {
            // SessionAnalytics sessionAnalytics = GetTelemetryAnalytics();
            // TODO: Save the analytics to a persistent storage or cloud
        }

        public void GetUserChatCounts(List<string> chatData)
        {
            /*
            UserChatCount.Clear();
            foreach (string chatMessage in chatData)
            {
                if (UserChatCount.ContainsKey(chatMessage.SenderID))
                {
                    UserChatCount[chatMessage.SenderID]++;
                }
                else
                {
                    UserChatCount.Add(chatMessage.SenderID, 1);
                }
            }
            */
        }

        public void GetActiveMembers(DateTime currentTime)
        {
            ActiveMembers.Clear();

            foreach (KeyValuePair<UserInfo, DateTime> userEntry in UserJoinTime)
            {
                UserInfo userData = userEntry.Key;

                if (UserExitTime.ContainsKey(userData) && UserExitTime[userData].Subtract(userEntry.Value).TotalMinutes < _timeThreshold)
                {
                    ActiveMembers.Add(userData.UserID);
                }
            }
        }

        public void UpdateUserIdToEmail(SessionInfo sessionData)
        {
            foreach (UserInfo user in sessionData.Users)
            {
                if (!UserIdToEmail.ContainsKey(user.UserID))
                {
                    UserIdToEmail[user.UserID] = user.UserEmail;
                }
            }
        }

        public void UpdateEmailToUsername(SessionInfo sessionData)
        {
            foreach (UserInfo user in sessionData.Users)
            {
                if (!EmailToUsername.ContainsKey(user.UserEmail))
                {
                    EmailToUsername[user.UserEmail] = user.UserName;
                }
            }
        }

        public void OnAnalyticsChanged(SessionInfo sessionData)
        {
            DateTime currentTime = DateTime.Now;
            UpdateUserIdToEmail(sessionData);
            UpdateEmailToUsername(sessionData);
            CalculateUserCountHistory(sessionData, currentTime);
            CalculateUserJoinExitTime(sessionData, currentTime);
            GetActiveMembers(currentTime);
            Trace.WriteLine("[Telemetry Submodule] OnAnalytics function called, telemetry data updated successfully.");
        }

        public void OnAnalyticsChanged(SessionInfo sessionData, DateTime currentTime)
        {
            CalculateUserCountHistory(sessionData, currentTime);
            CalculateUserJoinExitTime(sessionData, currentTime);
            GetActiveMembers(currentTime);
        }

        public void CalculateUserCountHistory(SessionInfo sessionData, DateTime currentTime)
        {
            UserCountHistory[currentTime] = sessionData.Users.Count;
        }

        public void CalculateUserJoinExitTime(SessionInfo sessionData, DateTime currentTime)
        {
            foreach (UserInfo user in sessionData.Users)
            {
                if (!UserJoinTime.ContainsKey(user))
                {
                    UserJoinTime[user] = currentTime;
                }
            }

            foreach (KeyValuePair<UserInfo, DateTime> userEntry in UserJoinTime)
            {
                if (!sessionData.Users.Contains(userEntry.Key) && !UserExitTime.ContainsKey(userEntry.Key))
                {
                    UserExitTime[userEntry.Key] = currentTime;
                }
            }
        }
        public SessionAnalytics GetTelemetryAnalytics()
        {
            throw new NotImplementedException();
        }

        public void SaveAnalytics()
        {
            throw new NotImplementedException();
        }
    }
}
