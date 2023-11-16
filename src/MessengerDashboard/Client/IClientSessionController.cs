/// <credits>
/// <author>
/// <name>Shailab Chauhan</name>
/// <rollnumber>112001038</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Client.Events;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Server;
using MessengerDashboard.Summarization;
using MessengerDashboard.Telemetry;
using MessengerNetworking.Communicator;
using MessengerNetworking.NotificationHandler;

namespace MessengerDashboard.Client
{
    public interface IClientSessionController : INotificationHandler
    {

        event EventHandler<AnalysisChangedEventArgs> TelemetryAnalysisChanged;

        event EventHandler<ClientSessionChangedEventArgs> SessionChanged;

        event EventHandler<SessionExitedEventArgs> SessionExited;

        event EventHandler<SummaryChangedEventArgs> SummaryChanged;

        event EventHandler<SentimentChangedEventArgs> SentimentChanged;

        Analysis? AnalysisResults { get; }

        TextSummary? ChatSummary { get; }

        ConnectionDetails ConnectionDetails { get; }

        bool IsConnectedToServer { get; }

        SessionInfo SessionInfo { get; }

        SentimentResult SentimentResult { get; }

        bool ConnectToServer(
            string serverIpAddress,
            int serverPort,
            int? timeoutInMilliseconds,
            string clientUsername,
            string clientEmail,
            string clientPhotoUrl
        );

        void SendTelemetryAnalysisRequestToServer();

        void SendSummaryRequestToServer();

        void SendSentimentRequestToServer();

        void SendExitSessionRequestToServer();

        void SendLabModeRequestToServer();

        void SendExamModeRequestToServer();
    }
}
