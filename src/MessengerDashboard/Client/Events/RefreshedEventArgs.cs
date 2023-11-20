/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;

namespace MessengerDashboard.Client.Events
{
    public class RefreshedEventArgs : EventArgs
    {
        public Analysis TelemetryAnalysis { get; }

        public SentimentResult Sentiment { get; }

        public TextSummary Summary { get; }

        public SessionInfo SessionInfo { get; }

        public RefreshedEventArgs(Analysis telemetryAnalysis, SentimentResult sentiment, TextSummary summary, SessionInfo sessionInfo)
        {
            TelemetryAnalysis = telemetryAnalysis;
            Sentiment = sentiment;
            Summary = summary;
            SessionInfo = sessionInfo;
        }
    }
}
