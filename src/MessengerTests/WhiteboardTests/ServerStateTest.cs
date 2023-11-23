/***************************
* Filename    = ServerStateTest.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This is for testing ServerState.cs file.
*               It contains all the test cases to check the functioning of Server Side.
***************************/

using System.Windows;
using System.Windows.Media;
using MessengerWhiteboard;
using MessengerWhiteboard.Models;

namespace MessengerTests.WhiteboardTests
{
    [TestClass]
    public class ServerStateTest
    {
        private readonly ServerState _server;
        private ServerSnapshotHandler _snapshotHandler;
        private readonly Serializer _serializer;
        
        public ServerStateTest()
        {
            _server = ServerState.Instance;
            _serializer = new Serializer();
        }

        [TestMethod]
        public void AlwaysReturnsSameInstance()
        {
            ServerState server1 = ServerState.Instance;
            ServerState server2 = ServerState.Instance;
            Assert.AreEqual(server1, server2);
        }

        [TestMethod]
        public void ClearServerListWithSizeZero()
        {
            _server.OnShapeReceived(null, Operation.Clear);
            Assert.AreEqual(0, _server.GetServerListSize());
            _server.ClearServerList();
        }

        [TestMethod]
        public void SetServerSnapshotNumber()
        {
            _server.ClearServerList();
            _snapshotHandler = _server.GetSnapshotHandler();
            _server.SetSnapshot("3");
            Assert.AreEqual("3", _snapshotHandler.SnapshotId);
        }

        [TestMethod]
        public void RemoveShape()
        {
            _server.ClearServerList();

            Point start = new(1, 1);
            Point end = new(2, 2);
            Guid id1 = Guid.NewGuid();

            _server.OnShapeReceived(Utils.CreateShape("Rectangle", start, end, Brushes.Transparent, Brushes.Black, 1, id1), Operation.Creation);
            Assert.AreEqual(1, _server.GetServerListSize());

            // Size remains 1 (removing non existent object)
            _server.OnShapeReceived(Utils.CreateShape("Rectangle", start, end, Brushes.Transparent, Brushes.Black, 1, Guid.NewGuid()), Operation.Deletion);
            Assert.AreEqual(1, _server.GetServerListSize());

            // Size becomes 0 (removing the initially created object)
            _server.OnShapeReceived(Utils.CreateShape("Rectangle", start, end, Brushes.Transparent, Brushes.Black, 1, id1), Operation.Deletion);
            Assert.AreEqual(0, _server.GetServerListSize());
        }

        [TestMethod]
        public void CheckServerListSizeIncrease()
        {
            _server.ClearServerList();

            Point start1 = new(1, 1);
            Point end1 = new(2, 2);

            Point start2 = new(4, 4);
            Point end2 = new(6, 6);

            _server.OnShapeReceived(Utils.CreateShape("Rectangle", start1, end1, Brushes.Transparent, Brushes.Black, 1, Guid.NewGuid()), Operation.Creation);
            _server.OnShapeReceived(Utils.CreateShape("Rectangle", start2, end2, Brushes.Transparent, Brushes.Black, 1, Guid.NewGuid()), Operation.Creation);

            Assert.AreEqual(2, _server.GetServerListSize());
        }
    }
}
