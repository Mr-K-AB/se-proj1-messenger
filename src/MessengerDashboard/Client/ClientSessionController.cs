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
using System.Threading;
using MessengerNetworking.Communicator;
using System.Security.RightsManagement;
using MessengerNetworking;
using MessengerNetworking.NotificationHandler;
using System.Windows;
using System.Net.Sockets;
using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Server;
using MessengerDashboard.Client.Events;
using MessengerNetworking.Factory;
using MessengerScreenshare.ScreenshareFactory;
using MessengerScreenshare.Client;
using MessengerContent.Client;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using SentimentAnalyzer.Models;
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

        private readonly IScreenshareClient _screenshareClient = ScreenshareFactory.getInstance();

        private readonly Serializer _serializer = new();

        private string _serverIp = string.Empty;

        private int _serverPort;

        private UserInfo? _user;

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

        public event EventHandler<AnalysisChangedEventArgs> TelemetryAnalysisChanged;

        public event EventHandler<ClientSessionChangedEventArgs> SessionChanged;

        public event EventHandler<SessionExitedEventArgs> SessionExited;

        public event EventHandler<SummaryChangedEventArgs> SummaryChanged;

        public event EventHandler<SentimentChangedEventArgs> SentimentChanged;

        public event EventHandler<SessionModeChangedEventArgs> SessionModeChanged;

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
            string clientUsername,
            string clientEmail,
            string clientPhotoUrl
        )
        {
            _serverIp = serverIpAddress;
            _serverPort = serverPort;
            Trace.WriteLine("Dashboard: Connecting to server at IP: " + serverIpAddress + " Port: " + serverPort);

            if (string.IsNullOrWhiteSpace(clientUsername))
            {
                return false;
            }
            lock (this)
            {
                Trace.WriteLine("Dashboard: Sending Connection Request");
                UserInfo userInfo = new(clientUsername, -1, clientEmail, clientPhotoUrl);
                ClientPayload clientPayload = new(Operation.AddClient, ConnectionDetails.IpAddress, ConnectionDetails.Port, userInfo);
                string serializedMessage = _serializer.Serialize(clientPayload);
                _communicator.SendMessage(serverIpAddress, serverPort, _serverModuleIdentifier, serializedMessage, 1);
                Trace.WriteLine("Dashboard: Sent Connection Request");
            }
            bool connected;
            if (timeoutInMilliseconds == null)
            {
                connected = _connectionEstablished.WaitOne();
            }
            else
            {
                connected = _connectionEstablished.WaitOne((int)timeoutInMilliseconds);
            }
            if (connected)
            {
                _communicator.AddClient(serverIpAddress, serverPort);
            }
            return connected;
        }

        public void SendTelemetryAnalysisRequestToServer()
        {
            Trace.WriteLine("Dashboard Client: Requesting server for telemetry");
            SendPayloadToServer(Operation.GetTelemetryAnalysis, _user);
            Trace.WriteLine("Dashboard Client: Requested server for telemetry");
        }

        public void SendSummaryRequestToServer()
        {
            Trace.WriteLine("Dashboard Client: Requesting server for summary");
            SendPayloadToServer(Operation.GetSummary, _user);
            Trace.WriteLine("Dashboard Client: Requested server for summary");
        }

        public void SendSentimentRequestToServer()
        {
            Trace.WriteLine("Dashboard Client: Requesting server for sentiment");
            SendPayloadToServer(Operation.GetSentiment, _user);
            Trace.WriteLine("Dashboard Client: Requested server for sentiment");
        }

        public void OnClientJoined(string ipAddress, int port) { }

        public void OnClientLeft(string ipAddress, int port) { }

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                Trace.WriteLine("Dashboard Server: Received null serialized data from network");
                return;
            }

            Trace.WriteLine("Dashboard Server: Data received from network");
            try
            {
                ServerPayload serverPayload = _serializer.Deserialize<ServerPayload>(serializedData);
                Operation operationType = serverPayload.Operation;

                switch (operationType)
                {

                    case Operation.ExamMode:
                    case Operation.LabMode:
                        UpdateSessionData(serverPayload);
                        return;

                    case Operation.AddClientACK:
                        UpdateSessionData(serverPayload);
                        SetUser(serverPayload);
                        return;

                    case Operation.GetSummary:
                        UpdateSummary(serverPayload);
                        UpdateSessionData(serverPayload);
                        return;

                    case Operation.GetTelemetryAnalysis:
                        UpdateTelemetryAnalysis(serverPayload);
                        UpdateSessionData(serverPayload);
                        return;

                    case Operation.GetSentiment:
                        UpdateSentiment(serverPayload);
                        UpdateSessionData(serverPayload);
                        return;

                    case Operation.RemoveClient:
                    case Operation.EndSession:
                        UpdateSummary(serverPayload);
                        UpdateTelemetryAnalysis(serverPayload);
                        UpdateSentiment(serverPayload);
                        UpdateSessionData(serverPayload);
                        ExitSession();
                        return;

                    case Operation.SessionUpdated:
                        UpdateSessionData(serverPayload);
                        return;

                    default:
                        return;
                }
            }
            catch(Exception e)
            {
                Trace.WriteLine("Dashboard Client: Exception " + e);
            }
        }

        public void SendExitSessionRequestToServer()
        {
            Trace.WriteLine("Dashboard Client: Requesting server to let client exit.");
            SendPayloadToServer(Operation.RemoveClient, _user);
            Trace.WriteLine("Dashboard Client: Requested server to let client exit.");
        }

        public void SendLabModeRequestToServer()
        {
            SendPayloadToServer(Operation.LabMode, _user);
        }

        public void SendExamModeRequestToServer()
        {
            SendPayloadToServer(Operation.ExamMode, _user);
        }

        private void SetUser(ServerPayload serverPayload)
        {
            IsConnectedToServer = true;
            _connectionEstablished.Set();
            _user = serverPayload.User;
            _screenshareClient.SetUser(_user.UserId, _user.UserName);
            _contentClient.SetUser(_user.UserId, _user.UserName, _serverIp, _serverPort);
        }

        private void SendPayloadToServer(Operation operation, UserInfo userInfo)
        {
            ClientPayload clientPayload;
            lock (this)
            {
                clientPayload = new ClientPayload(operation, ConnectionDetails.IpAddress, ConnectionDetails.Port, userInfo);
                string serializedData = _serializer.Serialize(clientPayload);
                _communicator.Broadcast(_serverModuleIdentifier, serializedData);
            }
            Trace.WriteLine("Dashboard Client: Data sent to communicator");
        }

        private void ExitSession()
        {
            Trace.WriteLine("Dashboard Client: Exiting session");
            _communicator.RemoveClient(_serverIp, _serverPort);
            _communicator.RemoveSubscriber(_clientModuleIdentifier);
            SessionExited?.Invoke(this, new SessionExitedEventArgs(ChatSummary, SentimentResult, AnalysisResults));
            Trace.WriteLine("Dashboard Client: Exited session");
        }

        private void UpdateTelemetryAnalysis(ServerPayload serverPayload)
        {
            Trace.WriteLine("Dashboard Client: Updating telemetry analysis");
            if (serverPayload.SessionAnalysis == null)
            {
                Trace.WriteLine("Dashboard Client: Received null telemetry");
                return;
            }
            lock (this)
            {
                AnalysisResults = serverPayload.SessionAnalysis;
                TelemetryAnalysisChanged?.Invoke(this, new(AnalysisResults));
            }
            Trace.WriteLine("Dashboard Client: Updated telemetry analysis");
        }

        private void UpdateSentiment(ServerPayload serverPayload)
        {
            Trace.WriteLine("Dashboard Client: Updating Sentiment");
            if (serverPayload.Sentiment == null)
            {
                Trace.WriteLine("Dashboard Client: Received null summary");
                return;
            }
            lock (this)
            {
                SentimentResult = serverPayload.Sentiment;
                SentimentChanged?.Invoke(this, new SentimentChangedEventArgs(SentimentResult));
            }
            Trace.WriteLine("Dashboard Client: Updated Sentiment");
        }

        private void UpdateSummary(ServerPayload serverPayload)
        {
            Trace.WriteLine("Dashboard Client: Updating Summary");
            if (serverPayload.Summary == null)
            {
                Trace.WriteLine("Dashboard Client: Received null summary");
                return;
            }
            lock (this)
            {
                ChatSummary = serverPayload.Summary;
                SummaryChanged?.Invoke(this, new SummaryChangedEventArgs(ChatSummary));
            }
            Trace.WriteLine("Dashboard Client: Updated Summary");
        }

        private void UpdateSessionData(ServerPayload receivedData)
        {
            SessionInfo? receivedSessionData = receivedData.SessionInfo;
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
        }
    }
}
