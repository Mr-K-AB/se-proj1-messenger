using System;
using MessengerDashboard;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MessengerNetworking.Communicator;
using System.Security.RightsManagement;

using MessengerDashboard.Dashboard;
using MessengerNetworking;
using MessengerNetworking.NotificationHandler;
using System.Windows;
using System.Net.Sockets;
using MessengerDashboard.Telemetry;

namespace MessengerDashboard.Dashboard.User.Session
{
    public delegate void NotifyEndMeet();

    public delegate void NotifyAnalyticsCreated(SessionAnalytics analytics);

    public delegate void NotifySummaryCreated(string summary);

    public delegate void NotifySessionTypeChanged(string sessionMode);

    public class UserSessionManagement : INotificationHandler
    {
        private readonly List<IUserNotification> _users;
        private readonly ICommunicator _communicator;
        //private readonly IContentUser _contentClient;
        private readonly IDashboardSerializer _serializer;
        private readonly string _moduleIdentifier;
        private string _chatSummary;
        public SessionInfo userSessionInfo;

        //private readonly ScreenSharingUser _screenshareClient;
        private SessionAnalytics _sessionAnalytics;
        private UserInfo _user;
        private readonly bool _testmode;

        public UserSessionManagement()
        {
            _moduleIdentifier = "Dashboard";
            _serializer = new DashboardSerializer();
            //_communicator = CommunitatorFactory.GetCommunicator();
            //_communicator.Subscribe(_moduleIdentifier, this);
            //_contentClient = ContentUserFactory.GetInstance();
            _users ??= new List<IUserNotification>();
            userSessionInfo = new SessionInfo();
            _user = null;
            _chatSummary = null;
            //_screenshareClient = null;
            Trace.WriteLine("[Dashboard] Created Client Session Manager");
        }

        public UserSessionManagement(ICommunicator communicator)
        {
            _moduleIdentifier = "Dashboard";
            _serializer = new DashboardSerializer();
            _communicator = communicator;
            //_communicator.Subscribe(_moduleIdentifier, this);
            _users ??= new List<IUserNotification>();
            userSessionInfo = new SessionInfo();
            _chatSummary = null;
            _testmode = true;
        }

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                Trace.WriteLine("[Dashboard] Null Serialized Data received from network");
                throw new ArgumentNullException("[Dashboard] Null SerializedObject as Argument");
            }

            Trace.WriteLine("[Dashboard] Data Received from Network");

            ServerToUserInfo deserializedObject = null;//_serializer.Deserialize<ServerToUserInfo>(serializedData);
            string eventType = "";//deserializedObject.eventType;

