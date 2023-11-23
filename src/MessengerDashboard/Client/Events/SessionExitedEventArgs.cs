/******************************************************************************
* Filename    = ClientSessionController.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A class that controls the session for the client.
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
    /// Provides data for the event when the client session is exited in the Messenger Dashboard project.
    /// </summary>
    public class SessionExitedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the text summary associated with the session exit.
        /// </summary>
        public TextSummary Summary { get; set; }

        /// <summary>
        /// Gets or sets the sentiment result associated with the session exit.
        /// </summary>
        public SentimentResult Sentiment { get; set; }

        /// <summary>
        /// Gets or sets the telemetry analysis data associated with the session exit.
        /// </summary>
        public Analysis TelemetryAnalysis { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionExitedEventArgs"/> class with the specified data.
        /// </summary>
        /// <param name="summary">The text summary associated with the session exit.</param>
        /// <param name="sentiment">The sentiment result associated with the session exit.</param>
        /// <param name="telemetryAnalysis">The telemetry analysis data associated with the session exit.</param>
        public SessionExitedEventArgs(TextSummary summary, SentimentResult sentiment, Analysis telemetryAnalysis)
        {
            Summary = summary;
            Sentiment = sentiment;
            TelemetryAnalysis = telemetryAnalysis;
        }
    }
}
