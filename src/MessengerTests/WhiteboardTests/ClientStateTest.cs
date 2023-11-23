/***************************
* Filename    = ClientStateTest.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This is for testing ClientState.cs file.
*               It contains all the test cases to check the functioning of Client Side.
***************************/

using System.Windows;
using System.Windows.Media;
using MessengerWhiteboard;
using MessengerWhiteboard.Interfaces;
using MessengerWhiteboard.Models;
using Moq;

namespace MessengerTests.WhiteboardTests
{
    [TestClass]
    public class ClientStateTest
    {
        readonly ClientState _client;
        private readonly Mock<IClientCommunicator> _mockCommunicator;
        private readonly Serializer _serializer;

        public ClientStateTest()
        {
            _client = ClientState.Instance;
            _mockCommunicator = new Mock<IClientCommunicator>();
            _serializer = new Serializer();
            _client.SetCommunicator(_mockCommunicator.Object);
        }

        [TestMethod]
        public void AlwaysReturnsSameInstance()
        {
            ClientState client1 = ClientState.Instance;
            ClientState client2 = ClientState.Instance;
            Assert.AreEqual(client1, client2);
        }

        [TestMethod]
        public void NewUserInfoPassingToCommunicator()
        {
            _mockCommunicator.Setup(a => a.SendToServer(It.IsAny<WBShape>()));
            _client.SetUserId("2");
            _client.InitializeUser();
            WBShape expected = new(null, Operation.NewUser, "2");

            _mockCommunicator.Verify(a => a.SendToServer(It.Is<WBShape>(obj => Utils.CompareBoardServerShapes(obj, expected))), Times.Once());
        }

        [TestMethod]
        public void ClientSendToCommunicator()
        {
            _mockCommunicator.Setup(a => a.SendToServer(It.IsAny<WBShape>()));
            _client.SetUserId("1");


            Point start = new(1, 1);
            Point end = new(2, 2);
            ShapeItem boardShape = Utils.CreateShape("Ellipse", start, end, Brushes.Transparent, Brushes.Black, 1, Guid.NewGuid());
            List<ShapeItem> newShapes = new()
            {
                boardShape
            };

            List<SerializableShapeItem> newSerializedShapes = _serializer.SerializeShapes(newShapes);
            WBShape expected = new(newSerializedShapes, Operation.Deletion, userID: "1");

            _client.OnShapeReceived(boardShape, Operation.Deletion);

            _mockCommunicator.Verify(m => m.SendToServer(It.Is<WBShape>(obj => Utils.CompareBoardServerShapes(obj, expected))), Times.Once());
        }
    }
}
