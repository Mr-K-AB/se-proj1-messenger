/******************************************************************************
* Filename    = Analysis.cs
*
* Author      = Aradhya Bijalwan
*
* Roll Number = 112001006
*
* Product     = MessengerApp
* 
* Project     = MessengerDashboard
*
* Description = Class model for storing various attributes related to a session
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Telemetry
{
    public class Analysis
    {   
        /// <summary>
        /// Constructor to set user data for the Analysis class.
        /// </summary>
        /// <param name="userIdToUserActivity">Dictionary to store user data with respect to user ID.</param>
        /// <param name="timeStampToUserIdMap">Dictionary storing entering time of users.</param>
        /// <param name="totalUserCount">Store the number of users in a session.</param>
        /// <param name="totalChatCount">Store the total chat count in a session.</param>
        public Analysis(
            Dictionary<int, UserActivity> userIdToUserActivity,
            Dictionary<DateTime, int> timeStampToUserIdMap,
            int totalUserCount,
            int totalChatCount
        )
        {
            UserIdToUserActivityMap = userIdToUserActivity;
            TimeStampToUserCountMap = timeStampToUserIdMap;
            TotalUserCount = totalUserCount;
            TotalChatCount = totalChatCount;
        }

        public Analysis() { }
        /// <summary>
        /// Gets or sets the dictionary mapping user IDs to user activities.
        /// </summary>
        public Dictionary<int, UserActivity> UserIdToUserActivityMap { get; set; } = new();

        /// <summary>
        /// Gets or sets the dictionary mapping timestamps to user counts.
        /// </summary>
        public Dictionary<DateTime, int> TimeStampToUserCountMap { get; set; } = new();

        /// <summary>
        /// Gets or sets the total number of users in a session.
        /// </summary>
        public int TotalUserCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total chat count in a session.
        /// </summary>


        public int TotalChatCount { get; set; } = 0;
    }
}

