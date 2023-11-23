/******************************************************************************
* Filename    = ServerPayload.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the definition of the ServerPayload class,
*               which represents the payload sent from the server to clients in the Messenger Dashboard application.
*****************************************************************************/

using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Client;
using MessengerDashboard.Sentiment;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Represents the payload sent from the server to clients in the Messenger Dashboard application.
    /// </summary>
    public class ServerPayload
    {
        /// <summary>
        /// Gets or sets the UserInfo associated with the payload.
        /// </summary>
        public UserInfo? UserInfo { get; set; }

        /// <summary>
        /// Gets or sets the operation type of the payload.
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Gets or sets the telemetry analysis associated with the payload.
        /// </summary>
        public Analysis? SessionAnalysis { get; set; }

        /// <summary>
        /// Gets or sets the session information associated with the payload.
        /// </summary>
        public SessionInfo? SessionInfo { get; set; }

        /// <summary>
        /// Gets or sets the text summary associated with the payload.
        /// </summary>
        public TextSummary? Summary { get; set; }

        /// <summary>
        /// Gets or sets the sentiment analysis result associated with the payload.
        /// </summary>
        public SentimentResult? Sentiment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPayload"/> class with the specified parameters.
        /// </summary>
        /// <param name="operation">The operation type of the payload.</param>
        /// <param name="sessionInfo">The session information associated with the payload.</param>
        /// <param name="user">The User information associated with the payload.</param>
        /// <param name="summary">The text summary associated with the payload.</param>
        /// <param name="telemetryAnalysis">The telemetry analysis associated with the payload.</param>
        /// <param name="sentiment">The sentiment analysis result associated with the payload.</param>
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
            UserInfo = user;
            SessionInfo = sessionInfo;
            Summary = summary;
            SessionAnalysis = telemetryAnalysis;
            Sentiment = sentiment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPayload"/> class.
        /// </summary>
        public ServerPayload()
        {
        }
    }
}
