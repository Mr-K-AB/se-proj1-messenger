using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Client;
using MessengerDashboard.Sentiment;

namespace MessengerDashboard.Server
{
    public class ServerPayload
    {
        public UserInfo? User { get; set; }

        public Operation Operation { get; set; }

        public Analysis? SessionAnalysis { get; set; }

        public SessionInfo? SessionInfo { get; set; }

        public TextSummary? Summary { get; set; }

        public SentimentResult? Sentiment { get; set; }

        public ServerPayload(
            Operation operation,
            SessionInfo? sessionInfo = null,
            UserInfo? user = null,
            TextSummary? summary = null,
            Analysis? telemetryAnalysis = null,
            SentimentResult? sentiment = null
        )
        {
            Operation = operation;
            User = user;
            SessionInfo = sessionInfo;
            Summary = summary;
            SessionAnalysis = telemetryAnalysis;
            Sentiment = sentiment;
        }

        public ServerPayload()
        {
        }
    }
}
