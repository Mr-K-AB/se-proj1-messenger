
/// <author>Aradhya Bijalwan</author>
/// <summary>
/// data class for session
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Telemetry
{
    public class Analysis
    {
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

        public Dictionary<int, UserActivity> UserIdToUserActivityMap { get; set; } = new();

        public Dictionary<DateTime, int> TimeStampToUserCountMap { get; set; } = new();

        public int TotalUserCount { get; set; } = 0;

        public int TotalChatCount { get; set; } = 0;
    }
}

