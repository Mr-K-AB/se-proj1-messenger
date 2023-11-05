using System;
using System.Collections.Generic;
namespace MessengerDashboard.Telemetry
{
    /// <summary>
    /// This class manages user data and telemetry analytics.
    /// </summary>

    public class TelemetryManager : ITelemetry
    {
        /*
        // Dependency fields
        // private readonly ITelemetrySessionManager serverSessionManager = TelemetrySessionManagerFactory.GetServerSessionManager();
        // private readonly TelemetryPersistence persistence = PersistenceFactory.GetTelemetryPersistenceInstance();
        // private readonly int timeThreshold = 30;

        // Data dictionaries
        public Dictionary<DateTime, int> UserCountHistory = new Dictionary<DateTime, int>();
        public Dictionary<UserData, DateTime> UserJoinTime = new Dictionary<UserData, DateTime>();
        public Dictionary<UserData, DateTime> UserExitTime = new Dictionary<UserData, DateTime>();
        public Dictionary<int, int> UserChatCount = new Dictionary<int, int>();
        public List<int> ActiveMembers = new List<int>();
        public Dictionary<int, string> UserIdToEmail = new Dictionary<int, string>();
        public Dictionary<string, string> EmailToUsername = new Dictionary<string, string>();
        public Dictionary<string, int> UsernameToChatCount = new Dictionary<string, int>();

        // Session start time
        private DateTime sessionStartTime;

        public TelemetryManager()
        {
            sessionStartTime = DateTime.Now;
            // serverSessionManager.Subscribe(this);
        }

        public void UpdateUsernameToChatCount()
        {
            UsernameToChatCount.Clear();

            foreach (var chatCountEntry in UserChatCount)
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

        public TelemetryAnalytics GetTelemetryAnalytics(object data)
        {
            DateTime currentTime = DateTime.Now;
            GetUserChatCounts(data);
            UpdateUsernameToChatCount();

            int totalChatCount = 0;
            int totalUsers = 0;

            foreach (var userChatCount in UserChatCount)
            {
                totalChatCount += userChatCount.Value;
                totalUsers++;
            }

            TelemetryAnalytics analytics = new TelemetryAnalytics
            {
                ChatCountByUser = UserChatCount,
                ActiveMembers = ActiveMembers,
                UserCountHistory = UserCountHistory,
                UsernameToChatCount = UsernameToChatCount
            };

            SessionSummary sessionSummary = new SessionSummary
            {
                UserCount = totalUsers,
                ChatCount = totalChatCount,
                Score = totalChatCount * totalUsers
            };

            analytics.SessionSummary = sessionSummary;

            return analytics;
        }

        public void SaveAnalytics(object chatMessages)
        {
            DateTime currentDateTime = DateTime.Now;
            GetUserChatCounts(chatMessages);
            UpdateUsernameToChatCount();
            GetActiveMembers(currentDateTime);

            int totalUsers = 0;
            int totalChatCount = 0;

            foreach (var userChatCount in UserChatCount)
            {
                totalChatCount += userChatCount.Value;
                totalUsers++;
            }

            TelemetryAnalytics finalAnalytics = new TelemetryAnalytics
            {
                ChatCountByUser = UserChatCount,
                ActiveMembers = ActiveMembers,
                UserCountHistory = UserCountHistory,
                UsernameToChatCount = UsernameToChatCount
            };

            SessionSummary sessionSummary = new SessionSummary
            {
                ChatCount = totalChatCount,
                UserCount = totalUsers
            };

            finalAnalytics.SessionSummary = sessionSummary;

            // TODO: Save the analytics to a persistent storage or cloud
        }

        public void GetUserChatCounts(object chatData)
        {
            UserChatCount.Clear();

            foreach (var chatThread in chatData)
            {
                foreach (var chatMessage in chatThread.MessageList)
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
            }
        }

        public void GetActiveMembers(DateTime currentTime)
        {
            ActiveMembers.Clear();

            foreach (var userEntry in UserJoinTime)
            {
                var userData = userEntry.Key;

                if (UserExitTime.ContainsKey(userData) && UserExitTime[userData].Subtract(userEntry.Value).TotalMinutes < timeThreshold)
                {
                    ActiveMembers.Add(userData.UserID);
                }
            }
        }

        public void UpdateUserIdToEmail(SessionData sessionData)
        {
            foreach (var user in sessionData.Users)
            {
                if (!UserIdToEmail.ContainsKey(user.UserID))
                {
                    UserIdToEmail[user.UserID] = user.UserEmail;
                }
            }
        }

        public void UpdateEmailToUsername(SessionData sessionData)
        {
            foreach (var user in sessionData.Users)
            {
                if (!EmailToUsername.ContainsKey(user.UserEmail))
                {
                    EmailToUsername[user.UserEmail] = user.Username;
                }
            }
        }

        public void OnAnalyticsChanged(SessionData sessionData)
        {
            var currentTime = DateTime.Now;
            UpdateUserIdToEmail(sessionData);
            UpdateEmailToUsername(sessionData);
            CalculateUserCountHistory(sessionData, currentTime);
            CalculateUserJoinExitTime(sessionData, currentTime);
            GetActiveMembers(currentTime);
            Trace.WriteLine("[Telemetry Submodule] OnAnalytics function called, telemetry data updated successfully.");
        }

        public void OnAnalyticsChanged(SessionData sessionData, DateTime currentTime)
        {
            CalculateUserCountHistory(sessionData, currentTime);
            CalculateUserJoinExitTime(sessionData, currentTime);
            GetActiveMembers(currentTime);
        }

        public void CalculateUserCountHistory(SessionData sessionData, DateTime currentTime)
        {
            UserCountHistory[currentTime] = sessionData.Users.Count;
        }

        public void CalculateUserJoinExitTime(SessionData sessionData, DateTime currentTime)
        {
            foreach (var user in sessionData.Users)
            {
                if (!UserJoinTime.ContainsKey(user))
                {
                    UserJoinTime[user] = currentTime;
                }
            }

            foreach (var userEntry in UserJoinTime)
            {
                if (!sessionData.Users.Contains(userEntry.Key) && !UserExitTime.ContainsKey(userEntry.Key))
                {
                    UserExitTime[userEntry.Key] = currentTime;
                }
            }
        }
    */
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
