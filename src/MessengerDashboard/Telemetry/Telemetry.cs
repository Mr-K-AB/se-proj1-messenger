/******************************************************************************
* Filename    =Telemetry.cs
*
* Author      = Aradhya Bijalwan
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description = this file update and manage all the data associated with telemetry
*****************************************************************************/
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
         //Dictionary to store user count for a given time stamp
        private readonly Dictionary<DateTime, int> _timeStampToUserCountMap = new();

        ///Dictionary to add userId to userActivity
        private readonly Dictionary<int, UserActivity> _userIdToUserActivityMap = new();

        /// storing the start time of session
        private readonly DateTime _sessionStartTime = DateTime.Now;

        //eventhandler which to notify during change in session
        public event EventHandler AnalysisChanged;
       
        ///<summary>
        ///function to subscribe to session side 
        ///</summary>
        ///Param name="serverSessionController">publisher side object</param>
        public void SubscribeToServerSessionController(ServerSessionController serverSessionController)
        {
            serverSessionController.SessionUpdated += SessionUpdatedHandler;
        }


        ///<summary>
        ///updating the telemetry model 
        ///</summary>
        ///<Param name="sender">object containing sender data</Param>
        ///<Param name="e">session side datatype for storing data for a particular session</Param>

        private void SessionUpdatedHandler(object? sender, Server.Events.SessionUpdatedEventArgs e)
        {
            lock (this)
            {   
                DateTime currentTime = DateTime.Now;
                Trace.WriteLine("Dashboard Server >>> Updating telemetry after session got updated");
                UpdateUserCountHistory(e.Session, currentTime);
                UpdateJoiningTimeOfUsers(e.Session, currentTime);
                UpdateLeavingTimeOfUsers(e.Session, currentTime);
                Trace.WriteLine("Dashboard Server >>> Updated telemetry after session got updated.");
            }
        }
        ///<summary>
        ///Adding username, userEmail .. etc, to the dictionary
        ///</summary>
        ///<Param name="userInfo">for storing userInfo information</Param>
        ///<Param name="currentTime">variable for storing time</Param>
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
        ///<summary>
        ///this function return the updated analysis called by subclass through interface
        ///</summary>
        ///<param name="userIdToUserInfoAndChatMap">dictionary for storing userIdto list of userinfo annd chtat</param>
        public Analysis UpdateAnalysis(Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap)
        {
            lock (this)
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
       }

        ///<summary>
        ///function to update  user count
        ///</summary>
        ///<param name="sessionData">model class object contianing data about a session</param>
        ///<param name="currentTime">variable for storing time</param>

        private void UpdateUserCountHistory(SessionInfo sessionData, DateTime currentTime)
        {
            _timeStampToUserCountMap[currentTime] = sessionData.Users.Count;
        }
        ///<summary>
        ///adding user if not present
        ///</summary>
        ///<Param name="sessionData">model class object contianing data about a session</Param>
        ///<Param name="currentTime">variable for storing time</Param>

        private void UpdateJoiningTimeOfUsers(SessionInfo sessionData, DateTime currentTime)
        {
            foreach (UserInfo userInfo in sessionData.Users)
            {
                AddUserIfNotPresent(userInfo, currentTime);
            }
        }
        ///<summary>
        ///updating list of user currently present
        ///</summary>i
        ///<Param name="sessionData">model class object contianing data about a session</Param>
        ///<Param name="currentTime">variable for storing time</Param>

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
        ///<summary>
        ///check if userId is in the session or not  
        ///</summary>
        ///<Param name="sessionData">model class object contianing data about a session</Param>
        ///<Param name="userId">integer containg user id</Param>
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
