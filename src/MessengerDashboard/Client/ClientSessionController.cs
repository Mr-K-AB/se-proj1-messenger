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

namespace MessengerDashboard.Client
{
    public class ClientSessionController : INotificationHandler
    {
        private readonly ICommunicator _communicator;
        private readonly Serializer _serializer = new();
        private readonly string _moduleIdentifier = "Dashboard";
        private UserInfo? _user;
        private readonly SessionMode _sessionMode;
        public ConnectionDetails ConnectionDetails { get; private set; }

        public bool IsConnectedToServer { get; private set; } = false;
        //private readonly IContentUser _contentClient;
        //private readonly ScreenSharingUser _screenshareClient;

        public ClientSessionController()
        {
            // TODO: Get Communicatior
            //_communicator = CommunitatorFactory.GetCommunicator();

            // TODO: Get Content
            //_contentClient = ContentUserFactory.GetInstance();

            _communicator.AddSubscriber(_moduleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
            // TODO: Add Screen share client
            //_screenshareClient = null;
            Trace.WriteLine("Dashboard: Created Client Session Manager");
        }

        public ClientSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_moduleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }

        public TextSummary? ChatSummary { get; private set; }

        public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

        public SessionAnalytics SessionAnalytics { get; private set; }

        public event EventHandler SessionEnded;

        public event EventHandler<SummaryChangedEventArgs> SummaryChanged;

        public event EventHandler<AnalyticsChangedEventArgs> AnalyticsChanged;

        public event EventHandler<SessionModeChangedEventArgs> SessionModeChanged;

        public event EventHandler<ClientSessionChangedEventArgs> ClientSessionChanged;

        public ManualResetEvent ConnectionEstablished = new(false);

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                Trace.WriteLine("Dashboard: Null Serialized Data received from network");
                throw new ArgumentNullException("Dashboard: Null SerializedObject as Argument");
            }

            Trace.WriteLine("Dashboard: Data Received from Network");

            ServerPayload serverPayload = _serializer.Deserialize<ServerPayload>(serializedData);
            Operation operationType = serverPayload.eventType;

