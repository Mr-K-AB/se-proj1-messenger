
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
    public class SessionAnalytics
    {
        public Dictionary<int, int> ChatCountPerUserID { get; set; }
        public Dictionary<string, int> UserNameToChatCount { get; set; }
        public List<int> InsincereMembers { get; set; }
        public Dictionary<DateTime, int> UserCountAtTimeStamp { get; set; }
        public SessionSummary SessionSummary { get; set; }
    }
}
