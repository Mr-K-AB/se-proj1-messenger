/******************************************************************************
* Filename    = ClientSessionController.cs
*
* Author      = Shailab Chauhan 
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A class that controls the session for the client.
*****************************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using MessengerNetworking.Communicator;
using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Server;
using MessengerDashboard.Client.Events;
using MessengerNetworking.Factory;
using MessengerScreenshare.ScreenshareFactory;
using MessengerScreenshare.Client;
using MessengerContent.Client;
using MessengerDashboard.Sentiment;

namespace MessengerDashboard.Client
{
    public class ClientSessionController : IClientSessionController
    {
        private readonly ICommunicator _communicator = Factory.GetInstance();

        private readonly ManualResetEvent _connectionEstablished = new(false);

        private readonly IContentClient _contentClient = ContentClientFactory.GetInstance();

        private readonly string _serverModuleIdentifier = "DashboardServer";

        private readonly string _clientModuleIdentifier = "DashboardClient";

        private readonly string _clientSessionControllerId = Guid.NewGuid().ToString();

        private readonly IScreenshareClient _screenshareClient = ScreenshareFactory.getInstance();

        private readonly Serializer _serializer = new();

        private string _serverIp = string.Empty;

        private int _serverPort;

        private UserInfo? _userInfo;

        public ClientSessionController()
        {
            _communicator.AddSubscriber(_clientModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
            Trace.WriteLine("Dashboard: Created Client Session Manager");
        }

        public ClientSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_clientModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }

        public ClientSessionController(ICommunicator communicator, IContentClient contentClient)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_clientModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
            _contentClient = contentClient;
        }

        public event EventHandler<ClientSessionChangedEventArgs> SessionChanged;

        public event EventHandler<SessionExitedEventArgs> SessionExited;

        public event EventHandler<SessionModeChangedEventArgs> SessionModeChanged;

        public event EventHandler<RefreshedEventArgs> Refreshed;

        public Analysis? AnalysisResults { get; private set; }

        public TextSummary? ChatSummary { get; private set; }

        public ConnectionDetails ConnectionDetails { get; private set; }

        public bool IsConnectedToServer { get; private set; } = false;

        public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

        public SentimentResult SentimentResult { get; private set; } = new SentimentResult();

        public bool ConnectToServer(
            string serverIpAddress, 
            int serverPort, 
            int? timeoutInMilliseconds,
            string userName,
            string userEmail,
            string userPhotoUrl
        )
        {
            _serverIp = serverIpAddress;
            _serverPort = serverPort;
            Trace.WriteLine("Dashboard Client >>> Connecting to server at IP: " + serverIpAddress + " Port: " + serverPort);

            if (string.IsNullOrWhiteSpace(userName))
            {
                Trace.WriteLine("Dashboard Client >>> Null username received");
                return false;
            }

            lock (this)
            {
                Trace.WriteLine("Dashboard Client >>> Sending Connection Request");
                UserInfo userInfo = new(userName, -1, userEmail, userPhotoUrl);
                SendPayloadToServer(Operation.AddClient, 1, userInfo);
                Trace.WriteLine("Dashboard Client >>> Sent Connection Request");
            }
            bool isConnected;
            if (timeoutInMilliseconds == null)
            {
                isConnected = _connectionEstablished.WaitOne();
            }
            else
            {
                isConnected = _connectionEstablished.WaitOne((int)timeoutInMilliseconds);
            }
            if (isConnected)
            {
                _communicator.AddClient(serverIpAddress, serverPort);
                Trace.WriteLine("Dashboard Client >>> Connected to server");
            }
            return isConnected;
        }

        public void SendRefreshRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server for any updates");
            SendPayloadToServer(Operation.Refresh);
            Trace.WriteLine("Dashboard Client >>> Requested server for any updates");
        }

        public void OnClientJoined(string ipAddress, int port) { }

        public void OnClientLeft(string ipAddress, int port) { }

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                Trace.WriteLine("Dashboard Server >>> Received null serialized data from network");
                return;
            }

            Trace.WriteLine("Dashboard Server >>> Data received from network");
            try
            {
                ServerPayload serverPayload = _serializer.Deserialize<ServerPayload>(serializedData);
                Operation operationType = serverPayload.Operation;

                switch (operationType)
                {

                    case Operation.ExamMode:
                    case Operation.LabMode:
                        UpdateSessionData(serverPayload.SessionInfo);
                        return;

                    case Operation.AddClientAcknowledgement:
                        SetUserInfo(serverPayload);
                        UpdateSessionData(serverPayload.SessionInfo);
                        SendConfirmationToServer();
                        return;

                    case Operation.RefreshAcknowledgement:
                        Refresh(serverPayload);
                        return;

                    case Operation.RemoveClient:
                    case Operation.EndSession:
                        Refresh(serverPayload);
                        ExitSession();
                        return;

                    case Operation.SessionUpdated:
                        UpdateSessionData(serverPayload.SessionInfo);
                        return;

                    default:
                        return;
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine("Dashboard Client >>> Exception " + e);
            }
        }

        private void Refresh(ServerPayload serverPayload)
        {

            Trace.WriteLine("Dashboard Client >>> Refreshing");
            UpdateSummary(serverPayload.Summary);
            UpdateTelemetryAnalysis(serverPayload.SessionAnalysis);
            UpdateSentiment(serverPayload.Sentiment);
            UpdateSessionData(serverPayload.SessionInfo);
            Refreshed?.Invoke(this, new(AnalysisResults, SentimentResult, ChatSummary, SessionInfo));
            Trace.WriteLine("Dashboard Client >>> Refreshed");
        }

        private void SendConfirmationToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Sending confirmation to server.");
            SendPayloadToServer(Operation.AddClientConfirmation, 1);
            Trace.WriteLine("Dashboard Client >>> Sent confirmation to server.");
        }

        public void SendExitSessionRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server to let client exit.");
            SendPayloadToServer(Operation.RemoveClient, 1);
            Trace.WriteLine("Dashboard Client >>> Requested server to let client exit.");
        }

        public void SendLabModeRequestToServer()
        {

            Trace.WriteLine("Dashboard Client >>> Requesting server for lab mode.");
            SendPayloadToServer(Operation.LabMode);
            Trace.WriteLine("Dashboard Client >>> Requested server for lab mode.");
        }

        public void SendExamModeRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server for lab mode.");
            SendPayloadToServer(Operation.ExamMode);
            Trace.WriteLine("Dashboard Client >>> Requested server for lab mode.");
        }

        private void SetUserInfo(ServerPayload serverPayload)
        {
            Trace.WriteLine("Dashboard Client >>> Setting user info.");
            IsConnectedToServer = true;
            _connectionEstablished.Set();
            _userInfo = serverPayload.UserInfo;
            _screenshareClient.SetUser(_userInfo.UserId, _userInfo.UserName);
            _contentClient.SetUser(_userInfo.UserId, _userInfo.UserName, _serverIp, _serverPort);
            Trace.WriteLine("Dashboard Client >>> User info set.");
        }

        private void SendPayloadToServer(Operation operation, int priority = 0, UserInfo? userInfo = null)
        {
            Trace.WriteLine("Dashboard Client >>> Sending data to communicator.");
            userInfo ??= _userInfo;
            lock (this)
            {
                ClientPayload clientPayload = new (operation, ConnectionDetails.IpAddress, ConnectionDetails.Port, userInfo, _clientSessionControllerId);
                string serializedData = _serializer.Serialize(clientPayload);
                _communicator.SendMessage(_serverIp, _serverPort, _serverModuleIdentifier, serializedData, priority);
            }
            Trace.WriteLine("Dashboard Client >>> Data sent to communicator.");
        }

        private void ExitSession()
        {
            Trace.WriteLine("Dashboard Client >>> Exiting session");
            _communicator.RemoveClient(_serverIp, _serverPort);
            _communicator.RemoveSubscriber(_clientModuleIdentifier);
            SessionExited?.Invoke(this, new SessionExitedEventArgs(ChatSummary, SentimentResult, AnalysisResults));
            Trace.WriteLine("Dashboard Client >>> Exited session");
        }

        private void UpdateTelemetryAnalysis(Analysis analysis)
        {
            Trace.WriteLine("Dashboard Client >>> Updating telemetry analysis");
            if (analysis == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null telemetry");
                return;
            }
            lock (this)
            {
                AnalysisResults = analysis;
            }
            Trace.WriteLine("Dashboard Client >>> Updated telemetry analysis");
        }

        private void UpdateSentiment(SentimentResult sentiment)
        {
            Trace.WriteLine("Dashboard Client >>> Updating Sentiment");
            if (sentiment == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null summary");
                return;
            }
            lock (this)
            {
                SentimentResult = sentiment;
            }
            Trace.WriteLine("Dashboard Client >>> Updated Sentiment");
        }

        private void UpdateSummary(TextSummary textSummary)
        {
            Trace.WriteLine("Dashboard Client >>> Updating Summary");
            if (textSummary == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null summary");
                return;
            }
            lock (this)
            {
                ChatSummary = textSummary;
            }
            Trace.WriteLine("Dashboard Client >>> Updated Summary");
        }

        private void UpdateSessionData(SessionInfo sessionInfo)
        {
            Trace.WriteLine("Dashboard Client >>> Updating Session information.");
            SessionInfo? receivedSessionData = sessionInfo;
            bool modeChanged = false;
            lock (this)
            {
                modeChanged = receivedSessionData.SessionMode != SessionInfo.SessionMode;
                SessionInfo = receivedSessionData;
            }
            SessionChanged?.Invoke(this, new ClientSessionChangedEventArgs(SessionInfo));
            if (modeChanged)
            {
                SessionModeChanged?.Invoke(this, new SessionModeChangedEventArgs(SessionInfo.SessionMode));
            }
            Trace.WriteLine("Dashboard Client >>> Updated Session information.");
        }
    }
}
