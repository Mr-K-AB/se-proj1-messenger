using System;
using System.Collections.Generic;
using System.Diagnostics;
using MessengerDashboard.Client;
using MessengerDashboard.Server;

namespace MessengerDashboard.Telemetry
{
    /// <summary>
    /// This class manages user data and telemetry analytics.
    /// </summary>

    public class TelemetryManager : ITelemetry
    {
        private readonly Dictionary<DateTime, int> _timeStampToUserCountMap = new();
        private readonly Dictionary<int, UserActivity> _userIdToUserActivityMap = new();
        private readonly DateTime _sessionStartTime = DateTime.Now;

        public event EventHandler AnalysisChanged;

        internal void SubscribeToServerSessionController(ServerSessionController serverSessionController)
        {
            serverSessionController.SessionUpdated += SessionUpdatedHandler;
        }

        private void SessionUpdatedHandler(object? sender, Server.Events.SessionUpdatedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            Trace.WriteLine("Dashboard: Updating Telemetry");
            UpdateUserCountHistory(e.Session, currentTime);
            UpdateJoiningTimeOfUsers(e.Session, currentTime);
            UpdateLeavingTimeOfUsers(e.Session, currentTime);
            Trace.WriteLine("Dashboard: Updated Telemetry");
        }

        private bool AddUserIfNotPresent(UserInfo userInfo, DateTime currentTime)
        {
            if (!_userIdToUserActivityMap.ContainsKey(userInfo.UserId))
            {
                UserActivity userActivity = new()
                {
                    UserName = userInfo.UserName,
                    UserEmail = userInfo.UserEmail,
                    EntryTime = currentTime,
                    UserChatCount = 0
                };
                _userIdToUserActivityMap.Add(userInfo.UserId, userActivity);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Analysis UpdateAnalysis(Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap)
        {
            DateTime currentTime = DateTime.Now;
            foreach (Tuple<UserInfo, List<string>> userInfoAndChat in userIdToUserInfoAndChatMap.Values)
            {
                UserInfo userInfo = userInfoAndChat.Item1;
                AddUserIfNotPresent(userInfo, currentTime);
                UserActivity userActivity = _userIdToUserActivityMap[userInfo.UserId];
                userActivity.UserChatCount += userInfoAndChat.Item2.Count;
            }

            int totalChatCount = 0;
            int totalUsers = 0;

            foreach (UserActivity userActivity in _userIdToUserActivityMap.Values)
            {
                totalChatCount += userActivity.UserChatCount;
                totalUsers++;
            }

            Analysis sessionAnalytics = new(_userIdToUserActivityMap, _timeStampToUserCountMap, totalUsers, totalChatCount);
            return sessionAnalytics;
        }

        private void UpdateUserCountHistory(SessionInfo sessionData, DateTime currentTime)
        {
            _timeStampToUserCountMap[currentTime] = sessionData.Users.Count;
        }

        private void UpdateJoiningTimeOfUsers(SessionInfo sessionData, DateTime currentTime)
        {
            foreach (UserInfo userInfo in sessionData.Users)
            {
                AddUserIfNotPresent(userInfo, currentTime);
            }
        }

        private void UpdateLeavingTimeOfUsers(SessionInfo sessionData, DateTime currentTime)
        {
            foreach (int userId in _userIdToUserActivityMap.Keys)
            {
                
                if (!SessionContainsUserId(sessionData, userId) && _userIdToUserActivityMap[userId].ExitTime != DateTime.MinValue)
                {
                    _userIdToUserActivityMap[userId].ExitTime = currentTime;
                }
            }
        }

        private bool SessionContainsUserId(SessionInfo sessionInfo, int userId)
        {
            bool contains = false;
            foreach (UserInfo user in sessionInfo.Users)
            {
                if (user.UserId == userId)
                {
                    contains = true;
                }
            }
            return contains;
        }
    }
}
