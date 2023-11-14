
/// <author>Aradhya Bijalwan</author>
/// <summary>
/// data class for session
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerCloud 
{
    public class AnalysisCloud
    {
        public AnalysisCloud(
            Dictionary<int, UserActivityCloud> userIdToUserActivity,
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
        [JsonConstructor]
        public AnalysisCloud() { }

        [JsonInclude]
        [JsonPropertyName("UserIdToUserActivityMap")]
        public Dictionary<int, UserActivityCloud> UserIdToUserActivityMap { get; set; }

        [JsonInclude]
        [JsonPropertyName("TimeStampToUserCountMap")]
        public Dictionary<DateTime, int> TimeStampToUserCountMap { get; set; }

        [JsonInclude]
        [JsonPropertyName("TotalUserCount")]
        public int TotalUserCount { get; set; }

        [JsonInclude]
        [JsonPropertyName("TotalChatCount")]
        public int TotalChatCount { get; set; }
    }
}

