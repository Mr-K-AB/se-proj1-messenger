/******************************************************************************
* Filename    = ServerSessionController.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains the definition of the ServerSessionController class,
*               which controls the server session for the Messenger Dashboard.
*****************************************************************************/

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
using TraceLogger;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Controls the server session.
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
        /// Initializes a new instance of the <see cref="ServerSessionController"/> used for testing.
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="contentServer"></param>
        public ServerSessionController(ICommunicator communicator, IContentServer contentServer)
        {
            _communicator = communicator;
            _contentServer = contentServer;
            SetupServer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSessionController"/> used for testing.
        /// </summary>
        public ServerSessionController() 
        {
            _communicator = CommunicationFactory.GetCommunicator(false);
            SetupServer();
        }

        /// <summary>
        /// Initial setup for server.j
        /// </summary>
        private void SetupServer()
        {
            _telemetry.SubscribeToServerSessionController(this);
            _communicator.Subscribe(_moduleName, this);
            string ipAndPort = _communicator.Start();
            string[] separator = { ":" };
            string[] ipAndPortArray = ipAndPort.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
            ConnectionDetails = new(ipAndPortArray[0], int.Parse(ipAndPortArray[1]));
        }

        /// <summary>
        /// Occurs when the session is updated.
        /// </summary>
        public event EventHandler<SessionUpdatedEventArgs>? SessionUpdated;

        /// <summary>
        /// Gets the credentials required for joining the meeting
        /// </summary>
        public ConnectionDetails ConnectionDetails { get; private set; }


        /// <summary>
        /// Gets the session information
        /// </summary>
        public SessionInfo SessionInfo { get; private set; } = new();

        /// <summary>
        /// Broadcasts payload data to all connected clients.
        /// </summary>
        /// <param name="operation">The type of operation to perform.</param>
        /// <param name="sessionInfo">Information about the session.</param>
        /// <param name="summary">Text summary data.</param>
        /// <param name="sessionAnalytics">Telemetry analysis data.</param>
        /// <param name="sentiment">Sentiment analysis result.</param>
        /// <param name="user">User information.</param>
        public void BroadcastPayloadToClients(Operation operation, SessionInfo? sessionInfo, TextSummary? summary = null,

                                                      Analysis? sessionAnalytics = null, SentimentResult? sentiment = null, UserInfo? user = null)
        {
            CreateLog("Dashboard Server >>> Broadcasting data");
            ServerPayload serverPayload;
            lock (this)
            {
                serverPayload = new ServerPayload(operation, sessionInfo, user, summary, sessionAnalytics, sentiment);
                string serializedData = _serializer.Serialize(serverPayload);
                _communicator.Send(serializedData, _moduleName, null);
            }
            CreateLog("Dashboard Server >>> Broadcasted data");
        }


        private void CalculateSentiment()
        {
            try
            {
                CreateLog("Dashboard Server >>> Getting chats for sentiment");
                List<ChatThread> chatThreads = _contentServer.GetAllMessages();
                CreateLog("Dashboard Server >>> Got chats for summary");
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
                CreateLog("Dashboard Server >>> Received " + chats.Count + " chat(s).");
                _sentiment = _sentimentAnalyzer.AnalyzeSentiment(chats.ToArray());
                CreateLog("Dashboard Server >>> Calculated sentiment");
            }
            catch (Exception e)
            {
                CreateLog("Dashboard Server >>> " + e.Message, LogLevel.ERROR);
            }
        }

        private void CalculateSummary()
        {
            try
            {
                CreateLog("Dashboard Server >>> Getting chats for summary");
                List<ChatThread> chatThreads = _contentServer.GetAllMessages();
                CreateLog("Dashboard Server >>> Got chats for summary");
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
                CreateLog("Dashboard Server >>> Received " + chats.Count + "chats.");
                TextSummarizationOptions options = new();
                _textSummary = _textSummarizer.Summarize(chats.ToArray(), options);
                CreateLog("Dashboard Server >>> Created Summary");
            }
            catch (Exception e)
            {
                CreateLog("Dashboard Server >>> " + e.Message, LogLevel.ERROR);
            }
        }

       private void SendPayloadToClient(Operation operation, int userId, SessionInfo? sessionInfo, UserInfo? userInfo = null,
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
            CreateLog("Dashboard Server >>> Data sent to client");
        }

        /// <summary>
        /// Handles Joining of new clients
        /// </summary>
        /// <param name="socketObject"></param>
        public void OnClientJoined(TcpClient socketObject)
        {
            CreateLog("Dashboard Server >>> Adding Client to session");
            _clientCount += 1;

            // The unique id for a client.
            int userId = _clientCount;
            _communicator.AddClient(userId.ToString(), socketObject);
            UserInfo userInfo = new("", userId, "", "");

            // Ask client for their details
            SendPayloadToClient(Operation.GiveUserDetails, userId, null, userInfo);
        }

        /// <summary>
        /// Handles leaving of client.
        /// </summary>
        /// <param name="clientId"></param>
        public void OnClientLeft(string clientId)
        {
            int userId = int.Parse(clientId);
            bool contains = false;
            foreach (UserInfo userInfo in SessionInfo.Users)
            {
                if (userInfo.UserId == userId)
                {
                    contains = true;
                }
            }
            if (contains)
            {
                RemoveClient(userId);
                _communicator.RemoveClient(clientId.ToString());
            }
        }

        /// <summary>
        /// Method that checks and deserializes data. 
        /// </summary>
        /// <param name="serializedData"></param>
        /// <returns>null if data is invalid otherwise the <see cref="ClientPayload"/></returns>
        private ClientPayload? DeserializeData(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                CreateLog("Dashboard Server >>> Null data received from communicator", LogLevel.WARNING);
                return null;
            }
            ClientPayload? clientPayload = _serializer.Deserialize<ClientPayload>(serializedData);
            if (clientPayload == null || 
                clientPayload.UserInfo == null ||
                string.IsNullOrEmpty(clientPayload.UserInfo.UserName))
            {
                CreateLog("Dashboard Server >>> Null user received from communicator", LogLevel.WARNING);
                clientPayload = null;
            }
            return clientPayload;
        }

        /// <summary>
        /// This method is called by the communicator for sending data.
        /// </summary>
        /// <param name="serializedData"></param>
        public void OnDataReceived(string serializedData)
        {
            try
            {
                CreateLog("Dashboard Server >>> Data received from communicator");
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
                    case Operation.CloseConnection:
                        _communicator.RemoveClient(clientPayload.UserInfo.UserId.ToString());
                        return;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                CreateLog("Dashboard Server >>> Exception: " + e.Message, LogLevel.ERROR);
            }
        }

        /// <summary>
        /// Recalculates all the analyzations and sends to clients.
        /// </summary>
        private void Refresh()
        {
            CreateLog("Dashboard Server >>> Started refresh");
            CalculateSummary();
            CalculateTelemetryAnalysis();
            CalculateSentiment();
            BroadcastPayloadToClients(Operation.Refresh, SessionInfo, _textSummary, _telemetryAnalysis, _sentiment);
            CreateLog("Dashboard Server >>> Done refresh");
        }

        /// <summary>
        /// Adds a new client and refreshes.
        /// </summary>
        /// <param name="userInfo"></param>
        private void IncludeClientInSession(UserInfo userInfo)
        {
            CreateLog("Dashboard Server >>> Adding Client to session");
            SessionInfo.Users.Add(userInfo);
            _userIdToUserInfoMap[userInfo.UserId] = userInfo;
            SessionUpdated?.Invoke(this, new (SessionInfo));
            Refresh();
            CreateLog("Dashboard Server >>> Added Client to session");
        }

        private void ChangeSessionMode(ClientPayload clientPayload)
        {
            if (!_userIdToUserInfoMap.ContainsKey(clientPayload.UserInfo.UserId))
            {
                return;
            }
            if (clientPayload.UserInfo.UserId == 1) // The leader or instructor
            {
                CreateLog("Dashboard Server >>> Changing session mode");
                SessionInfo.SessionMode = (clientPayload.Operation == Operation.ExamMode) ? SessionMode.Exam : SessionMode.Lab;
                SessionUpdated?.Invoke(this, new (SessionInfo));
                BroadcastPayloadToClients(Operation.SessionUpdated, SessionInfo);
                CreateLog("Dashboard Server >>> Changed session mode");
            }
        }

        private void CalculateTelemetryAnalysis()
        {
            try
            {
                CreateLog("Dashboard Server >>> Calculating telemetry analysis.");
                List<ChatThread> chatThreads = _contentServer.GetAllMessages();
                Dictionary<int, Tuple<UserInfo, List<string>>> userIdToUserInfoAndChatMap = new();
                foreach (ChatThread chatThread in chatThreads)
                {
                    foreach (ReceiveChatData receiveChatData in chatThread.MessageList)
                    {
                        if (receiveChatData.Type == MessengerContent.MessageType.Chat)
                        {
                            if (!userIdToUserInfoAndChatMap.ContainsKey(receiveChatData.SenderID) && 
                                _userIdToUserInfoMap.ContainsKey(receiveChatData.SenderID))
                            {
                                userIdToUserInfoAndChatMap.Add(receiveChatData.SenderID, new(_userIdToUserInfoMap[receiveChatData.SenderID], new()));
                                userIdToUserInfoAndChatMap[receiveChatData.SenderID].Item2.Add(receiveChatData.Data);
                            }
                        }
                    }
                }
                _telemetryAnalysis = _telemetry.UpdateAnalysis(userIdToUserInfoAndChatMap);
                CreateLog("Dashboard Server >>> Calculated telemetry analysis.");
            }
            catch (Exception e)
            {
                CreateLog("Dashboard Server >>> " + e.Message, LogLevel.ERROR);
            }
        }

        /// <summary>
        /// Removes the client from session, if initiated by instructor then the session ends.
        /// </summary>
        /// <param name="userId"></param>
        private void RemoveClient(int userId)
        {

            if (!_userIdToUserInfoMap.ContainsKey(userId))
            {
                return;
            }
            if (userId == 1) // The leader or instructor has id 1
            {
                CreateLog("Dashboard Server >>> Ending the session");
                SessionInfo.Users.Clear();
                SessionUpdated?.Invoke(this, new(SessionInfo));
                CalculateSummary();
                CalculateSentiment();
                CalculateTelemetryAnalysis();
                BroadcastPayloadToClients(Operation.EndSession, SessionInfo, _textSummary, _telemetryAnalysis, _sentiment);
                CreateLog("Dashboard Server >>> Ended the session");
            }
            else // The member or student 
            {
                CreateLog("Dashboard Server >>> Removing Client");
                int removedCount = SessionInfo.Users.RemoveAll(user => user.UserId == userId);
                if (removedCount != 0)
                {
                    SessionUpdated?.Invoke(this, new(SessionInfo));
                }
                CalculateSummary();
                CalculateSentiment();
                CalculateTelemetryAnalysis();
                SendPayloadToClient(Operation.EndSession, userId, SessionInfo, null, _textSummary, _telemetryAnalysis, _sentiment);
                BroadcastPayloadToClients(Operation.SessionUpdated, SessionInfo);
                CreateLog("Dashboard Server >>> Removed Client");
            }
        }

        private void CreateLog(string message, TraceLogger.LogLevel level = LogLevel.INFO)
        {
            Trace.WriteLine(message);
            Logger.Log(message, level);
        }
    }
}
