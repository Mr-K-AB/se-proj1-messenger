using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Summarization;
using MessengerNetworking.Communicator;

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

        public object Session { get; private set; }

        public MeetingMode MeetingMode { get; private set; }

        private readonly ICommunicator _communicator;
        private readonly ITextSummarizer _textSummarizer = TextSummarizerFactory.GetTextSummarizer();
        private readonly ISentimentAnalyzer _sentimentAnalyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();

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
            throw new NotImplementedException();
        }

        public void OnClientJoined(TcpClient socket)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string clientId)
        {
            throw new NotImplementedException();
        }

        public void DeliverPayloadToClient(ServerPayload serverPayload)
        {
            // serialize the payload
            
            // send through communicator
        }
    }
}
