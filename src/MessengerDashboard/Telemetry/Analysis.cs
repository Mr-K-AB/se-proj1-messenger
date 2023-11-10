
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
            Dictionary<int, int> userIdToChatCountMap,
            Dictionary<int, string> userIdToName,
            Dictionary<DateTime, int> timeStampToUserIdMap,
            int totalUserCount,
            int totalChatCount
        )
        {
            UserIdToChatCountMap = userIdToChatCountMap;
            UserIdToName = userIdToName;
            TimeStampToUserCount = timeStampToUserIdMap;
            TotalUserCount = totalUserCount;
            TotalChatCount = totalChatCount;
        }

        public Dictionary<int, int> UserIdToChatCountMap { get; set; }

        public Dictionary<int, string> UserIdToName { get; set; }

        public Dictionary<DateTime, int> TimeStampToUserCount { get; set; }

        public int TotalUserCount { get; set; }

        public int TotalChatCount { get; set; }
    }
}

