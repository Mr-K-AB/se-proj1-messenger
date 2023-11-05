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
using MessengerDashboard.Client.User.Session;
using MessengerDashboard.Summarization;

namespace MessengerDashboard.Dashboard.User.Session
{
    public class ClientSessionController : INotificationHandler
    {
        private readonly List<IUserNotification> _users;
        private readonly ICommunicator _communicator;
        private readonly Serializer _serializer = new();
        private readonly string _moduleIdentifier = "Dashboard";
        private UserInfo? _user;
        private readonly SessionMode _sessionMode;
        //private readonly IContentUser _contentClient;
        //private readonly ScreenSharingUser _screenshareClient;

        public ClientSessionController()
        {
            // TODO: Get Communicatior
            //_communicator = CommunitatorFactory.GetCommunicator();
            
            // TODO: Get Content
            //_contentClient = ContentUserFactory.GetInstance();

            _communicator.AddSubscriber(_moduleIdentifier, this);
            _users ??= new List<IUserNotification>();
            // TODO: Add Screen share client
            //_screenshareClient = null;
            Trace.WriteLine("[Dashboard] Created Client Session Manager");
        }

        public ClientSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_moduleIdentifier, this);
            _users ??= new List<IUserNotification>();
            _sessionMode = SessionMode.Test;
        }

        public TextSummary? ChatSummary { get; private set; }

        public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

        public SessionAnalytics SessionAnalytics { get; private set; }


        public event EventHandler<ClientSessionChangedEventArgs> ClientSessionChanged;

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                Trace.WriteLine("[Dashboard] Null Serialized Data received from network");
                throw new ArgumentNullException("[Dashboard] Null SerializedObject as Argument");
            }

            Trace.WriteLine("[Dashboard] Data Received from Network");

            ServerPayload serverPayload = _serializer.Deserialize<ServerPayload>(serializedData);
            Operation operationType = serverPayload.eventType;

            switch (operationType)
            {
                case Operation.ToggleSessionMode:
                    UpdateClientSessionModeData(serverPayload);
                    return;

                case Operation.AddClient:
                    UpdateClientSessionData(serverPayload);
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

        private void SendDataToServer(Operation operation, string username, int userID = -1, string userEmail = null, string photoUrl = null)
        {
            ClientPayload clientPayload;
            lock (this)
            {
                clientPayload = new ClientPayload(operation, username, userID, userEmail, photoUrl);
                string serializedData = _serializer.Serialize(clientPayload);
                //_communicator.Send(serializedData, _moduleIdentifier, null);
            }
            Trace.WriteLine("[Dashboard] Data sent to Network module to transfer to Server");
        }

        public bool AddClient(string ipAddress, int port, string username, string email = null, string photoUrl = null)
        {
            Trace.WriteLine("[Dashboard] AddClient() is called");

            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

           // ipAddress = ipAddress.Trim();

            lock (this)
            {
                Trace.WriteLine("[Dashboard] Sending to Network for connecting");

                string connectionStatus = "";// _communicator.Start(ipAddress, port.ToString());

                if (connectionStatus == "failure")
                {
                    Trace.WriteLine("[Dashboard] Connection not established");
                    return false;
                }
            }
            Trace.WriteLine("[Dashboard] Connection established");
            _user = new(username, -1, email, photoUrl);
            return true;
        }

        public void EndMeet()
        {
            Trace.WriteLine("[Dashboard] End Meet is called from Dashboard UX. Sending to Server to End Meet");
            SendDataToServer(Operation.EndSession, _user.userName, _user.userID);
        }

        public void GetAnalytics()
        {
            Trace.WriteLine("[Dashboard] GetAnalytics() is called from Dashboard UX");
            SendDataToServer(Operation.GetAnalytics, _user.userName, _user.userID);
        }

        public void GetSummary()
        {
            Trace.WriteLine("[Dashboard] GetSummary() is called from Dashboard UX");
            SendDataToServer(Operation.GetSummary, _user.userName, _user.userID);
        }

        public UserInfo GetUser()
        {
            Trace.WriteLine("[Dashboard] GetUser() is Called from Dashboard UX");
            return _user;
        }

        public void ToggleSessionMode()
        {
            Trace.WriteLine("[Dashboard] ToggleSessionMode() is Called from Dashboard UX");
            SendDataToServer(Operation.ToggleSessionMode, _user.userName, _user.userID);
        }

        public void RemoveClient()
        {
            SendDataToServer(Operation.RemoveClient, _user.userName, _user.userID);
            Thread.Sleep(2000);
          //  _communicator.Stop();
            SessionEnded?.Invoke(this, EventArgs.Empty);
            if (_sessionMode == SessionMode.Lab)
            {
                CloseProgram();
            }
        }

        public void SubscribeSession(IUserNotification listener)
        {
            lock (this)
            {
                _users.Add(listener);
            }
        }

        public event EventHandler SessionEnded;
        public event EventHandler<SummaryChangedEventArgs> SummaryChanged;
        public event EventHandler<AnalyticsChangedEventArgs> AnalyticsChanged;
        public event EventHandler<SessionModeChangedEventArgs> SessionModeChanged;

        private void SetClientID(ServerPayload receivedData)
        {
            if (_user.userID == -1)
            {
                lock (this)
                {
                    _user.userID = receivedData._user.userID;
                    SendDataToServer(Operation.AddClient, _user.userName, _user.userID, _user.userEmail, _user.userPhotoUrl);
                    if (_sessionMode == SessionMode.Lab)
                    {
                        // ScreenShare's user ID and username set.
                        // _screenshareClient.SetUser(_user.userID.ToString(), _user.userName);
                        // Content's user ID set.
                    }
                }
            }
        }

        public void SetUser(string userName, int userID = 1, string userEmail = null, string photoUrl = null)
        {
            _user = new UserInfo(userName, userID, userEmail, photoUrl);
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
            SummaryDetail receivedSummary = receivedData.summaryDetail;
            UserInfo receivedUser = receivedData.GetUser();
            if (receivedUser.userID == _user.userID)
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
            Trace.WriteLine("[Dashboard] Calling Network to Stop listening");
            //   _communicator.Stop();

            SessionEnded?.Invoke(this, EventArgs.Empty);

            Trace.WriteLine("[Dashboard] Shutdown Application");

            if (_sessionMode == SessionMode.Lab)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                });
            }
        }

        public void OnClientJoined(TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
