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
    public class SessionExitedEventArgs : EventArgs
    {
        public TextSummary Summary { get; set; }

        public SentimentResult Sentiment { get; set; }

        public Analysis TelemetryAnalysis { get; set; }

        public SessionExitedEventArgs(TextSummary summary, SentimentResult sentiment, Analysis telemetryAnalysis)
        {
            Summary = summary;
            Sentiment = sentiment;
            TelemetryAnalysis = telemetryAnalysis;
        }
    }
}
