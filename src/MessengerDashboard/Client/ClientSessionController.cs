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
using System.Diagnostics;
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
using MessengerNetworking.NotificationHandler;

namespace MessengerDashboard.Client
{
    public class ClientSessionController : IClientSessionController, INotificationHandler
    {
        private readonly ICommunicator _communicator;

        private readonly IContentClient _contentClient;

        private readonly string _moduleName = "Dashboard";

        private readonly IScreenshareClient _screenshareClient;

        private readonly Serializer _serializer = new();

        private string _serverIp = string.Empty;

        private int _serverPort;

        private readonly UserInfo _userInfo = new();

        public ClientSessionController()
        {
            Trace.WriteLine("Dashboard Client >>> Creating Client Session Manager");
            _communicator = CommunicationFactory.GetCommunicator(true);
            _communicator.Subscribe(_moduleName, this);
            _contentClient = ContentClientFactory.GetInstance();
            _screenshareClient = ScreenshareFactory.getInstance();
            Trace.WriteLine("Dashboard Client >>> Created Client Session Manager");
        }

        public ClientSessionController(ICommunicator communicator, IContentClient contentClient, IScreenshareClient screenshareClient)
        {
            Trace.WriteLine("Dashboard Client >>> Creating Client Session Manager");
            _communicator = communicator;
            _communicator.Subscribe(_moduleName, this);
            _contentClient = contentClient;
            _screenshareClient = screenshareClient;
            Trace.WriteLine("Dashboard Client >>> Created Client Session Manager");
        }

        public event EventHandler<ClientSessionChangedEventArgs>? SessionChanged;

        public event EventHandler<SessionExitedEventArgs>? SessionExited;

        public event EventHandler<SessionModeChangedEventArgs>? SessionModeChanged;

        public event EventHandler<RefreshedEventArgs>? Refreshed;

        public Analysis AnalysisResults { get; private set; } = new();

        public TextSummary ChatSummary { get; private set; } = new();

        public bool IsConnectedToServer { get; private set; } = false;

        public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

        public SentimentResult SentimentResult { get; private set; } = new SentimentResult();

        public bool ConnectToServer(string serverIp, int serverPort, string userName, string userEmail, string userPhotoUrl)
        {
            if (IsConnectedToServer)
            {
                return true;
            }
            _serverIp = serverIp;
            _serverPort = serverPort;
            Trace.WriteLine("Dashboard Client >>> Connecting to server at IP: " +
                             serverIp + " Port: " + serverPort);

            if (string.IsNullOrWhiteSpace(userName))
            {
                Trace.WriteLine("Dashboard Client >>> Null username received");
                return false;
            }
            lock (this)
            {
                Trace.WriteLine("Dashboard Client >>> Connecting to server");
                _userInfo.UserId = -1;
                _userInfo.UserName = userName;
                _userInfo.UserEmail = userEmail;
                _userInfo.UserPhotoUrl = userPhotoUrl;
                string connected = _communicator.Start(serverIp, serverPort.ToString());
                if (connected == "failure")
                {
                    Trace.WriteLine("Dashboard Client >>> Connection failed");
                }
                else
                {
                    IsConnectedToServer = true;
                    Trace.WriteLine("Dashboard Client >>> Connection succeeded");
                }
            }
            return IsConnectedToServer;
        }

        public void SendRefreshRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server for any updates");
            SendPayloadToServer(Operation.Refresh);
            Trace.WriteLine("Dashboard Client >>> Requested server for any updates");
        }

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
                HandleOperation(serverPayload);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Client >>> Exception " + e);
            }
        }

        private void HandleOperation(ServerPayload serverPayload)
        {
            Operation operationType = serverPayload.Operation;
            switch (operationType)
            {
                case Operation.GiveUserDetails:
                    HandleGiveUserDetailsOperation(serverPayload);
                    break;
                case Operation.SessionUpdated:
                    HandleSessionUpdated(serverPayload);
                    break;
                case Operation.Refresh:
                    Refresh(serverPayload);
                    break;
                case Operation.RemoveClient:
                case Operation.EndSession:
                    Refresh(serverPayload);
                    ExitSession();
                    break;
                default:
                    break;
            }
        }

        private void HandleSessionUpdated(ServerPayload serverPayload)
        {
            if (serverPayload.SessionInfo == null) 
            { 
                Trace.WriteLine("Dashboard Client >>> Null session info received");
                return;
            }
            UpdateSessionData(serverPayload.SessionInfo);
        }

        private void HandleGiveUserDetailsOperation(ServerPayload serverPayload)
        {
            if (serverPayload.UserInfo == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null user info in GiveUser operation.");
                return;
            }
            int userId = serverPayload.UserInfo.UserId;
            Trace.WriteLine("Dashboard Client >>> Setting user info.");
            _userInfo.UserId = userId;
            _screenshareClient.SetUser(_userInfo.UserId, _userInfo.UserName);
            _contentClient.SetUser(_userInfo.UserId, _userInfo.UserName, _serverIp, _serverPort);
            Trace.WriteLine("Dashboard Client >>> User info set.");
            SendPayloadToServer(Operation.TakeUserDetails);
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

        public void SendExitSessionRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server to let client exit.");
            SendPayloadToServer(Operation.RemoveClient);
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

        private void SendPayloadToServer(Operation operation)
        {
            Trace.WriteLine("Dashboard Client >>> Sending data to server.");
            lock (this)
            {
                ClientPayload clientPayload = new(operation, _userInfo);
                string serializedData = _serializer.Serialize(clientPayload);
                _communicator.Send(serializedData, _moduleName, null);
            }
            Trace.WriteLine("Dashboard Client >>> Data sent to server.");
        }

        private void ExitSession()
        {
            Trace.WriteLine("Dashboard Client >>> Exiting session");
            _communicator.Stop();
            IsConnectedToServer = false;
            SessionExited?.Invoke(this, new(ChatSummary, SentimentResult, AnalysisResults));
            Trace.WriteLine("Dashboard Client >>> Exited session");
        }

        private void UpdateTelemetryAnalysis(Analysis? analysis)
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

        private void UpdateSentiment(SentimentResult? sentiment)
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

        private void UpdateSummary(TextSummary? textSummary)
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

        private void UpdateSessionData(SessionInfo? sessionInfo)
        {
            if (sessionInfo == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null session info.");
                return;
            }
            Trace.WriteLine("Dashboard Client >>> Updating Session information.");
            bool modeChanged = false;
            lock (this)
            {
                modeChanged = sessionInfo.SessionMode != SessionInfo.SessionMode;
                SessionInfo = sessionInfo;
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