            switch (eventType)
            {
                case "toggleSessionMode":
                    UpdateClientSessionModeData(deserializedObject);
                    return;

                case "addClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "getSummary":
                    UpdateSummary(deserializedObject);
                    return;

                case "getAnalytics":
                    UpdateAnalytics(deserializedObject);
                    return;

                case "removeClient":
                    UpdateClientSessionData(deserializedObject);
                    return;

                case "endMeet":
                    CloseProgram();
                    return;

                case "newID":
                    SetClientID(deserializedObject);
                    return;

                default:
                    return;
            }
        }

        private void SendDataToServer(string eventName, string username, int userID = -1, string userEmail = null, string photoUrl = null)
        {
            UserToServerInfo userToServerInfo;
            lock (this)
            {
                userToServerInfo = new UserToServerInfo(eventName, username, userID, userEmail, photoUrl);
                string serializedData = _serializer.Serialize(userToServerInfo);
                _communicator.Send(serializedData, _moduleIdentifier, null);
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
            SendDataToServer("endMeet", _user.userName, _user.userID);
        }

        public void GetAnalytics()
        {
            Trace.WriteLine("[Dashboard] GetAnalytics() is called from Dashboard UX");
            SendDataToServer("getAnalytics", _user.userName, _user.userID);
        }

        public void GetSummary()
        {
            Trace.WriteLine("[Dashboard] GetSummary() is called from Dashboard UX");
            SendDataToServer("getSummary", _user.userName, _user.userID);
        }

        public UserInfo GetUser()
        {
            Trace.WriteLine("[Dashboard] GetUser() is Called from Dashboard UX");
            return _user;
        }

        public void ToggleSessionMode()
        {
            Trace.WriteLine("[Dashboard] ToggleSessionMode() is Called from Dashboard UX");
            SendDataToServer("toggleSession", _user.userName, _user.userID);
        }

        public void RemoveClient()
        {
            SendDataToServer("removeClient", _user.userName, _user.userID);
            Thread.Sleep(2000);
          //  _communicator.Stop();
            MeetingEnded?.Invoke();
            if (_testmode == false)
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

        public event NotifyEndMeet MeetingEnded;
        public event NotifySummaryCreated SummaryCreated;
        public event NotifyAnalyticsCreated AnalyticsCreated;
        public event NotifySessionTypeChanged SessionModeChanged;

        public SessionInfo GetSessionData()
        {
            Trace.WriteLine("[Dashboard] Sending Session Data to Caller");
            return userSessionInfo;
        }

        public SessionAnalytics GetStoredAnalytics()
        {
            return _sessionAnalytics;
        }

        public string GetStoredSummary()
        {
            return _chatSummary;
        }

        public void NotifyUXSession()
        {
            for (int i = 0; i < _users.Count; ++i)
            {
                lock (this)
                {
                    Trace.WriteLine("[Dashboard] Notifying subscribed UX about the session change");
                    _users[i].OnUserSessionChange(userSessionInfo);
                }
            }
        }

        private void SetClientID(ServerToUserInfo receivedData)
        {
            if (_user.userID == -1)
            {
                lock (this)
                {
                    _user.userID = receivedData._user.userID;
                    SendDataToServer("addClient", _user.userName, _user.userID, _user.userEmail, _user.userPhotoUrl);
                    if (_testmode == false)
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
                userSessionInfo.AddUser(users[i]);
            }
        }

        private void UpdateAnalytics(ServerToUserInfo receivedData)
        {
            _sessionAnalytics = receivedData.sessionAnalytics;
            UserInfo receiveduser = receivedData.GetUser();
            Trace.WriteLine("Notifying UX about the Analytics");
            AnalyticsCreated?.Invoke(_sessionAnalytics);
        }

        private void UpdateSummary(ServerToUserInfo receivedData)
        {
            SummaryDetail receivedSummary = receivedData.summaryDetail;
            UserInfo receivedUser = receivedData.GetUser();
            if (receivedUser.userID == _user.userID)
            {
                lock (this)
                {
                    _chatSummary = receivedSummary.detail;
                    Trace.WriteLine("Notifying UX about the summary");
                    SummaryCreated?.Invoke(_chatSummary);
                }
            }
        }

        private void UpdateClientSessionModeData(ServerToUserInfo receivedData)
        {
            
           SessionInfo receivedSessionData = receivedData.sessionData;
            lock (this)
            {
                userSessionInfo = receivedSessionData;
            }
            SessionModeChanged?.Invoke(userSessionInfo.sessionType);
            NotifyUXSession();
        }

        private void UpdateClientSessionData(ServerToUserInfo receivedData)
        {
            
            SessionInfo? receivedSessionData = receivedData.sessionData;
            UserInfo user = receivedData.GetUser();
            if (receivedSessionData != null && userSessionInfo != null && receivedSessionData.users.Equals(userSessionInfo.users))
            {
                return;
            }

            if (_user == null)
            {
                _user = user;
            }
            else if (_user.Equals(user) && receivedData.eventType == "removeClient")
            {
                _user = null;
                receivedSessionData = null;
            }

            lock (this)
            {
                userSessionInfo = receivedSessionData;
            }
            
            NotifyUXSession();
        }

        public void CloseProgram()
        {
            Trace.WriteLine("[Dashboard] Calling Network to Stop listening");
         //   _communicator.Stop();
            MeetingEnded?.Invoke();
            Trace.WriteLine("[Dashboard] Shutdown Application");

            if (_testmode == false)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Application.Current.Shutdown();
                    System.Environment.Exit(0);
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