            switch (operationType)
            {
                case Operation.ToggleSessionMode:
                    UpdateClientSessionModeData(serverPayload);
                    return;

                case Operation.AddClientACK:
                    UpdateClientSessionData(serverPayload);
                    IsConnectedToServer = true;
                    ConnectionEstablished.Set();
                    return;

                case Operation.GetSummary:
                    UpdateSummary(serverPayload);
                    return;

                case Operation.GetAnalytics:
                    UpdateAnalytics(serverPayload);
                    return;

                case Operation.RemoveClient:
                    UpdateClientSessionData(serverPayload);
                    return;

                case Operation.EndSession:
                    CloseProgram();
                    return;

                case Operation.ID:
                    SetClientID(serverPayload);
                    return;

                default:
                    return;
            }
        }

        private void DeliverPayloadToServer(Operation operation, string username, int UserID = -1, string UserEmail = null, string photoUrl = null)
        {
            ClientPayload clientPayload;
            lock (this)
            {
                clientPayload = new ClientPayload(operation, username, ConnectionDetails.IpAddress, ConnectionDetails.Port, UserID, UserEmail, photoUrl);
                string serializedData = _serializer.Serialize(clientPayload);
                //_communicator.Send(serializedData, _moduleIdentifier, null);
            }
            Trace.WriteLine("Dashboard: Data sent to Network module to transfer to Server");
        }

        public void RequestConnectionToServer(string serverIpAddress, int serverPort, string clientUsername, string clientEmail = null, string clientPhotoUrl = null)
        {
            Trace.WriteLine("Dashboard: Connecting to server at IP: " + serverIpAddress + " Port: " + serverPort);

            if (string.IsNullOrWhiteSpace(clientUsername))
            {
                return;
            }

            // ipAddress = ipAddress.Trim();

            lock (this)
            {
                Trace.WriteLine("Dashboard: Sending add client to server");
                ClientPayload clientPayload = new(Operation.AddClient, clientUsername, ConnectionDetails.IpAddress, ConnectionDetails.Port, -1, clientEmail, clientPhotoUrl);
                string serializedMessage = _serializer.Serialize(clientPayload);
                _communicator.SendMessage(serverIpAddress, serverPort, _moduleIdentifier, serializedMessage);
                Trace.WriteLine("Dashboard: Sent Connection Request");
            }
        }

        public void EndMeet()
        {
            Trace.WriteLine("Dashboard: End Meet is called from Dashboard UX. Sending to Server to End Meet");
            DeliverPayloadToServer(Operation.EndSession, _user.UserName, _user.UserID);
        }

        public void GetAnalytics()
        {
            Trace.WriteLine("Dashboard: GetAnalytics() is called from Dashboard UX");
            DeliverPayloadToServer(Operation.GetAnalytics, _user.UserName, _user.UserID);
        }

        public void GetSummary()
        {
            Trace.WriteLine("Dashboard: GetSummary() is called from Dashboard UX");
            DeliverPayloadToServer(Operation.GetSummary, _user.UserName, _user.UserID);
        }

        public UserInfo GetUser()
        {
            Trace.WriteLine("Dashboard: GetUser() is Called from Dashboard UX");
            return _user;
        }

        public void ToggleSessionMode()
        {
            Trace.WriteLine("Dashboard: ToggleSessionMode() is Called from Dashboard UX");
            DeliverPayloadToServer(Operation.ToggleSessionMode, _user.UserName, _user.UserID);
        }

        public void RemoveClient()
        {
            DeliverPayloadToServer(Operation.RemoveClient, _user.UserName, _user.UserID);
            Thread.Sleep(2000);
            //  _communicator.Stop();
            SessionEnded?.Invoke(this, EventArgs.Empty);
        }

        private void SetClientID(ServerPayload receivedData)
        {
            if (_user.UserID == -1)
            {
                lock (this)
                {
                    _user.UserID = receivedData._user.UserID;
                    DeliverPayloadToServer(Operation.AddClient, _user.UserName, _user.UserID, _user.UserEmail, _user.UserPhotoUrl);
                        // ScreenShare's user ID and username set.
                        // _screenshareClient.SetUser(_user.UserID.ToString(), _user.UserName);
                        // Content's user ID set.
                }
            }
        }

        public void SetUser(string UserName, int UserID = 1, string UserEmail = null, string photoUrl = null)
        {
            _user = new UserInfo(UserName, UserID, UserEmail, photoUrl);
        }

        public void SetSessionUsers(List<UserInfo> users)
        {
            for (int i = 0; i < users.Count; ++i)
            {
                SessionInfo.AddUser(users[i]);
            }
        }

        private void UpdateAnalytics(ServerPayload receivedData)
        {
            SessionAnalytics = receivedData.sessionAnalytics;
            UserInfo receiveduser = receivedData.GetUser();
            Trace.WriteLine("Notifying UX about the Analytics");
            AnalyticsChanged?.Invoke(this, new AnalyticsChangedEventArgs()
            {
                SessionAnalytics = SessionAnalytics
            });
        }

        private void UpdateSummary(ServerPayload receivedData)
        {
            TextSummary receivedSummary = receivedData.summaryDetail;
            UserInfo receivedUser = receivedData.GetUser();
            if (receivedUser.UserID == _user.UserID)
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

        private void UpdateClientSessionModeData(ServerPayload receivedData)
        {

            SessionInfo receivedSessionData = receivedData.sessionData;
            lock (this)
            {
                SessionInfo = receivedSessionData;
            }
            SessionModeChanged?.Invoke(this, new SessionModeChangedEventArgs());
            ClientSessionChanged?.Invoke(this, new ClientSessionChangedEventArgs());
        }

        private void UpdateClientSessionData(ServerPayload receivedData)
        {

            SessionInfo? receivedSessionData = receivedData.sessionData;
            UserInfo user = receivedData.GetUser();
            if (receivedSessionData != null && SessionInfo != null && receivedSessionData.Users.Equals(SessionInfo.Users))
            {
                return;
            }

            if (_user == null)
            {
                _user = user;
            }
            else if (_user.Equals(user) && receivedData.eventType == Operation.RemoveClient)
            {
                _user = null;
                receivedSessionData = null;
            }

            lock (this)
            {
                SessionInfo = receivedSessionData;
            }

            ClientSessionChanged?.Invoke(this, new ClientSessionChangedEventArgs());
        }

        public void CloseProgram()
        {
            Trace.WriteLine("Dashboard: Calling Network to Stop listening");
            //   _communicator.Stop();

            SessionEnded?.Invoke(this, EventArgs.Empty);

            Trace.WriteLine("Dashboard: Shutdown Application");

            Application.Current.Dispatcher.Invoke(delegate
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
            });
        }

        public void OnClientJoined(TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }

        public void OnClientJoined(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }
    }
}
