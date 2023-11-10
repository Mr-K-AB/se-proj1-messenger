using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Client;

namespace MessengerDashboard.Server
{
    public class ServerPayload
    {
        public UserInfo User { get; set; }

        public Operation Operation { get; set; }

        public Analysis SessionAnalysis { get; set; }

        public SessionInfo SessionInfo { get; set; }

        public TextSummary Summary { get; set; }

        public ServerPayload(
            Operation eventName,
            SessionInfo sessionDataToSend,
            TextSummary summaryDataToSend,
            Analysis sessionAnalyticsToSend,
            UserInfo user
        )
        {
            Operation = eventName;
            User = user;
            SessionInfo = sessionDataToSend;
            Summary = summaryDataToSend;
            SessionAnalysis = sessionAnalyticsToSend;
        }

        public ServerPayload()
        {
        }
    }
}
