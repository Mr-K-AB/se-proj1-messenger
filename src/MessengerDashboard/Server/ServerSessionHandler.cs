using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Dashboard;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerNetworking.Communicator;
using System.Net;
using MessengerNetworking.NotificationHandler;
using MessengerDashboard.Telemetry;
using System.Diagnostics;
using System.Threading;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Handles the server session.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IServerSessionHandler"/>
    /// </remarks>
    public class ServerSessionHandler : IServerSessionHandler    
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionHandler"/> with the provided <see cref="ICommunicator"/> instance.
        /// </summary>
        /// <param name="communicator">An <see cref="ICommunicator "/> implementation for server communication.</param>
        public ServerSessionHandler(ICommunicator communicator)
        {
            _communicator = communicator;
        }


        public SessionMode SessionMode { get; private set; }

        private readonly ICommunicator _communicator;
        private readonly ITextSummarizer _textSummarizer = TextSummarizerFactory.GetTextSummarizer();
        private readonly ISentimentAnalyzer _sentimentAnalyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();
        private readonly Serializer _serializer = new();
        private int _clientCount = 0;
        private SessionInfo _sessionInfo;
        private readonly SessionMode _sessionMode;
        private TextSummary? _chatSummary;
        public event EventHandler<SessionUpdatedEventArgs> SessionUpdated;

        /// <summary>
        /// Safely ends the meeting.
        /// </summary>
        public void EndMeet()
        {
            // Calculate the summary from the chats
            // Save the summary
        }

        public void OnDataReceived(string serializedData)
        {
            if (serializedData == null)
            {
                throw new ArgumentNullException("Null data received");
            }
            ClientPayload clientPayload = _serializer.Deserialize<ClientPayload>(serializedData);

            if (clientPayload == null || clientPayload.username == null)
            {
                throw new ArgumentNullException("Null user received");
            }
            Operation operationType = clientPayload.eventType;
            switch (operationType)
            {
                case Operation.ToggleSessionMode:
                    break;
                case Operation.AddClient:
                    break;
                case Operation.GetSummary:
                    break;
                case Operation.GetAnalytics:
                    break;
                case Operation.RemoveClient:
                    break;
                case Operation.EndSession:
                    break;
                default:
                    break;
            }
        }

        public void DeliverPayloadToClient(Operation operation, SessionInfo? sessionInfo,
            TextSummary? summary = null, SessionAnalytics? sessionAnalytics = null, 
            UserInfo? user = null, int userID = -1)
        {
            // serialize the payload
            
            // send through communicator
        }

        public void OnClientJoined(TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }

        /*

        //    Telemetry will Subscribes to changes in the session object
        public void Subscribe(ITelemetryNotifications listener)
        {
            lock (this)
            {
                _telemetrySubscribers.Add(listener);
            }
        }


        //     Returns the credentials required to Join the meeting
        public MeetingCredentials GetPortsAndIPAddress()
        {
            try
            {
                Trace.WriteLine("[Dashboard] Asking Network to create Server");
                var meetAddress = _communicator.Start();

                // Invalid credentials results in a returning a null object
                if (IsValidIPAddress(meetAddress) != true)
                {
                    return null;
                }

                // For valid IP address, a MeetingCredentials Object is created and returned
                var ipAddress = meetAddress[..meetAddress.IndexOf(':')];
                var port = Convert.ToInt32(meetAddress[(meetAddress.IndexOf(':') + 1)..]);

                _meetingCredentials = new MeetingCredentials(ipAddress, port);
                Trace.WriteLine("[Dashboard] Server Created Succesfully");
                return _meetingCredentials;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
        }
        */

        public event EventHandler SessionEnded;

        private void AddUser(UserInfo user)
        {
            lock (this)
            {
                _sessionInfo.AddUser(user);
            }
        }

        private void ToggleSessionMode()
        {
            Trace.WriteLine("[Dashboard Server] Session Mode changed in Session Data");
            DeliverPayloadToClient(Operation.ToggleSessionMode, _sessionInfo);
        }

        public void ServeNewClient(ClientPayload arrivedClient)
        {
            UserInfo user = new()
            {
                userName = arrivedClient.username, userID = arrivedClient.userID,
                userEmail = arrivedClient.userEmail, userPhotoUrl = arrivedClient.photoUrl
            };
            //AddUserToSession(user);
            NewUserAdded?.Invoke(this, EventArgs.Empty);
            DeliverPayloadToClient(Operation.AddClient, _sessionInfo, null, null, user);
        }

        public event EventHandler NewUserAdded;

        private TextSummary CreateSummary()
        {
            Trace.WriteLine("Dashboard: Getting chats");
            // TODO : GET chats
            Trace.WriteLine("Dashboard: Creating Summary");
            string[] chats = new string[] { string.Empty };
            TextSummarizationOptions options = new();
            _chatSummary = _textSummarizer.Summarize(chats, options);
            Trace.WriteLine("Dashboard: Created Summary");
            return _chatSummary;
        }

        private void EndSession(ClientPayload receivedObject)
        {
            Trace.WriteLine("Dashboard: Ending Session");
            try
            {
                Trace.WriteLine("Dashboard: Saving summary and analytics");

                // TODO : Summary saving

                Trace.WriteLine("Dashboard: Summary Saved");
                DeliverPayloadToClient(Operation.EndSession, _sessionInfo);
            }
            catch (Exception)
            {
                DeliverPayloadToClient(Operation.EndSession, _sessionInfo);
            }
            Thread.Sleep(2000);
            // TODO : STOP COMMUNICATOR
            SessionEnded?.Invoke(this, EventArgs.Empty);
        }

        private void GetAnalytics(ClientPayload receivedObject)
        {
            UserInfo user = new(receivedObject.username, receivedObject.userID,
                receivedObject.userEmail, receivedObject.photoUrl);
            try
            {
                // TODO : Analysis of all chats

                DeliverPayloadToClient(Operation.GetAnalytics, null, null, null, user);
            }
            catch (Exception)
            {
                DeliverPayloadToClient(Operation.GetAnalytics, null, null, null, user);
            }
        }

        /*
        public string GetStoredSummary()
        {
            return _sessionSummary;
        }

        /// <summary>
        /// This function is for UX  to get session id info and session Mode
        /// </summary>
        /// <returns></returns>
        public SessionData GetSessionData()
        {
            return _sessionData;
        }


        //     This method is called when a request for getting summary reaches the server side.
        //     A summary is created along with a user object (with the ID and the name of the user who requested the summary)
        //     This data is then sent back to the client side.
        private void GetSummaryProcedure(ClientToServerData receivedObject)
        {
            var summaryData = CreateSummary();
            UserData user = new(receivedObject.username, receivedObject.userID, receivedObject.userEmail, receivedObject.photoUrl);
            Trace.WriteLine("Sending summary to client");
            SendDataToClient("getSummary", null, summaryData, null, user);
        }


        //     Checks if an IPAddress is valid or not.
        private static bool IsValidIPAddress(string IPAddress)
        {
            // Check for null string, whitespaces or absence of colon
            if (string.IsNullOrWhiteSpace(IPAddress) || IPAddress.Contains(':') == false) return false;

            // Take the part after the colon as the port number and check the range
            var port = IPAddress[(IPAddress.LastIndexOf(':') + 1)..];
            if (int.TryParse(port, out var portNumber))
                if (portNumber < 0 || portNumber > 65535)
                    return false;

            // Take the part before colon as the ip address
            IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(':'));
            var byteValues = IPAddress.Split('.');

            // IPV4 contains 4 bytes separated by .
            if (byteValues.Length != 4) return false;

            // We have 4 bytes in a address
            //byte tempForParsing;

            // for each part(elements of byteValues list), we check whether the string 
            // can be successfully converted into a byte or not.
            return byteValues.All(r => byte.TryParse(r, out var tempForParsing));
        }

        //     All subscribers are notified about the new session by calling the
        //     OnAnalyticsChanged function for Notifying Telemetry module about the session data changes.
        public void NotifyTelemetryModule()
        {
            for (var i = 0; i < _telemetrySubscribers.Count; ++i)
                lock (this)
                {
                    _telemetrySubscribers[i].OnAnalyticsChanged(_sessionData);
                }
        }

        //     Removes the user received (from the ClientToServerData) object from the sessionData and
        //     Notifies telemetry about it. The new session is then broadcasted to all the users.
        private void RemoveClientProcedure(ClientToServerData receivedObject, int userID = -1)
        {
            Trace.WriteLine("[Dashboard Server] In RemoveClientProcedure() removing user from sessionData");
            int userIDToRemove;
            if (userID == -1)
            {
                userIDToRemove = receivedObject.userID;
            }
            else
            {
                Trace.WriteLine("[Dashboard] Network called the RemoveClientProcedure() to remove user from sessionData");
                userIDToRemove = userID;
            }

            var removedUser = _sessionData.RemoveUserFromSession(userIDToRemove);
            _communicator.RemoveClient(userIDToRemove.ToString());

            if (_sessionData.users.Count == 0)
            {
                EndSession(receivedObject);
                return;
            }

            if (removedUser != null)
            {
                NotifyTelemetryModule();
                SendDataToClient("removeClient", _sessionData, null, null, removedUser);
            }
        }

        /// <summary>
        /// Function to send data from Server to client side of the session manager.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="sessionData"></param>
        /// <param name="summaryData"></param>
        /// <param name="sessionAnalytics"></param>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        private void SendDataToClient(string eventName, SessionData sessionData, SummaryData summaryData,
           SessionAnalytics sessionAnalytics, UserData user, int userId = -1)
        {
            ServerToClientData serverToClientData;
            lock (this)
            {
                serverToClientData = new ServerToClientData(eventName, sessionData, summaryData, sessionAnalytics, user);
                string serializedSessionData = _serializer.Serialize(serverToClientData);

                if (userId == -1)
                {
                    Trace.WriteLine("[Dashboard]Sending To Network to braoadcast");
                    _communicator.Send(serializedSessionData, moduleIdentifier, null);
                }
                else
                {
                    Trace.WriteLine("[Dashboard] Sending To Network to notify to client ID:" + userId);
                    _communicator.Send(serializedSessionData, moduleIdentifier, userId.ToString());

                }
            }
        }
        */
    }
}
