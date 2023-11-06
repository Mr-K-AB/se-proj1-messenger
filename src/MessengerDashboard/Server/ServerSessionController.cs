using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerNetworking.Communicator;
using System.Net;
using MessengerNetworking.NotificationHandler;
using MessengerDashboard.Telemetry;
using System.Diagnostics;
using System.Threading;
using MessengerDashboard.Server.Events;
using MessengerDashboard.Client;
using Microsoft.Extensions.Configuration.UserSecrets;
using MessengerNetworking.Factory;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Handles the server session.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IServerSessionController"/>
    /// </remarks>
    public class ServerSessionController : IServerSessionController    
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionController"/> with the provided <see cref="ICommunicator"/> instance.
        /// </summary>
        /// <param name="communicator">An <see cref="ICommunicator "/> implementation for server communication.</param>
        public ServerSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }
        
        public ServerSessionController()
        {
            _communicator = Factory.GetInstance();
            _communicator.AddSubscriber(_moduleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }

        public SessionMode SessionMode { get; private set; }

        private readonly ICommunicator _communicator;
        private readonly ITextSummarizer _textSummarizer = TextSummarizerFactory.GetTextSummarizer();
        private readonly ISentimentAnalyzer _sentimentAnalyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();
        private readonly Serializer _serializer = new();
        private int _clientCount = 0;
        public SessionInfo _sessionInfo;
        private readonly SessionMode _sessionMode;
        private TextSummary? _chatSummary;
        public event EventHandler<SessionUpdatedEventArgs> SessionUpdated;
        private readonly string _moduleIdentifier = "Dashboard";
        public SessionAnalytics _sessionAnalytics;

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

            if (clientPayload == null || clientPayload.UserName == null)
            {
                throw new ArgumentNullException("Null user received");
            }
            Operation operationType = clientPayload.Operation;
            switch (operationType)
            {
                case Operation.ToggleSessionMode:
                    break;
                case Operation.AddClient:
                    AddClient(clientPayload);
                    break;
                case Operation.GetSummary:
                    DeliverSummaryToClient(clientPayload);
                    break;
                case Operation.GetAnalytics:
                    DeliverAnalyticsToClient(clientPayload);
                    break;
                case Operation.RemoveClient:
                    break;
                case Operation.EndSession:
                    break;
                default:
                    break;
            }
        }

        public void DeliverPayloadToClient(Operation operation, string ip, int port, SessionInfo? sessionInfo,
            TextSummary? summary = null, SessionAnalytics? sessionAnalytics = null, 
            UserInfo? user = null, int userId = -1)
        {
            ServerPayload serverPayload;
            lock (this)
            {
                serverPayload = new ServerPayload(operation, sessionInfo, summary, sessionAnalytics, user);
                string serializedData = _serializer.Serialize(serverPayload);
                _communicator.SendMessage(ip, port, _moduleIdentifier, serializedData);
            }
            Trace.WriteLine("Dashboard: Data sent to specific client");
        }

        private void AddClient(ClientPayload clientPayload)
        {
            lock (this)
            {
                _clientCount += 1;
                int id = _clientCount;
                UserInfo user = new() { UserEmail = clientPayload.UserEmail, UserID = clientPayload.UserID, UserName = clientPayload.UserName,
                                        UserPhotoUrl = clientPayload.UserPhotoURL };
                AddUser(user);
                _communicator.AddClient(clientPayload.IpAddress, clientPayload.Port);
                DeliverPayloadToClient(Operation.AddClientACK, clientPayload.IpAddress, clientPayload.Port, _sessionInfo, null, null, user, id);
                NewUserAdded?.Invoke(this, EventArgs.Empty);
            }
       }

        public void OnClientJoined(string ip, int port)
        {
            /*
             
            lock (this)
            {
                _clientCount += 1;
                //_communicator.
                int id = _clientCount;
                UserInfo userInfo = new(null, id, null, null);
                //_communicator.
                //_communicator.AddClient();
                DeliverPayloadToClient(Operation.ID, null, null, null, userInfo, id);
            }
           */
        }

        public void OnClientLeft(string ip, int port)
        {
            // TODO: Remove Client
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


        */
        //     Returns the credentials required to Join the meeting
        public ConnectionDetails ConnectionDetails { get; private set; } = null;

        public event EventHandler SessionEnded;

        private void AddUser(UserInfo user)
        {
            lock (this)
            {
                _sessionInfo.AddUser(user);
            }
        }

        public void BroadcastPayloadToClients(Operation operation, SessionInfo? sessionInfo,
            TextSummary? summary = null, SessionAnalytics? sessionAnalytics = null, 
            UserInfo? user = null, int userId = -1)
        {

        }

        private void ToggleSessionMode()
        {
            Trace.WriteLine("Dashboard: Session Mode changed in Session Data");
            BroadcastPayloadToClients(Operation.ToggleSessionMode, _sessionInfo);
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

        private void EndSession(ClientPayload clientPayload)
        {
            Trace.WriteLine("Dashboard: Ending Session");
            try
            {
                Trace.WriteLine("Dashboard: Saving summary and analytics");

                // TODO : Summary saving

                Trace.WriteLine("Dashboard: Summary Saved");
            }
            catch (Exception)
            {
            }
            finally
            {
                DeliverPayloadToClient(Operation.EndSession, clientPayload.IpAddress, clientPayload.Port, _sessionInfo);
            }
            Thread.Sleep(2000);
            // TODO : STOP COMMUNICATOR
            SessionEnded?.Invoke(this, EventArgs.Empty);
        }

        private void GetAnalytics(ClientPayload receivedObject)
        {
            UserInfo user = new(receivedObject.UserName, receivedObject.UserID,
                receivedObject.UserEmail, receivedObject.UserPhotoURL);
            // TODO : Analysis of all chats
            DeliverPayloadToClient(Operation.GetAnalytics, receivedObject.IpAddress, receivedObject.Port, null, null, null, user);
        }

        private void DeliverSummaryToClient(ClientPayload clientPayload)
        {
            TextSummary summaryData = CreateSummary();
            UserInfo user = new(clientPayload.UserName, clientPayload.UserID, clientPayload.UserEmail, clientPayload.UserPhotoURL);
            Trace.WriteLine("Dashboard: Sending summary to client");
            DeliverPayloadToClient(Operation.GetSummary, clientPayload.IpAddress, clientPayload.Port, null, summaryData, null, user);
        }

        private void DeliverAnalyticsToClient(ClientPayload receivedObject)
        {
            UserInfo user = new(receivedObject.UserName, receivedObject.UserID, receivedObject.UserEmail, receivedObject.UserPhotoURL);
            try
            {
                //var allChats = _contentServer.GetAllMessages().ToArray();
                //_sessionAnalytics = _telemetry.GetTelemetryAnalytics(allChats);
                DeliverPayloadToClient(Operation.GetAnalytics,receivedObject.IpAddress, receivedObject.Port, null, null, _sessionAnalytics, user);
            }
            catch (Exception)
            {
                // In case of a failure, the user is returned a null object
                //SendDataToClient("getAnalytics", null, null, null, user);
            }
        }

        private void RemoveClient(ClientPayload receivedObject, int userID = -1)
        {
            Trace.WriteLine("Dashboard: Removing Client");
            //int userIDToRemove;
            if (userID == -1)
            {
                //userIDToRemove = receivedObject.UserID;
            }
            else
            {
                //userIDToRemove = userID;
            }

            // TODO : Remove user
            //var removedUser = _sessionInfo.RemoveUser(userIDToRemove);
            _communicator.RemoveClient(receivedObject.IpAddress, receivedObject.Port);

            /*
            if (removedUser != null)
            {
                SessionUpdated.Invoke(this, new SessionUpdatedEventArgs(_sessionInfo));
                DeliverPayloadToClient(Operation.RemoveClient, receivedObject.IpAddress, receivedObject.Port, _sessionInfo);
            }
            */
        }
    }
}
