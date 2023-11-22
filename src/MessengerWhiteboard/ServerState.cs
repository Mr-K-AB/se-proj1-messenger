/***************************
* Filename    = ServerState.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This represents Server Side Implementation.
*               It handles server-side operations when receiving shapes or messages
*               from the ViewModel of the server and broadcasts it to clients.
*               It also contains the Server Side White-board State Management.
***************************/

using System.Diagnostics;
using MessengerWhiteboard.Interfaces;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public class ServerState : IShapeReceiver
    {
        private static IServerCommunicator s_communicator;
        private readonly Serializer _serializer;
        private static ServerState s_instance;
        private readonly ServerSnapshotHandler _serverSnapshotHandler;

        /// <summary>
        ///     Making sure there is a single instance of the server on a particular machine.
        /// </summary>
        public static ServerState Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ServerState();
                    s_communicator = ServerCommunicator.Instance;
                }

                return s_instance;
            }
        }

        // Constructor for Server side
        private ServerState()
        {
            _serializer = new Serializer();
        }


        /// <summary>
        ///     This function gives server snap-shot handler for testing purpose.
        /// </summary>
        /// <returns>Snap-shot handler.</returns>
        //public ServerSnapshotHandler GetSnapshotHandler()
        //{
        //    return _serverSnapshotHandler;
        //}


        /// <summary>
        ///     This represents the server's list, which is the Server-Side State. It's a
        ///     ShapeID-to-shape mapping that contains all the ShapeItems currently on the Whiteboard.
        /// </summary>
        private Dictionary<Guid, ShapeItem> _mapping = new();

        /// <summary>
        ///     This function returns the size of the server list for testing purpose.
        /// </summary>
        /// <returns>Size of the Server List, means total number of shapes currently on the white-board.</returns>
        public int GetServerListSize()
        {
            return _mapping.Count();
        }


        /// <summary>
        ///     This function handles the creation of snap-shot. It invokes the server snap-shot handler
        ///     to store the current server list of ShapeItems as a snap-shot, including the user ID.
        ///     Subsequently, it broadcasts the received WBShape to all clients.
        /// </summary>
        /// <param name="deserializedObject">Deserialized object received from the network.</param>
        /// <returns>Current snap-shot number.</returns>
        //public int CreateSnapshotHandler(WBShape deserializedObject)
        //{
        //    int n = _serverSnapshotHandler.SaveBoard(_mapping.Values.ToList(), deserializedObject.UserId);
        //    s_communicator.Broadcast(deserializedObject, null);
        //    return n;
        //}


        /// <summary>
        ///     When a ShapeItem is received from the Client or View Model, it modifies the server-side
        ///     list of ShapeItems based on the operation performed on the ShapeItem.
        /// </summary>
        /// <param name="shapeItem">ShapeItem received from the client.</param>
        /// <param name="operation">Operation preformed.</param>
        public void OnShapeReceived(ShapeItem shapeItem, Operation operation)
        {
            if (operation == Operation.Creation)
            {
                Trace.WriteLine($"HandleData: {shapeItem.Id}");
                AddShapeToServerList(shapeItem.Id, shapeItem, operation);
            }
            else if (operation == Operation.Deletion)
            {
                RemoveShapeFromServerList(shapeItem.Id, shapeItem, operation);
            }
            else if (operation == Operation.ModifyShape)
            {
                UpdateShapeInServerList(shapeItem.Id, shapeItem, operation);
            }
            else if (operation == Operation.Clear)
            {
                ClearShapesInServerList(shapeItem, operation);
            }
        }


        string _userId;

        /// <summary>
        ///     This function sets the User ID.
        /// </summary>
        /// <param name="userId">Value of the user ID to be set.</param>
        public void SetUserId(string userId)
        {
            _userId = userId;
        }


        /// <summary>
        ///     When a new user joins the session, all the ShapeItems currently in the server list for that session
        ///     are transmitted to the new user. These shapes undergo serialization and are then transformed into
        ///     a WBServerShape with the 'NewUser' operation. Subsequently, the WBServerShape is exclusively broadcasted
        ///     to the user corresponding to the userId.
        /// </summary>
        /// <param name="deserializedObject">The deserialized object received from the network.</param>

        public void InitializeUser(WBShape deserializedObject)
        {
            List<ShapeItem> shapeItems = _mapping.Values.ToList();
            List<SerializableShapeItem> serializableShapeItems = _serializer.SerializeShapes(shapeItems);

            WBShape wBShape = new(serializableShapeItems, Operation.NewUser, deserializedObject.UserId);

            s_communicator.Broadcast(wBShape, deserializedObject.UserId);
        }


        private static int s_maxZindex = 0;     // For storing Zindex of the shape

        /// <summary>
        ///     This function increments the Zindex value and returns
        ///     the current value of Zindex for the last shape.
        /// </summary>
        /// <param name="lastShape">Shape whose Zindeex is needed.</param>
        public int GetMaxZindex(ShapeItem lastShape)
        {
            s_maxZindex++;
            return s_maxZindex;
        }


        /// <summary>
        ///     This function sets the snapshot number.
        /// </summary>
        /// <param name="snapshotNumber">Value of the snap-shot number to be set.</param>
        public void SetSnapshot(string ssid)
        {
            _serverSnapshotHandler.SnapshotId = ssid;
        }


        /// <summary>
        ///     When a request to restore a snapshot is received, a new WBShape is created with
        ///     the operation set to RestoreSnapshot. The serverSnapshotHandler utilizes the 
        ///     loadboard function to retrieve the snapshot as a list of ShapeItems. Subsequently,
        ///     these shapes undergo serialization and are transformed into a WBShape containing
        ///     all the ShapeItems, which is then roadcasted to all clients
        /// </summary>
        /// <param name="snapshotNumber">The snap-shot number of the snap-shot which needs to be restoreed.</param>
        /// <param name="userId">User ID of the user who wants to restore the snap-shot.</param>
        /// <returns>The restored loaded shapes.</returns>
        public List<ShapeItem> OnLoadMessage(string ssid)
        {
            WBShape deserializedObject = new(null, Operation.RestoreSnapshot);

            Trace.WriteLine("[Whiteboard] ServerState.RestoreSnapshotHandler: Restoring Snapshot " + deserializedObject.SnapshotID);
            Trace.WriteLine("[Whiteboard] " + GetServerListSize());

            List<ShapeItem> loadedShapes = _serverSnapshotHandler.LoadSession(deserializedObject.SnapshotID);
            //List<SerializableShapeItem> serializableShapeItems = _serializer.SerializeShapes(loadedShapes);

            //WBShape wBShape = new(serializableShapeItems, Operation.RestoreSnapshot, deserializedObject.UserId);

            BroadcastToClients(loadedShapes, Operation.RestoreSnapshot);
            return loadedShapes;
        }


        /// <summary>
        ///     When a request to save a snapshot is received, a new WBServerShape is created with
        ///     the operation set to CreateSnapshot. This WBShape is then passed to the CreateSnapshotHandler.
        /// </summary>
        /// <param name="userId">User ID of the user who wants to save the snap-shot.</param>
        /// <returns>Snap-shot number of the new snap-shot created.</returns>
        public string OnSaveMessage(string ssid)
        {
            WBShape wBShape = new(null, Operation.CreateSnapshot, "0", ssid);
            return CreateSnapshot(wBShape);
        }

        public string CreateSnapshot(WBShape wBShape)
        {
            string s = _serverSnapshotHandler.SaveSession(wBShape.SnapshotID, _mapping.Values.ToList());

            return s;
        }



        /// <summary>
        ///     This function will add the ShapeItem in the dictionary mapping with the key as
        ///     shapeId and then also broadcast with the operation as 'Creation'.
        /// </summary>
        /// <param name="shapeId">Id of the shape to be added to the server list.</param>
        /// <param name="shapeItem">Type of the shape created.</param>
        /// <param name="operation">Operation Performed - Creation.</param>
        private void AddShapeToServerList(Guid shapeId, ShapeItem shapeItem, Operation operation)
        {
            try
            {
                if (_mapping.ContainsKey(shapeId))
                {
                    return;
                }
                _mapping.Add(shapeId, shapeItem);
                Trace.WriteLine("[White-Board] " + "inside AddShapeToServerList" + shapeItem.Id);
                Trace.WriteLine("[White-Board] " + "inside AddShapeToServerList" + shapeItem.Geometry.GetType().Name);
                BroadcastToClients(shapeItem, operation);
            }
            catch (Exception e)
            {
                Trace.WriteLine("[White-Board] Error Occured in ServerSide: AddShapeToServerSide");
                Trace.WriteLine("exception: ", e.Message);
            }
        }


        /// <summary>
        ///     This funciton will remove the ShapeItem corresponding to shapeId from the 
        ///     dictionary mapping and also broadcast with the operation as 'Deletion'.
        /// </summary>
        /// <param name="shapeId">Id of the shape to be removed from the server list.</param>
        /// <param name="shapeItem">Type of the shape deleted.</param>
        /// <param name="operation">Operation Performed - Deletion.</param>
        private void RemoveShapeFromServerList(Guid shapeId, ShapeItem shapeItem, Operation operation)
        {
            if (_mapping.ContainsKey(shapeId))
            {
                _mapping.Remove(shapeId);
                BroadcastToClients(shapeItem, operation);
            }
        }


        /// <summary>
        ///     This funciton will update the ShapeItem corresponding to shapeId from the dictionary mapping
        ///     whenever that shape is modified and also broadcast with the operation as 'ModifyShape'.
        /// </summary>
        /// <param name="shapeId">Id of the shape to be updated in the server list.</param>
        /// <param name="shapeItem">Type of the shape updated.</param>
        /// <param name="operation">Operation Performed - which led to the modification.</param>
        private void UpdateShapeInServerList(Guid shapeId, ShapeItem shapeItem, Operation operation)
        {
            if (_mapping.ContainsKey(shapeId))
            {
                _mapping[shapeId] = shapeItem;
                BroadcastToClients(shapeItem, operation);
            }
        }


        /// <summary>
        ///     This funciton will clear all the shapes from the dictionary mapping - basically
        ///     clearing the server list and also broadcast with the operation as 'Clear'.
        /// </summary>
        /// <param name="shapeItem">Type of the shape deleted.</param>
        /// <param name="operation">Operation Performed - Clear.</param>
        private void ClearShapesInServerList(ShapeItem shapeItem, Operation operation)
        {
            _mapping = new Dictionary<Guid, ShapeItem>();
            BroadcastToClients(shapeItem, operation);
        }


        /// <summary>
        ///         To clear the server list. This will be used by some utility functions.
        /// </summary>
        public void ClearServerList()
        {
            _mapping.Clear();
        }


        /// <summary>
        ///     This is a function for broadcasting single shape to clients.
        /// </summary>
        /// <param name="shapeItem">List of shapes that needs to be broadcast.</param>
        /// <param name="operation">Operation performed.</param>
        public void BroadcastToClients(ShapeItem shapeItem, Operation operation)
        {
            s_communicator.Broadcast(shapeItem, operation);
        }


        /// <summary>
        ///     This is a function for broadcasting List of Shapes to clients.
        /// </summary>
        /// <param name="shapeItems">List of shapes that needs to be broadcast.</param>
        /// <param name="operation">Operation performed.</param>
        public void BroadcastToClients(List<ShapeItem> shapeItems, Operation operation)
        {
            s_communicator.Broadcast(shapeItems, operation);
        }
    }
}
