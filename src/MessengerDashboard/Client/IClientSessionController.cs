/******************************************************************************
* Filename    = IClientSessionController.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Interface for client session control
*****************************************************************************/
using System;
using MessengerDashboard.Client.Events;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;

namespace MessengerDashboard.Client
{
    /// <summary>
    /// Interface representing the controller for client sessions in the Messenger Dashboard project.
    /// </summary>
    public interface IClientSessionController 
    {
        /// <summary>
        /// Event triggered when the client session is refreshed.
        /// </summary>
        event EventHandler<RefreshedEventArgs> Refreshed;

        /// <summary>
        /// Event triggered when the client session has changed.
        /// </summary>
        event EventHandler<ClientSessionChangedEventArgs> SessionChanged;

        /// <summary>
        /// Event triggered when the client session has exited.
        /// </summary>
        event EventHandler<SessionExitedEventArgs> SessionExited;

        /// <summary>
        /// Event triggered when the client session mode changes
        /// </summary>
        event EventHandler<SessionModeChangedEventArgs> SessionModeChanged;

        /// <summary>
        /// Gets the analysis results related to the client session.
        /// </summary>
        Analysis? AnalysisResults { get; }

        /// <summary>
        /// Gets the text summary of the client session's chat.
        /// </summary>
        TextSummary? ChatSummary { get; }

        /// <summary>
        /// Gets a value indicating whether the client is connected to the server.
        /// </summary>
        bool IsConnectedToServer { get; }

        /// <summary>
        /// Gets information about the current session.
        /// </summary>
        SessionInfo SessionInfo { get; }

        /// <summary>
        /// Gets the sentiment result related to the client session.
        /// </summary>
        SentimentResult SentimentResult { get; }

        /// <summary>
        /// Attempts to connect the client to the server.
        /// </summary>
        /// <param name="serverIpAddress">The IP address of the server to connect to.</param>
        /// <param name="serverPort">The port number of the server to connect to.</param>
        /// <param name="clientUsername">The username of the client.</param>
        /// <param name="clientEmail">The email of the client.</param>
        /// <param name="clientPhotoUrl">The photo URL of the client.</param>
        /// <returns>True if the connection is successful; otherwise, false.</returns>
        bool ConnectToServer(
            string serverIpAddress,
            int serverPort,
            string clientUsername,
            string clientEmail,
            string clientPhotoUrl
        );

        /// <summary>
        /// Sends a request to the server to refresh the client session.
        /// </summary>
        void SendRefreshRequestToServer();

        /// <summary>
        /// Sends a request to the server to allow the client to exit the session.
        /// </summary>
        void SendExitSessionRequestToServer();

        /// <summary>
        /// Sends a request to the server to enable lab mode for the client.
        /// </summary>
        void SendLabModeRequestToServer();

        /// <summary>
        /// Sends a request to the server to enable exam mode for the client.
        /// </summary>
        void SendExamModeRequestToServer();
    }
}
