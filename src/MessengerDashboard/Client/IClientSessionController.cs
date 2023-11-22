/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>
using System;
using MessengerDashboard.Client.Events;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;

namespace MessengerDashboard.Client
{
    public interface IClientSessionController 
    {

        event EventHandler<RefreshedEventArgs> Refreshed;

        event EventHandler<ClientSessionChangedEventArgs> SessionChanged;

        event EventHandler<SessionExitedEventArgs> SessionExited;

        Analysis? AnalysisResults { get; }

        TextSummary? ChatSummary { get; }

        bool IsConnectedToServer { get; }

        SessionInfo SessionInfo { get; }

        SentimentResult SentimentResult { get; }

        bool ConnectToServer(
            string serverIpAddress,
            int serverPort,
            string clientUsername,
            string clientEmail,
            string clientPhotoUrl
        );

        void SendRefreshRequestToServer();

        void SendExitSessionRequestToServer();

        void SendLabModeRequestToServer();

        void SendExamModeRequestToServer();
    }
}
