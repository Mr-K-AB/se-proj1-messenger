using System;
using System.Collections.Generic;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerNetworking.Communicator;
using MessengerDashboard.Telemetry;
using System.Diagnostics;
using MessengerDashboard.Server.Events;
using MessengerDashboard.Client;
using MessengerNetworking.Factory;
using MessengerContent.Server;
using MessengerContent.DataModels;

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
        private readonly ICommunicator _communicator;

        private readonly IContentServer _contentServer = ContentServerFactory.GetInstance();
        
        private readonly ISentimentAnalyzer _sentimentAnalyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();

        private readonly string _clientModuleIdentifier = "DashboardClient";

        private readonly string _serverModuleIdentifier = "DashboardServer";

        private readonly Serializer _serializer = new();

        private readonly ITelemetry _telemetry = TelemetryFactory.GetTelemetryInstance();

        private readonly ITextSummarizer _textSummarizer = TextSummarizerFactory.GetTextSummarizer();

        private int _clientCount = 0;

        private readonly Dictionary<int, UserInfo> _userIdToUserInfoMap = new();

        private readonly List<UserInfo> _notConfirmedClients = new();

        private readonly Dictionary<string, int> _clientSessionControllerIdsToUserIdMap = new();

        private TextSummary? _textSummary;

        private Analysis? _telemetryAnalysis;

        private SentimentResult? _sentiment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionController"/> with the provided <see cref="ICommunicator"/> instance.
        /// </summary>
        /// <param name="communicator">An <see cref="ICommunicator "/> implementation for server communication.</param>
        public ServerSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_serverModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
            _telemetry.SubscribeToServerSessionController(this);
        }

        public ServerSessionController()
        {
            _communicator = Factory.GetInstance();
            _communicator.AddSubscriber(_serverModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
            _telemetry.SubscribeToServerSessionController(this);
        }

        public event EventHandler<SessionUpdatedEventArgs> SessionUpdated;

        /// <summary>
        ///  the credentials required to Join the meeting
        /// </summary>
        public ConnectionDetails ConnectionDetails { get; private set; } = null;

        public SessionInfo SessionInfo = new();

        public void BroadcastPayloadToClients(Operation operation, SessionInfo? sessionInfo, TextSummary? summary = null,
                                                      Analysis? sessionAnalytics = null, SentimentResult? sentiment = null, UserInfo? user = null)
        {
            Trace.WriteLine("Dashboard Server >>> Broadcasting data");
            ServerPayload serverPayload;
            lock (this)
            {
                serverPayload = new ServerPayload(operation, sessionInfo, user, summary, sessionAnalytics, sentiment);
                string serializedData = _serializer.Serialize(serverPayload);
                _communicator.Broadcast(_clientModuleIdentifier, serializedData);
            }
            Trace.WriteLine("Dashboard Server >>> Broadcasted data");
        }

        public void CalculateSentiment()
        {
            try
            {
                Trace.WriteLine("Dashboard Server >>> Getting chats for sentiment");
                List<ChatThread> chatThreads = _contentServer.GetAllMessages();
                Trace.WriteLine("Dashboard Server >>> Got chats for summary");
                List<string> chats = new();
                foreach (ChatThread chatThread in chatThreads)
                {
                    foreach (ReceiveChatData receiveChatData in chatThread.MessageList)
                    {
                        if (receiveChatData.Type == MessengerContent.MessageType.Chat)
                        {
                            chats.Add(receiveChatData.Data);
                        }
                    }
                }
                Trace.WriteLine("Dashboard Server >>> Received " + chats.Count + " chat(s).");
                _sentiment = _sentimentAnalyzer.AnalyzeSentiment(chats.ToArray());
                Trace.WriteLine("Dashboard Server >>> Calculated sentiment");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Server >>> Exception in CalculateSentiment " + e.Message);
            }
        }

        public void CalculateSummary()
        {
            try
            {
                Trace.WriteLine("Dashboard Server >>> Getting chats for summary");
                List<ChatThread> chatThreads = _contentServer.GetAllMessages();
                Trace.WriteLine("Dashboard Server >>> Got chats for summary");
                List<string> chats = new();
                foreach (ChatThread chatThread in chatThreads)
                {
                    foreach (ReceiveChatData receiveChatData in chatThread.MessageList)
                    {
                        if (receiveChatData.Type == MessengerContent.MessageType.Chat)
                        {
                            chats.Add(receiveChatData.Data);
                        }
                    }
                }
                Trace.WriteLine("Dashboard Server >>> Received " + chats.Count + "chats.");
                TextSummarizationOptions options = new();
                _textSummary = _textSummarizer.Summarize(chats.ToArray(), options);
                Trace.WriteLine("Dashboard Server >>> Created Summary");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Server >>> Exception in Calculate Summary " + e.Message);
            }
        }

        public void SendPayloadToClient(Operation operation, string ip, int port, SessionInfo? sessionInfo,
                    TextSummary? summary = null, Analysis? sessionAnalytics = null,
                    UserInfo? user = null, SentimentResult? sentiment = null)
        {
            ServerPayload serverPayload;
            lock (this)
            {
                serverPayload = new ServerPayload(operation, sessionInfo, user, summary, sessionAnalytics, sentiment);
                string serializedData = _serializer.Serialize(serverPayload);
                _communicator.SendMessage(ip, port, _clientModuleIdentifier, serializedData);
            }
            Trace.WriteLine("Dashboard Server >>> Data sent to client");
        }

        public void OnClientJoined(string ip, int port)
        {
        }

        public void OnClientLeft(string ip, int port)
        {
        }

        private  ClientPayload? DeserializeData(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                Trace.WriteLine("Dashboard Server >>> Null data received from communicator");
                return null;
            }
            ClientPayload clientPayload = _serializer.Deserialize<ClientPayload>(serializedData);
            if (clientPayload == null || clientPayload.UserInfo == null || string.IsNullOrEmpty(clientPayload.UserInfo.UserName)
                || string.IsNullOrEmpty(clientPayload.IpAddress))
            {
                Trace.WriteLine("Dashboard Server >>> Null user received from communicator");
                clientPayload = null;
            }
            return clientPayload;
        }

        public void OnDataReceived(string serializedData)
        {
            try
            {
                Trace.WriteLine("Dashboard Server >>> Data received from communicator");
                ClientPayload? clientPayload = DeserializeData(serializedData);
                if (clientPayload == null)
                {
                    return;
                }
                Operation operationType = clientPayload.Operation;
                switch (operationType)
                {
                    case Operation.AddClient:
                        SendAddClientAcknowledgement(clientPayload);
                        return;

                    case Operation.AddClientConfirmation:
                        AddClientToSession(clientPayload);
                        return;

                    case Operation.RemoveClient:
                        RemoveClient(clientPayload);
                        return;

                    case Operation.LabMode:
                    case Operation.ExamMode:
                        ChangeSessionMode(clientPayload);
                        return;

                    case Operation.Refresh:
                        Refresh();
                        return;

                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Server >>> Exception: " + e);
            }
        }

        private void Refresh()
        {
            Trace.WriteLine("Dashboard Server >>> Started refresh");
            CalculateSummary();
            CalculateTelemetryAnalysis();
            CalculateSentiment();
            BroadcastPayloadToClients(Operation.RefreshAcknowledgement, SessionInfo, _textSummary, _telemetryAnalysis, _sentiment);
            Trace.WriteLine("Dashboard Server >>> Done refresh");
        }

        private void AddClientToSession(ClientPayload clientPayload)
        {

            Trace.WriteLine("Dashboard Server >>> Adding Client to session");
            string clientSessionControllerId = clientPayload.ClientSessionControllerId;
            if (!_clientSessionControllerIdsToUserIdMap.ContainsKey(clientSessionControllerId))
            {
                return;
            }
            int userId = _clientSessionControllerIdsToUserIdMap[clientSessionControllerId];
            if (userId != clientPayload.UserInfo.UserId)
            {
                return;
            }
            SessionInfo.Users.RemoveAll(user =>  user.UserId == userId);
            SessionInfo.Users.Add(clientPayload.UserInfo);
            _userIdToUserInfoMap[userId] = clientPayload.UserInfo;
            SessionUpdated?.Invoke(this, new (SessionInfo));
            _communicator.AddClient(clientPayload.IpAddress, clientPayload.Port);
            Refresh();
            Trace.WriteLine("Dashboard Server >>> Added Client to session");
        }

        private void ChangeSessionMode(ClientPayload clientPayload)
        {
            if (clientPayload.UserInfo.UserId == 1) // The leader or instructor
            {
                Trace.WriteLine("Dashboard Server >>> Changing session mode");
                SessionInfo.SessionMode = (clientPayload.Operation == Operation.ExamMode) ? SessionMode.Exam : SessionMode.Lab;
                SessionUpdated.Invoke(this, new (SessionInfo));
                BroadcastPayloadToClients(clientPayload.Operation, SessionInfo);
                Trace.WriteLine("Dashboard Server >>> Changed session mode");
            }
        }

        public void SetExamMode()
        {
            SessionInfo.SessionMode = SessionMode.Exam;
            SessionUpdated?.Invoke(this, new(SessionInfo));
            BroadcastPayloadToClients(Operation.ExamMode, SessionInfo);
        }

        public void SetLabMode()
        {
            SessionInfo.SessionMode = SessionMode.Lab;
            SessionUpdated?.Invoke(this, new(SessionInfo));
            BroadcastPayloadToClients(Operation.LabMode, SessionInfo);
        }

        private void SendAddClientAcknowledgement(ClientPayload clientPayload)
        {
            lock (this)
            {
                Trace.WriteLine("Dashboard Server >>> Sending add client acknowledgement to " + 
                                ((clientPayload.UserInfo == null) ? "" : clientPayload.UserInfo.UserName ?? ""));
                int userId;
                string clientSessionControllerId = clientPayload.ClientSessionControllerId;
                if (_clientSessionControllerIdsToUserIdMap.ContainsKey(clientSessionControllerId))
                {
                    userId = _clientSessionControllerIdsToUserIdMap[clientSessionControllerId];
                }
                else
                {
                    _clientCount += 1;
                    userId = _clientCount;
                    _clientSessionControllerIdsToUserIdMap[clientSessionControllerId] = userId;
                }
                UserInfo user = new() { UserEmail = clientPayload.UserInfo.UserEmail, UserId = userId, UserName = clientPayload.UserInfo.UserName,
                                        UserPhotoUrl = clientPayload.UserInfo.UserPhotoUrl};
                _notConfirmedClients.Add(user);
                SendPayloadToClient(Operation.AddClientAcknowledgement, clientPayload.IpAddress, clientPayload.Port, SessionInfo, null, null, user);
                Trace.WriteLine("Dashboard Server >>> Sent add client acknowledgement");
            }
        }
        
        private void CalculateTelemetryAnalysis()
        {
            try
            {
                Trace.WriteLine("Dashboard Server: Calculating telemetry analysis.");
                List<ChatThread> chatThreads = _contentServer.GetAllMessages();
                Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap = new();
                foreach(ChatThread chatThread in chatThreads)
                {
                    foreach(ReceiveChatData receiveChatData in chatThread.MessageList)
                    {
                        if (receiveChatData.Type == MessengerContent.MessageType.Chat)
                        {
                            if (!userIdToUserInfoAndChatMap.ContainsKey(receiveChatData.SenderID))
                            {
                                userIdToUserInfoAndChatMap[receiveChatData.SenderID] = new(_userIdToUserInfoMap[receiveChatData.SenderID], new());
                                userIdToUserInfoAndChatMap[receiveChatData.SenderID].Item2.Add(receiveChatData.Data);
                            }
                        }
                    }
                }
                _telemetryAnalysis = _telemetry.UpdateAnalysis(userIdToUserInfoAndChatMap);
                Trace.WriteLine("Dashboard Server: Calculated telemetry analysis.");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Server: Exception in CalculateTelemetryAnalysis " + e.ToString());
            }
       }

        private void RemoveClient(ClientPayload clientPayload)
        {
            CalculateSummary();
            CalculateSentiment();
            CalculateTelemetryAnalysis();
            if (clientPayload.UserInfo.UserId == 1) // The leader or instructor
            {
                SessionInfo.Users.Clear();
                SessionUpdated?.Invoke(this, new(SessionInfo));
                Trace.WriteLine("Dashboard Server >>> Ending the session");
                BroadcastPayloadToClients(Operation.EndSession, SessionInfo, _textSummary, _telemetryAnalysis, _sentiment);
                _communicator.RemoveSubscriber(_serverModuleIdentifier);
                Trace.WriteLine("Dashboard Server >>> Ended the session");
            }
            else // The member or student
            {
                Trace.WriteLine("Dashboard Server >>> Removing Client");
                int removedCount = SessionInfo.Users.RemoveAll(user => user.UserId == clientPayload.UserInfo.UserId);
                if (removedCount != 0)
                {
                    SessionUpdated?.Invoke(this, new(SessionInfo));
                }
                SendPayloadToClient(Operation.RemoveClient, clientPayload.IpAddress, clientPayload.Port, SessionInfo, _textSummary, _telemetryAnalysis, null, _sentiment);
                _communicator.RemoveClient(clientPayload.IpAddress, clientPayload.Port);
                BroadcastPayloadToClients(Operation.SessionUpdated, SessionInfo);
                Trace.WriteLine("Dashboard Server >>> Removed Client");
            }
        }
    }
}
