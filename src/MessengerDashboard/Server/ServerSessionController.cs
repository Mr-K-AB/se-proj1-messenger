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
using System.Net.Sockets;
using MessengerNetworking.NotificationHandler;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Handles the server session.
    /// </summary>
    /// <remarks>
    /// Implements <see cref="IServerSessionController"/>
    /// </remarks>
    public class ServerSessionController : IServerSessionController, INotificationHandler    
    {
        private readonly ICommunicator _communicator;

        private readonly IContentServer _contentServer = ContentServerFactory.GetInstance();
        
        private readonly ISentimentAnalyzer _sentimentAnalyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();

        private readonly string _moduleName = "Dashboard";

        private readonly Serializer _serializer = new();

        private readonly ITelemetry _telemetry = TelemetryFactory.GetTelemetryInstance();

        private readonly ITextSummarizer _textSummarizer = TextSummarizerFactory.GetTextSummarizer();

        private int _clientCount = 0;

        private readonly Dictionary<int, UserInfo> _userIdToUserInfoMap = new();

        private TextSummary _textSummary = new();

        private Analysis _telemetryAnalysis = new();

        private SentimentResult _sentiment = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionController"/> with the provided <see cref="ICommunicator"/> instance.
        /// </summary>
        /// <param name="communicator">An <see cref="ICommunicator "/> implementation for server communication.</param>
        public ServerSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            SetupServer();
        }

        public ServerSessionController() 
        {
            _communicator = CommunicationFactory.GetCommunicator(false);
            SetupServer();
        }

        private void SetupServer()
        {
            _telemetry.SubscribeToServerSessionController(this);
            _communicator.Subscribe(_moduleName, this);
            string ipAndPort = _communicator.Start();
            string[] separator = { ":" };
            string[] ipAndPortArray = ipAndPort.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
            ConnectionDetails = new(ipAndPortArray[0], int.Parse(ipAndPortArray[1]));
        }

        public event EventHandler<SessionUpdatedEventArgs>? SessionUpdated;

        /// <summary>
        ///  the credentials required to Join the meeting
        /// </summary>
        public ConnectionDetails ConnectionDetails { get; private set; }

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
                _communicator.Send(serializedData, _moduleName, null);
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

        public void SendPayloadToClient(Operation operation, int userId, SessionInfo? sessionInfo, UserInfo? userInfo = null,
                    TextSummary? summary = null, Analysis? sessionAnalytics = null,
                    SentimentResult? sentiment = null)
        {
            ServerPayload serverPayload;
            lock (this)
            {
                serverPayload = new ServerPayload(operation, sessionInfo, userInfo, summary, sessionAnalytics, sentiment);
                string serializedData = _serializer.Serialize(serverPayload);
                _communicator.Send(serializedData, _moduleName, userId.ToString());
            }
            Trace.WriteLine("Dashboard Server >>> Data sent to client");
        }

        public void OnClientJoined(TcpClient socketObject)
        {
            Trace.WriteLine("Dashboard Server >>> Adding Client to session");
            _clientCount += 1;
            int userId = _clientCount;
            _communicator.AddClient(userId.ToString(), socketObject);
            UserInfo userInfo = new("", userId, "", "");
            SendPayloadToClient(Operation.GiveUserDetails, userId, null, userInfo);
        }

        public void OnClientLeft(string clientId)
        {
            int userId = int.Parse(clientId);
            RemoveClient(userId);
        }

        private ClientPayload? DeserializeData(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                Trace.WriteLine("Dashboard Server >>> Null data received from communicator");
                return null;
            }
            ClientPayload? clientPayload = _serializer.Deserialize<ClientPayload>(serializedData);
            if (clientPayload == null || 
                clientPayload.UserInfo == null ||
                string.IsNullOrEmpty(clientPayload.UserInfo.UserName))
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
                    case Operation.TakeUserDetails:
                        IncludeClientInSession(clientPayload.UserInfo);
                        return;

                    case Operation.RemoveClient:
                        RemoveClient(clientPayload.UserInfo.UserId);
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
            BroadcastPayloadToClients(Operation.Refresh, SessionInfo, _textSummary, _telemetryAnalysis, _sentiment);
            Trace.WriteLine("Dashboard Server >>> Done refresh");
        }

        private void IncludeClientInSession(UserInfo userInfo)
        {
            Trace.WriteLine("Dashboard Server >>> Adding Client to session");
            SessionInfo.Users.Add(userInfo);
            _userIdToUserInfoMap[userInfo.UserId] = userInfo;
            SessionUpdated?.Invoke(this, new (SessionInfo));
            Refresh();
            Trace.WriteLine("Dashboard Server >>> Added Client to session");
        }

        private void ChangeSessionMode(ClientPayload clientPayload)
        {
            if (clientPayload.UserInfo.UserId == 1) // The leader or instructor
            {
                Trace.WriteLine("Dashboard Server >>> Changing session mode");
                SessionInfo.SessionMode = (clientPayload.Operation == Operation.ExamMode) ? SessionMode.Exam : SessionMode.Lab;
                SessionUpdated?.Invoke(this, new (SessionInfo));
                BroadcastPayloadToClients(Operation.SessionUpdated, SessionInfo);
                Trace.WriteLine("Dashboard Server >>> Changed session mode");
            }
        }

        private void CalculateTelemetryAnalysis()
        {
            try
            {
                Trace.WriteLine("Dashboard Server >>> Calculating telemetry analysis.");
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
                Trace.WriteLine("Dashboard Server >>> Calculated telemetry analysis.");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Dashboard Server: Exception in CalculateTelemetryAnalysis " + e.ToString());
            }
        }

        private void RemoveClient(int userId)
        {
            if (userId == 1) // The leader or instructor
            {
                Trace.WriteLine("Dashboard Server >>> Ending the session");
                SessionInfo.Users.Clear();
                SessionUpdated?.Invoke(this, new(SessionInfo));
                BroadcastPayloadToClients(Operation.EndSession, SessionInfo, _textSummary, _telemetryAnalysis, _sentiment);
                _communicator.RemoveClient(userId.ToString());
                _communicator.Stop();
                Trace.WriteLine("Dashboard Server >>> Ended the session");
            }
            else // The member or student
            {
                Trace.WriteLine("Dashboard Server >>> Removing Client");
                int removedCount = SessionInfo.Users.RemoveAll(user => user.UserId == userId);
                if (removedCount != 0)
                {
                    SessionUpdated?.Invoke(this, new(SessionInfo));
                }
                CalculateSummary();
                CalculateSentiment();
                CalculateTelemetryAnalysis();
                SendPayloadToClient(Operation.EndSession, userId, SessionInfo, null, _textSummary, _telemetryAnalysis, _sentiment);
                _communicator.RemoveClient(userId.ToString());
                BroadcastPayloadToClients(Operation.SessionUpdated, SessionInfo);
                Trace.WriteLine("Dashboard Server >>> Removed Client");
            }
        }
    }
}
