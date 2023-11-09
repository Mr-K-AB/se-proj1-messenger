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

namespace MessengerDashboard.Client
{
    public class ClientSessionController : INotificationHandler
    {
        private readonly ICommunicator _communicator = Factory.GetInstance();

        private readonly ManualResetEvent _connectionEstablished = new(false);

        private readonly string _moduleIdentifier = "Dashboard";

        private readonly Serializer _serializer = new();

        private readonly ManualResetEvent _sessionExited = new(false);

        private readonly SessionMode _sessionMode;

        private string _serverIp = string.Empty;

        private int _serverPort;

        private ClientInfo? _user;

        private IScreenshareClient _screenshareClient = ScreenshareFactory.getInstance();

        public ClientSessionController()
        {
            _communicator.AddSubscriber(_moduleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
            Trace.WriteLine("Dashboard: Created Client Session Manager");
        }

        public ClientSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_moduleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }

        public event EventHandler<AnalyticsChangedEventArgs>? AnalyticsChanged;

        public event EventHandler<ClientSessionChangedEventArgs>? ClientSessionChanged;

        public event EventHandler? SessionEnded;

        public event EventHandler? SessionExited;

        public event EventHandler<SessionModeChangedEventArgs>? SessionModeChanged;

        public event EventHandler<SummaryChangedEventArgs>? SummaryChanged;

        public Analysis? AnalysisResults { get; private set; }

        public TextSummary? ChatSummary { get; private set; }

        public ConnectionDetails ConnectionDetails { get; private set; }

        public bool IsConnectedToServer { get; private set; } = false;

        public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

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
                Trace.WriteLine("Dashboard: Sending add client to server");
                ClientPayload clientPayload = new(Operation.AddClient, clientUsername, ConnectionDetails.IpAddress, ConnectionDetails.Port, -1, clientEmail, clientPhotoUrl);
                string serializedMessage = _serializer.Serialize(clientPayload);
                _communicator.SendMessage(serverIpAddress, serverPort, _moduleIdentifier, serializedMessage);
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
            _communicator.AddClient(serverIpAddress, serverPort);
         
            return connected;
        }

        public void GetAnalytics()
        {
            Trace.WriteLine("Dashboard: GetAnalytics() is called from Dashboard UX");
            DeliverPayloadToServer(Operation.GetAnalytics, _user.ClientId, _user.ClientName);
        }

        public void GetSummary()
        {
            Trace.WriteLine("Dashboard: GetSummary() is called from Dashboard UX");
            DeliverPayloadToServer(Operation.GetSummary, _user.ClientId, _user.ClientName);
        }

        public ClientInfo GetUser()
        {
            Trace.WriteLine("Dashboard: GetUser() is Called from Dashboard UX");
            return _user;
        }

        public void OnClientJoined(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                Trace.WriteLine("Dashboard: Null Serialized Data received from network");
                return;
            }

            Trace.WriteLine("Dashboard: Data Received from Network");

            ServerPayload serverPayload = _serializer.Deserialize<ServerPayload>(serializedData);
            Operation operationType = serverPayload.Operation;

            switch (operationType)
            {
                case Operation.ToggleSessionMode:
                    UpdateClientSessionModeData(serverPayload);
                    return;

                case Operation.AddClientACK:
                    UpdateClientSessionData(serverPayload);
                    IsConnectedToServer = true;
                    _connectionEstablished.Set();
                    _user = serverPayload.User;
                    _screenshareClient.SetUser(_user.ClientId, _user.ClientName);
                    return;

                case Operation.GetSummary:
                    UpdateSummary(serverPayload);
                    return;

                case Operation.GetAnalytics:
                    //UpdateTelemetryAnalysis(serverPayload);
                    return;

                case Operation.RemoveClient:
                    UpdateClientSessionData(serverPayload);
                    ExitSession();
                    return;

                case Operation.EndSession:
                    SessionEnded?.Invoke(this, EventArgs.Empty);
                    ExitSession();
                    return;

                default:
                    return;
            }
        }

        public bool RequestServerToRemoveClient(int? timeout)
        {
            DeliverPayloadToServer(Operation.RemoveClient, _user.ClientId, _user.ClientName);
            bool exited;
            if (timeout is null)
            {
                _sessionExited.WaitOne();
                exited = true;
            }
            else
            {
                exited = _sessionExited.WaitOne((int)timeout);
            }
            _communicator.RemoveClient(_serverIp, _serverPort);
            return exited;
        }

        public void SetSessionUsers(List<ClientInfo> users)
        {
            for (int i = 0; i < users.Count; ++i)
            {
                SessionInfo.Users.Add(users[i]);
            }
        }

        public void SetUser(string UserName, int UserID = 1, string UserEmail = null, string photoUrl = null)
        {
            _user = new ClientInfo(UserName, UserID, UserEmail, photoUrl);
        }

        public void ToggleSessionMode()
        {
            Trace.WriteLine("Dashboard: ToggleSessionMode() is Called from Dashboard UX");
            DeliverPayloadToServer(Operation.ToggleSessionMode, _user.ClientId, _user.ClientName);
        }

        private void DeliverPayloadToServer(Operation operation, int userId, string? username, string? userEmail = null, string photoUrl = null)
        {
            ClientPayload clientPayload;
            lock (this)
            {
                clientPayload = new ClientPayload(operation, username, ConnectionDetails.IpAddress, ConnectionDetails.Port, userId, userEmail, photoUrl);
                string serializedData = _serializer.Serialize(clientPayload);
                _communicator.Broadcast(_moduleIdentifier, serializedData);
            }
            Trace.WriteLine("Dashboard: Data sent to Networking Module who will tranfer it to server");
        }

        private void ExitSession()
        {
            Trace.WriteLine("Dashboard: Ended session");
            _sessionExited.Set();
            SessionExited?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateAnalytics(ServerPayload receivedData)
        {
            AnalysisResults = receivedData.SessionAnalysis;
            ClientInfo receiveduser = receivedData.User;
            AnalyticsChanged?.Invoke(this, new(AnalysisResults));
        }

        private void UpdateClientSessionData(ServerPayload receivedData)
        {

            SessionInfo? receivedSessionData = receivedData.SessionInfo;
            lock (this)
            {
                SessionInfo = receivedSessionData;
            }
            ClientSessionChanged?.Invoke(this, new ClientSessionChangedEventArgs());
        }

        private void UpdateClientSessionModeData(ServerPayload receivedData)
        {

            SessionInfo receivedSessionData = receivedData.SessionInfo;
            lock (this)
            {
                SessionInfo = receivedSessionData;
            }
            SessionModeChanged?.Invoke(this, new SessionModeChangedEventArgs());
            ClientSessionChanged?.Invoke(this, new ClientSessionChangedEventArgs());
        }

        private void UpdateSummary(ServerPayload receivedData)
        {
            TextSummary receivedSummary = receivedData.Summary;
            ClientInfo receivedUser = receivedData.User;
            if (receivedUser.ClientId == _user.ClientId)
            {
                lock (this)
                {
                    ChatSummary = null;//receivedSummary.detail;
                    Trace.WriteLine("Notifying UX about the summary");
                    SummaryChanged?.Invoke(this, new SummaryChangedEventArgs()
                    {
                        Summary = null
                    });
                }
            }
        }


    }
}
