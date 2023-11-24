/******************************************************************************
* Filename    = TelemetryAndSentimentTests.cs
*
* Author      = Aradhya Bijalwan
* 
* Roll Number = 112001006
*
* Product     = Messenger
* 
* Project     = MessengerTests
*
* Description = Telemetry And Sentiment Tests.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerContent.DataModels;
using MessengerDashboard.Client;
using MessengerDashboard.Sentiment;
using MessengerDashboard.Server;
using MessengerDashboard.Telemetry;
using MessengerTests.DashboardTests.Mocks;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class TelemetryAndSentimentTests
    {

        [TestMethod]
        public void TestSentiment()
        {
            ISentimentAnalyzer analyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();
            List<string> chats = new()
            {
                "Hi",
                "Hello",
                "good",
                "bad",
                "worst"
            };
            SentimentResult result = analyzer.AnalyzeSentiment(chats.ToArray());
            Assert.AreEqual(result.PositiveChatCount, 1);
            Assert.AreEqual(result.NegativeChatCount, 2);
            Assert.AreEqual(result.NeutralChatCount, 2);
            Assert.AreEqual(result.OverallSentiment, "Neutral");
        }

        [TestMethod]
        public void TestSentiment2()
        {
            ISentimentAnalyzer analyzer = SentimentAnalyzerFactory.GetSentimentAnalyzer();
            List<string> chats = new()
            {
                "Very good day today for a picnic.",
                "Eat as much food as possible",
                "I don't know that works or not",
                "Please stop it",
                "Thank you sir"
            };
            SentimentResult result = analyzer.AnalyzeSentiment(chats.ToArray());
            Assert.AreEqual(result.PositiveChatCount, 2);
            Assert.AreEqual(result.NegativeChatCount, 0);
            Assert.AreEqual(result.NeutralChatCount, 3);
            Assert.AreEqual(result.OverallSentiment, "Positive");
        }


        [TestMethod]
        public void TestTelemetry()
        {
            ITelemetry telemetry = TelemetryFactory.GetTelemetryInstance();
            MockServerCommunicator serverCommunicator = new ();
            MockClientCommunicator clientCommunicator1 = new ();
            serverCommunicator.AddClientCommunicator("1", clientCommunicator1);
            clientCommunicator1.SetServer(serverCommunicator);
            MockContentServer contentServer = new ();
            ServerSessionController server = new (serverCommunicator, contentServer);
            telemetry.SubscribeToServerSessionController (server);
            ClientSessionController client1 = new (clientCommunicator1, new MockContentClient(), new MockScreenShareClient());

            ConnectionDetails connectionDetails = server.ConnectionDetails;
            if (!client1.ConnectToServer(connectionDetails.IpAddress, connectionDetails.Port, "Name1", "1@gmail.com", "photo1"))
            {
                Assert.Fail("Client 1 was not able to connect to the server");
            }
            Thread.Sleep(500);
            List<ChatThread> chatThreads = new()
            {
                new() { MessageList = new List<ReceiveChatData>() { new() { SenderID = 1, Data = "hello I am good",
                        Type = MessengerContent.MessageType.Chat } } },
                new() { MessageList = new List<ReceiveChatData>() { new() { SenderID = 1, Data = "I am in a bad mood",
                        Type = MessengerContent.MessageType.Chat } } },
                new() { MessageList = new List<ReceiveChatData>() { new() { SenderID = 1, Data = "What an achivement!",
                        Type = MessengerContent.MessageType.Chat } } },
            };
            contentServer.SetChats(chatThreads);
            client1.Refreshed += (sender, args) =>
            {
                Assert.AreEqual(args.TelemetryAnalysis.TimeStampToUserCountMap.Count, 1);
                Assert.AreEqual(args.TelemetryAnalysis.UserIdToUserActivityMap.Count, 1);
                Assert.AreEqual(args.TelemetryAnalysis.TotalChatCount, 3);
                Assert.AreEqual(args.TelemetryAnalysis.TotalUserCount, 1);
            };
            client1.SendRefreshRequestToServer();
        }
    }
}
