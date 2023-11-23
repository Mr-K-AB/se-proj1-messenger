/******************************************************************************
* Filename    = ClientSessionController.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = A class that controls the session for the client.
*****************************************************************************/

using System;
using System.Diagnostics;
using MessengerNetworking.Communicator;
using MessengerDashboard.Telemetry;
using MessengerDashboard.Summarization;
using MessengerDashboard.Server;
using MessengerDashboard.Client.Events;
using MessengerNetworking.Factory;
using MessengerScreenshare.ScreenshareFactory;
using MessengerScreenshare.Client;
using MessengerContent.Client;
using MessengerDashboard.Sentiment;
using MessengerNetworking.NotificationHandler;

namespace MessengerDashboard.Client
{
    /// <summary>
    /// Manages the client session, handling communication with the server and updating session data.
    /// </summary>
    public class ClientSessionController : IClientSessionController, INotificationHandler
    {
        private readonly ICommunicator _communicator;

        private readonly IContentClient _contentClient = ContentClientFactory.GetInstance();

        private readonly string _moduleName = "Dashboard";

        private readonly IScreenshareClient _screenshareClient = ScreenshareFactory.getInstance();

        private readonly Serializer _serializer = new();

        private string _serverIp = string.Empty;

        private int _serverPort;

        private readonly UserInfo _userInfo = new();

        // Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionController"/> class.
        /// </summary>
        public ClientSessionController()
        {
            Trace.WriteLine("Dashboard Client >>> Creating Client Session Manager");
            _communicator = CommunicationFactory.GetCommunicator(true);
            _communicator.Subscribe(_moduleName, this);
            Trace.WriteLine("Dashboard Client >>> Created Client Session Manager");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionController"/> class.
        /// </summary>
        /// <param name="communicator">The communicator to use for communication with the server.</param>
        public ClientSessionController(ICommunicator communicator)
        {
            _communicator = communicator;
            _communicator.Subscribe(_moduleName, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionController"/> class.
        /// </summary>
        /// <param name="communicator">The communicator to use for communication with the server.</param>
        /// <param name="contentClient">The content client to use for managing content.</param>

        public ClientSessionController(ICommunicator communicator, IContentClient contentClient)
        {
            _communicator = communicator;
            _communicator.Subscribe(_moduleName, this);
            _contentClient = contentClient;
        }

      
        /// <summary>
        /// Occurs when the client session has changed.
        /// </summary>
        public event EventHandler<ClientSessionChangedEventArgs>? SessionChanged;

        /// <summary>
        /// Occurs when the client session has exited.
        /// </summary>
        public event EventHandler<SessionExitedEventArgs>? SessionExited;

        /// <summary>
        /// Occurs when the session mode has changed.
        /// </summary>
        public event EventHandler<SessionModeChangedEventArgs>? SessionModeChanged;

        /// <summary>
        /// Occurs when a refresh is requested.
        /// </summary>
        public event EventHandler<RefreshedEventArgs>? Refreshed;

        /// <summary>
        /// Gets the analysis results associated with the client session.
        /// </summary>
        public Analysis AnalysisResults { get; private set; } = new();

        /// <summary>
        /// Gets the chat summary associated with the client session.
        /// </summary>
        public TextSummary ChatSummary { get; private set; } = new();

        /// <summary>
        /// Gets a value indicating whether the client is connected to the server.
        /// </summary>
        public bool IsConnectedToServer { get; private set; } = false;

        /// <summary>
        /// Gets the session information associated with the client session.
        /// </summary>
        public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

        /// <summary>
        /// Gets the sentiment result associated with the client session.
        /// </summary>
        public SentimentResult SentimentResult { get; private set; } = new SentimentResult();

        /// <summary>
        /// Attempts to connect to the server with the specified parameters.
        /// </summary>
        /// <param name="serverIp">The IP address of the server.</param>
        /// <param name="serverPort">The port number of the server.</param>
        /// <param name="userName">The user's name.</param>
        /// <param name="userEmail">The user's email.</param>
        /// <param name="userPhotoUrl">The URL of the user's photo.</param>
        /// <returns>True if the connection is successful, otherwise false.</returns>
        public bool ConnectToServer(string serverIp, int serverPort, string userName, string userEmail, string userPhotoUrl)
        {
            // Check if already connected to the server
            if (IsConnectedToServer)
            {
                return true;
            }
            // Set server IP and port
            _serverIp = serverIp;
            _serverPort = serverPort;
            // Log connection attempt
            Trace.WriteLine("Dashboard Client >>> Connecting to server at IP: " +
                             serverIp + " Port: " + serverPort);

            // Check if the username is not empty or whitespace
            if (string.IsNullOrWhiteSpace(userName))
            {
                Trace.WriteLine("Dashboard Client >>> Null username received");
                return false;
            }
            // Lock to ensure thread safety during connection attempt
            lock (this)
            {
                // Log the connection attempt
                Trace.WriteLine("Dashboard Client >>> Connecting to server");
                // Initialize user information
                _userInfo.UserId = -1;
                _userInfo.UserName = userName;
                _userInfo.UserEmail = userEmail;
                _userInfo.UserPhotoUrl = userPhotoUrl;

                // Attempt to start communication with the server
                string connected = _communicator.Start(serverIp, serverPort.ToString());
               
                // Check if the connection was successful
                if (connected == "failure")
                {
                    Trace.WriteLine("Dashboard Client >>> Connection failed");
                }
                else
                {
                    IsConnectedToServer = true;
                    Trace.WriteLine("Dashboard Client >>> Connection succeeded");
                }
            }

            // Return the connection status
            return IsConnectedToServer;
        }

        /// <summary>
        /// Sends a refresh request to the server to retrieve any updates.
        /// </summary>
        public void SendRefreshRequestToServer()
        {
            // Log the refresh request
            Trace.WriteLine("Dashboard Client >>> Requesting server for any updates");

            // Send a refresh payload to the server
            SendPayloadToServer(Operation.Refresh);

            // Log that the request has been made
            Trace.WriteLine("Dashboard Client >>> Requested server for any updates");
        }

        /// <summary>
        /// Handles data received from the network.
        /// </summary>
        /// <param name="serializedData">The serialized data received from the network.</param>
        public void OnDataReceived(string serializedData)
        {
            // Check if the received serialized data is null
            if (serializedData == null)
            {
                Trace.WriteLine("Dashboard Server >>> Received null serialized data from network");
                return;
            }

            // Log that data has been received from the network
            Trace.WriteLine("Dashboard Server >>> Data received from network");
            try
            {
                // Deserialize the received data into a ServerPayload
                ServerPayload serverPayload = _serializer.Deserialize<ServerPayload>(serializedData);
               
                // Handle the operation based on the payload
                HandleOperation(serverPayload);
            }
            catch (Exception e)
            {
                // Log any exceptions that occur during data handling
                Trace.WriteLine("Dashboard Client >>> Exception " + e);
            }
        }

        /// <summary>
        /// Handles the operation based on the type of operation received from the server.
        /// </summary>
        /// <param name="serverPayload">The payload received from the server.</param>
        private void HandleOperation(ServerPayload serverPayload)
        {
            Operation operationType = serverPayload.Operation;
            switch (operationType)
            {
                case Operation.GiveUserDetails:
                    HandleGiveUserDetailsOperation(serverPayload);
                    break;
                case Operation.SessionUpdated:
                    HandleSessionUpdated(serverPayload);
                    break;
                case Operation.Refresh:
                    Refresh(serverPayload);
                    break;
                case Operation.RemoveClient:
                case Operation.EndSession:
                    Refresh(serverPayload);
                    ExitSession();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the SessionUpdated operation received from the server.
        /// </summary>
        /// <param name="serverPayload">The payload containing session information.</param>
        private void HandleSessionUpdated(ServerPayload serverPayload)
        {
            // Check if the received session info is null
            if (serverPayload.SessionInfo == null) 
            { 
                Trace.WriteLine("Dashboard Client >>> Null session info received");
                return;
            }

            // Update session data based on received information
            UpdateSessionData(serverPayload.SessionInfo);
        }

        /// <summary>
        /// Handles the GiveUserDetails operation received from the server.
        /// </summary>
        /// <param name="serverPayload">The payload containing user information.</param>
        private void HandleGiveUserDetailsOperation(ServerPayload serverPayload)
        {
            if (serverPayload.UserInfo == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null user info in GiveUser operation.");
                return;
            }
            int userId = serverPayload.UserInfo.UserId;
            Trace.WriteLine("Dashboard Client >>> Setting user info.");
            _userInfo.UserId = userId;
            _screenshareClient.SetUser(_userInfo.UserId, _userInfo.UserName);
            _contentClient.SetUser(_userInfo.UserId, _userInfo.UserName, _serverIp, _serverPort);
            Trace.WriteLine("Dashboard Client >>> User info set.");
            SendPayloadToServer(Operation.TakeUserDetails);
        }

        /// <summary>
        /// Handles the Refresh operation received from the server.
        /// </summary>
        /// <param name="serverPayload">The payload containing updated session information.</param>
        private void Refresh(ServerPayload serverPayload)
        {

            Trace.WriteLine("Dashboard Client >>> Refreshing");
            UpdateSummary(serverPayload.Summary);
            UpdateTelemetryAnalysis(serverPayload.SessionAnalysis);
            UpdateSentiment(serverPayload.Sentiment);
            UpdateSessionData(serverPayload.SessionInfo);
            Refreshed?.Invoke(this, new(AnalysisResults, SentimentResult, ChatSummary, SessionInfo));
            Trace.WriteLine("Dashboard Client >>> Refreshed");
        }

        /// <summary>
        /// Sends a request to the server to let the client exit the session.
        /// </summary>
        public void SendExitSessionRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server to let client exit.");
            SendPayloadToServer(Operation.RemoveClient);
            Trace.WriteLine("Dashboard Client >>> Requested server to let client exit.");
        }

        /// <summary>
        /// Sends a request to the server to enable lab mode.
        /// </summary>
        public void SendLabModeRequestToServer()
        {

            Trace.WriteLine("Dashboard Client >>> Requesting server for lab mode.");
            SendPayloadToServer(Operation.LabMode);
            Trace.WriteLine("Dashboard Client >>> Requested server for lab mode.");
        }

        /// <summary>
        /// Sends a request to the server to enable exam mode.
        /// </summary>
        public void SendExamModeRequestToServer()
        {
            Trace.WriteLine("Dashboard Client >>> Requesting server for lab mode.");
            SendPayloadToServer(Operation.ExamMode);
            Trace.WriteLine("Dashboard Client >>> Requested server for lab mode.");
        }

        /// <summary>
        /// Sends a payload to the server with the specified operation and user information.
        /// </summary>
        /// <param name="operation">The operation to be sent to the server.</param>

        private void SendPayloadToServer(Operation operation)
        {
            Trace.WriteLine("Dashboard Client >>> Sending data to server.");
            lock (this)
            {
                ClientPayload clientPayload = new(operation, _userInfo);
                string serializedData = _serializer.Serialize(clientPayload);
                _communicator.Send(serializedData, _moduleName, null);
            }
            Trace.WriteLine("Dashboard Client >>> Data sent to server.");
        }

        /// <summary>
        /// Exits the current session by stopping communication with the server.
        /// </summary>
        private void ExitSession()
        {
            Trace.WriteLine("Dashboard Client >>> Exiting session");
            _communicator.Stop();
            IsConnectedToServer = false;
            SessionExited?.Invoke(this, new(ChatSummary, SentimentResult, AnalysisResults));
            Trace.WriteLine("Dashboard Client >>> Exited session");
        }

        /// <summary>
        /// Updates the telemetry analysis data with the specified analysis.
        /// </summary>
        /// <param name="analysis">The telemetry analysis data to be updated.</param>
        private void UpdateTelemetryAnalysis(Analysis? analysis)
        {
            Trace.WriteLine("Dashboard Client >>> Updating telemetry analysis");
            if (analysis == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null telemetry");
                return;
            }
            lock (this)
            {
                AnalysisResults = analysis;
            }
            Trace.WriteLine("Dashboard Client >>> Updated telemetry analysis");
        }

        /// <summary>
        /// Updates the sentiment analysis data with the specified sentiment result.
        /// </summary>
        /// <param name="sentiment">The sentiment result data to be updated.</param>
        private void UpdateSentiment(SentimentResult? sentiment)
        {
            Trace.WriteLine("Dashboard Client >>> Updating Sentiment");
            if (sentiment == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null summary");
                return;
            }
            lock (this)
            {
                SentimentResult = sentiment;
            }
            Trace.WriteLine("Dashboard Client >>> Updated Sentiment");
        }

        /// <summary>
        /// Updates the summary data with the specified text summary.
        /// </summary>
        /// <param name="textSummary">The text summary data to be updated.</param>
        private void UpdateSummary(TextSummary? textSummary)
        {
            Trace.WriteLine("Dashboard Client >>> Updating Summary");
            if (textSummary == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null summary");
                return;
            }
            lock (this)
            {
                ChatSummary = textSummary;
            }
            Trace.WriteLine("Dashboard Client >>> Updated Summary");
        }

        /// <summary>
        /// Updates the session information data with the specified session information.
        /// </summary>
        /// <param name="sessionInfo">The session information data to be updated.</param>
        private void UpdateSessionData(SessionInfo? sessionInfo)
        {
            if (sessionInfo == null)
            {
                Trace.WriteLine("Dashboard Client >>> Received null session info.");
                return;
            }
            Trace.WriteLine("Dashboard Client >>> Updating Session information.");
            bool modeChanged = false;
            lock (this)
            {
                modeChanged = sessionInfo.SessionMode != SessionInfo.SessionMode;
                SessionInfo = sessionInfo;
            }
            SessionChanged?.Invoke(this, new ClientSessionChangedEventArgs(SessionInfo));
            if (modeChanged)
            {
                SessionModeChanged?.Invoke(this, new SessionModeChangedEventArgs(SessionInfo.SessionMode));
            }
            Trace.WriteLine("Dashboard Client >>> Updated Session information.");
        }
    }
}
