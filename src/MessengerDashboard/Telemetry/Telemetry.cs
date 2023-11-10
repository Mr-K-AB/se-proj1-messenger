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

        private readonly Dictionary<DateTime, int> _timeStampToUserCountMap = new();
        private readonly Dictionary<int, DateTime> _userToJoiningTimeMap = new();
        private readonly Dictionary<int, DateTime> _userLeavingTimeMap = new();
        private readonly Dictionary<int, int> _userChatCountMap = new();
        private readonly Dictionary<int, string> _userIdToEmailMap = new();
        private readonly Dictionary<string, string> _emailToUsernameMap = new();
        private readonly Dictionary<string, int> _usernameToChatCountMap = new();
        private readonly DateTime _sessionStartTime = DateTime.Now;

        public event EventHandler AnalysisChanged;

        public void UpdateUsernameToChatCount()
        {
            _usernameToChatCountMap.Clear();

            foreach (KeyValuePair<int, int> chatCountEntry in _userChatCountMap)
            {
                string userEmail = _userIdToEmailMap[chatCountEntry.Key];
                string username = _emailToUsernameMap[userEmail];

                if (!_usernameToChatCountMap.ContainsKey(username))
                {
                    _usernameToChatCountMap[username] = chatCountEntry.Value;
                }
                else
                {
                    _usernameToChatCountMap[username] += chatCountEntry.Value;
                }
            }
        }

        public Analysis Analyze(List<string> chatData)
        {
            //DateTime currentTime = DateTime.Now;
            UpdateUserIdToChatCount();
            UpdateUsernameToChatCount();
            int totalChatCount = 0;
            int totalUsers = 0;

            foreach (KeyValuePair<int, int> userChatCount in _userChatCountMap)
            {
                totalChatCount += userChatCount.Value;
                totalUsers++;
            }

            Analysis sessionAnalytics = new(_userChatCountMap, new(), new(), new(), new());
            {
                /*
                    ChatCountPerUserID = UserChatCount,
                    UserCountAtTimeStamp = UserCountHistory,
                    UserNameToChatCount = UsernameToChatCount,
                    InsincereMembers = ActiveMembers,
                */
            };

            ;
            sessionAnalytics.TotalChatCount = totalChatCount;
            sessionAnalytics.TotalUserCount = totalUsers;
            return sessionAnalytics;
        }

        // TODO: Saving Analysis
        public void SaveAnalytics()
        {
            // SessionAnalytics sessionAnalytics = GetTelemetryAnalytics();
            // TODO: Save the analytics to a persistent storage or cloud
        }

        // TODO: Integrate with chat module
        public void UpdateUserIdToChatCount()
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

        //TODO:Add Comments
        public void UpdateEmailToUsername(SessionInfo sessionData)
        {
            foreach (UserInfo user in sessionData.Users)
            {
                if (!_emailToUsernameMap.ContainsKey(user.UserEmail))
                {
                    _emailToUsernameMap[user.UserEmail] = user.UserName;
                }
            }
        }

        public void OnAnalyticsChanged(SessionInfo sessionData, DateTime currentTime)
        {
            Trace.WriteLine("Dashboard: Updating Telemetry");
            UpdateUserCountHistory(sessionData, currentTime);
            UpdateJoiningTimeOfUsers(sessionData, currentTime);
            UpdateLeavingTimeOfUsers(sessionData, currentTime);
            UpdateEmailToUsername(sessionData);
            Trace.WriteLine("Dashboard: Updated Telemetry");
        }

        //TODO: AddComments
        public void UpdateUserCountHistory(SessionInfo sessionData, DateTime currentTime)
        {
            _timeStampToUserCountMap[currentTime] = sessionData.Users.Count;
        }


        //TODO: Add Comments
        public void UpdateJoiningTimeOfUsers(SessionInfo sessionData, DateTime currentTime)
        {
            foreach (UserInfo user in sessionData.Users)
            {
                if (!_userToJoiningTimeMap.ContainsKey(user.UserId))
                {
                    _userToJoiningTimeMap[user.UserId] = currentTime;
                }
            }
        }

        //TODO: Add Comments
        public void UpdateLeavingTimeOfUsers(SessionInfo sessionData, DateTime currentTime)
        {
            foreach (KeyValuePair<int, DateTime> userEntry in _userToJoiningTimeMap)
            {
                bool contains1 = false;
                foreach (UserInfo user in sessionData.Users)
                {
                    if (user.UserId == userEntry.Key)
                    {
                        contains1 = true;
                    }
                }
                if (!contains1 && !_userLeavingTimeMap.ContainsKey(userEntry.Key))
                {
                    _userLeavingTimeMap[userEntry.Key] = currentTime;
                }
            }
        }

        public Analysis GetTelemetryAnalytics()
        {
            return new Analysis(new(), new(), new(), 1, 1);
        }

    }
}
