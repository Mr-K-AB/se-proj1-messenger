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
        readonly Utils _utils;
        
        public ServerStateTest()
        {
            _server = ServerState.Instance;
            _serializer = new Serializer();
            _utils = new Utils();
        }

        [TestMethod]
        public void AlwaysReturnsSameInstance()
        {
            ServerState server1 = ServerState.Instance;
            ServerState server2 = ServerState.Instance;
            Assert.Equals(server1, server2);
        }

        [TestMethod]
        public void ClearServerListWithSizeZero()
        {
            _server.OnShapeReceived(null, Operation.Clear);
            Assert.Equals(0, _server.GetServerListSize());
            _server.ClearServerList();
        }

        [TestMethod]
        public void SetServerSnapshotNumber()
        {
            _server.ClearServerList();
            _snapshotHandler = _server.GetSnapshotHandler();
            _server.SetSnapshot("3");
            Assert.Equals("3", _snapshotHandler.SnapshotId);
        }

        [TestMethod]
        public void RemoveShape()
        {
            _server.ClearServerList();

            Point start = new(1, 1);
            Point end = new(2, 2);

            _server.OnShapeReceived(_utils.CreateShape("RectangleGeometry", start, end, Brushes.Transparent, Brushes.Black, 1, "u0f1"), Operation.Creation);
            Assert.Equals(1, _server.GetServerListSize());

            // Size remains 1 (removing non existent object)
            _server.OnShapeReceived(_utils.CreateShape("RectangleGeometry", start, end, Brushes.Transparent, Brushes.Black, 1, "u1f1"), Operation.Deletion);
            Assert.Equals(1, _server.GetServerListSize());

            // Size becomes 0 (removing the initially created object)
            _server.OnShapeReceived(_utils.CreateShape("RectangleGeometry", start, end, Brushes.Transparent, Brushes.Black, 1, "u0f1"), Operation.Deletion);
            Assert.Equals(0, _server.GetServerListSize());
        }

        [TestMethod]
        public void CheckServerListSizeIncrease()
        {
            _server.ClearServerList();

            Point start1 = new(1, 1);
            Point end1 = new(2, 2);

            Point start2 = new(4, 4);
            Point end2 = new(6, 6);

            _server.OnShapeReceived(_utils.CreateShape("RectangleGeometry", start1, end1, Brushes.Transparent, Brushes.Black, 1, "u0f1"), Operation.Creation);
            _server.OnShapeReceived(_utils.CreateShape("RectangleGeometry", start2, end2, Brushes.Transparent, Brushes.Black, 1, "u0f2"), Operation.Creation);

            Assert.Equals(2, _server.GetServerListSize());
        }
    }
}
