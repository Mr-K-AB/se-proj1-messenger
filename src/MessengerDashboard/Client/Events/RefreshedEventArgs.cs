/******************************************************************************
* Filename    = RefreshedEventArgs.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Provides data for the event when the client session is refreshed.
*****************************************************************************/
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
    /// <summary>
    /// Provides data for the event when the client session is refreshed.
    /// </summary>
    public class RefreshedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the telemetry analysis data associated with the session refresh.
        /// </summary>
        public Analysis TelemetryAnalysis { get; }

        /// <summary>
        /// Gets the sentiment result associated with the session refresh.
        /// </summary>
        public SentimentResult Sentiment { get; }

        /// <summary>
        /// Gets the text summary associated with the session refresh.
        /// </summary>
        public TextSummary Summary { get; }

        /// <summary>
        /// Gets the information about the current session associated with the refresh.
        /// </summary>
        public SessionInfo SessionInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshedEventArgs"/> class with the specified data.
        /// </summary>
        /// <param name="telemetryAnalysis">The telemetry analysis data associated with the session refresh.</param>
        /// <param name="sentiment">The sentiment result associated with the session refresh.</param>
        /// <param name="summary">The text summary associated with the session refresh.</param>
        /// <param name="sessionInfo">The information about the current session associated with the refresh.</param>
        public RefreshedEventArgs(Analysis telemetryAnalysis, SentimentResult sentiment, TextSummary summary, SessionInfo sessionInfo)
        {
            TelemetryAnalysis = telemetryAnalysis;
            Sentiment = sentiment;
            Summary = summary;
            SessionInfo = sessionInfo;
        }
    }
}
