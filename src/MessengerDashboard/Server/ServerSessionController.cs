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

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionController"/> with the provided <see cref="ICommunicator"/> instance.
        /// </summary>
        /// <param name="communicator">An <see cref="ICommunicator "/> implementation for server communication.</param>
        public ServerSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.AddSubscriber(_serverModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }

        public ServerSessionController()
        {
            _communicator = Factory.GetInstance();
            _communicator.AddSubscriber(_serverModuleIdentifier, this);
            ConnectionDetails = new(_communicator.IpAddress, _communicator.ListenPort);
        }

        public event EventHandler<SessionUpdatedEventArgs> SessionUpdated;

        //     Returns the credentials required to Join the meeting
        public ConnectionDetails ConnectionDetails { get; private set; } = null;

        private readonly SessionInfo _sessionInfo = new();

        public void BroadcastPayloadToClients(Operation operation, SessionInfo? sessionInfo, TextSummary? summary = null,
                                                      Analysis? sessionAnalytics = null, UserInfo? user = null, SentimentResult? sentiment = null)
        {
            ServerPayload serverPayload;
            lock (this)
            {
                serverPayload = new ServerPayload(operation, sessionInfo, user, summary, sessionAnalytics, sentiment);
                string serializedData = _serializer.Serialize(serverPayload);
                _communicator.Broadcast(_clientModuleIdentifier, serializedData);
            }
            Trace.WriteLine("Dashboard: Data sent to specific client");
        }

        public SentimentResult CalculateSentiment()
        {
            Trace.WriteLine("Dashboard: Getting chats for sentiment");
            List<ChatThread> chatThreads = _contentServer.GetAllMessages();
            List<string> chats = new();
            foreach(ChatThread chatThread in chatThreads)
            {
                foreach(ReceiveChatData receiveChatData in chatThread.MessageList)
                {
                    if (receiveChatData.Type == MessengerContent.MessageType.Chat)
                    {
                        chats.Add(receiveChatData.Data);
                    }
                }
            }
            SentimentResult sentiment = _sentimentAnalyzer.AnalyzeSentiment(chats.ToArray());
            return sentiment;
        }

        public TextSummary CalculateSummary()
        {
            Trace.WriteLine("Dashboard: Getting chats");
            List<ChatThread> chatThreads = _contentServer.GetAllMessages();
            List<string> chats = new();
            foreach(ChatThread chatThread in chatThreads)
            {
                foreach(ReceiveChatData receiveChatData in chatThread.MessageList)
                {
                    if (receiveChatData.Type == MessengerContent.MessageType.Chat)
                    {
                        chats.Add(receiveChatData.Data);
                    }
                }
            }
            TextSummarizationOptions options = new();
            TextSummary chatSummary = _textSummarizer.Summarize(chats.ToArray(), options);
            Trace.WriteLine("Dashboard: Created Summary");
            return chatSummary;
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
            Trace.WriteLine("Dashboard: Data sent to specific client");
        }

        public void EndSession()
        {
        }

        public void OnClientJoined(string ip, int port)
        {
        }

        public void OnClientLeft(string ip, int port)
        {
        }

        public void OnDataReceived(string serializedData)
        {
            Trace.WriteLine("Dashboard Server: Data received from communicator");
            try
            {
                if (serializedData == null)
                {
                    Trace.WriteLine("Dashboard Server: Null data received from communicator");
                    return;
                }
                ClientPayload clientPayload = _serializer.Deserialize<ClientPayload>(serializedData);
                if (clientPayload == null || clientPayload.UserInfo.UserName == null)
                {
                    Trace.WriteLine("Dashboard Server: Null user received from communicator");
                    return;
                }
                Operation operationType = clientPayload.Operation;
                switch (operationType)
                {
                    case Operation.AddClient:
                        AddClient(clientPayload);
                        return;

                    case Operation.GetSummary:
                        SendSummaryToClients();
                        return;

                    case Operation.GetTelemetryAnalysis:
                        SendTelemetryAnalysisToClients();
                        return;

                    case Operation.GetSentiment:
                        SendSentimentToClients();
                        return;

                    case Operation.RemoveClient:
                        RemoveClient(clientPayload);
                        return;

                    case Operation.LabMode:
                    case Operation.ExamMode:
                        ChangeSessionMode(clientPayload);
                        return;

                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Server: Exception" + e);
            }
        }

        private void ChangeSessionMode(ClientPayload clientPayload)
        {
            if (clientPayload.UserInfo.UserId == 1) // The leader or instructor
            {
                Trace.WriteLine("Dashboard Server: Changing session mode");
                _sessionInfo.SessionMode = (clientPayload.Operation == Operation.ExamMode) ? SessionMode.Exam : SessionMode.Lab;
                BroadcastPayloadToClients(clientPayload.Operation, _sessionInfo);
                Trace.WriteLine("Dashboard Server: Changed session mode");
            }
        }

        public void SetExamMode()
        {
            _sessionInfo.SessionMode = SessionMode.Exam;
            SessionUpdated?.Invoke(this, new(_sessionInfo));
            BroadcastPayloadToClients(Operation.ExamMode, _sessionInfo);
        }

        public void SetLabMode()
        {
            _sessionInfo.SessionMode = SessionMode.Lab;
            SessionUpdated?.Invoke(this, new(_sessionInfo));
            BroadcastPayloadToClients(Operation.LabMode, _sessionInfo);
        }

        private void AddClient(ClientPayload clientPayload)
        {
            lock (this)
            {
                Trace.WriteLine("Dashboard Server: Adding new user");
                _clientCount += 1;
                int id = _clientCount;
                UserInfo user = new() { UserEmail = clientPayload.UserInfo.UserEmail, UserId = id, UserName = clientPayload.UserInfo.UserName,
                                        UserPhotoUrl = clientPayload.UserInfo.UserPhotoUrl};
                _sessionInfo.Users.Add(user);
                SessionUpdated?.Invoke(this, new(_sessionInfo));
                _communicator.AddClient(clientPayload.IpAddress, clientPayload.Port);
                SendPayloadToClient(Operation.AddClientACK, clientPayload.IpAddress, clientPayload.Port, _sessionInfo, null, null, user);
                Trace.WriteLine("Dashboard Server: Added new user");
            }
        }
        
        private void SendSentimentToClients()
        {
            Trace.WriteLine("Dashboard: Sending sentiment to clients");
            SentimentResult sentiment = CalculateSentiment();
            BroadcastPayloadToClients(Operation.GetSentiment, _sessionInfo, null, null, null, sentiment);
            Trace.WriteLine("Dashboard: Sending sentiment to clients");
        }

        private void SendTelemetryAnalysisToClients()
        {
            Trace.WriteLine("Dashboard: Sending telemetry to clients");
            Analysis analysis = CalculateAnalysis();
            BroadcastPayloadToClients(Operation.GetSentiment, _sessionInfo, null, analysis);
            Trace.WriteLine("Dashboard: Sent telemetry to clients");
        }

        private void SendSummaryToClients()
        {
            Trace.WriteLine("Dashboard: Sending summary to clients");
            TextSummary summaryData = CalculateSummary();
            BroadcastPayloadToClients(Operation.GetSentiment, _sessionInfo, summaryData);
            Trace.WriteLine("Dashboard: Sent summary to clients");
        }

        private Analysis CalculateAnalysis()
        {
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
            Analysis analysis = _telemetry.UpdateAnalysis(userIdToUserInfoAndChatMap);
            return analysis;
        }

        private void RemoveClient(ClientPayload clientPayload)
        {
            TextSummary summary = CalculateSummary();
            SentimentResult sentiment = CalculateSentiment();
            Analysis analysis = CalculateAnalysis();
            if (clientPayload.UserInfo.UserId == 1) // The leader or instructor
            {
                Trace.WriteLine("Ending the session");
                BroadcastPayloadToClients(Operation.EndSession, _sessionInfo, summary, analysis, null, sentiment);
                _communicator.RemoveSubscriber(_serverModuleIdentifier);
                Trace.WriteLine("Ended the session");
            }
            else // The member or student
            {
                Trace.WriteLine("Dashboard Server: Removing Client");
                int removedCount = _sessionInfo.Users.RemoveAll(user => user.UserId == clientPayload.UserInfo.UserId);
                if (removedCount != 0)
                {
                    SessionUpdated?.Invoke(this, new(_sessionInfo));
                }
                SendPayloadToClient(Operation.RemoveClient, clientPayload.IpAddress, clientPayload.Port, _sessionInfo, summary, analysis, null, sentiment);
                _communicator.RemoveClient(clientPayload.IpAddress, clientPayload.Port);
                Trace.WriteLine("Dashboard: Removed Client");
            }
        }
    }
}
