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

namespace MessengerWhiteboard
{
    public class ServerState : IShapeReceiver
    {
        private static IServerCommunicator _communicator;
        private Serializer _serializer;
        private static ServerState _instance;

        /// <summary>
        ///     Making sure there is a single instance of the server on a particular machine.
        /// </summary>
        public static ServerState Instance
        {
            get
            {
                _instance ??= new ServerState();

                return _instance;
            }
        }

        // Constructor for Server side
        private ServerState()
        {
            _serializer = new Serializer();
        }

        string _userID;

        // Sets the User ID
        public void SetUserId(string userID)
        {
            _userID = userID;
        }

        /// <summary>
        ///     This represents the server's list, which is the Server-Side State. It's an
        ///     ShapeID-to-shape mapping that contains all the ShapeItems currently on the Whiteboard.
        /// </summary>
        private Dictionary<Guid, ShapeItem> _mapping = new();

        // To return the size of the server list for testing purpose
        public int GetServerListSize()
        {
            return _mapping.Count();
        }


        /// <summary>
        ///     When a ShapeItem is received from the Client or View Model, it modifies the server-side
        ///     list of ShapeItems based on the operation performed on the ShapeItem.
        /// </summary>
        /// <param name="shapeItem">ShapeItem received from the client</param>
        /// <param name="operation">Operation preformed</param>
        public void OnShapeReceived(ShapeItem shapeItem, Operation operation)
        {
            if (operation == Operation.Creation)
            {
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


        /// <summary>
        ///     This function will add the ShapeItem in the dictionary mapping with the key as
        ///     shapeId and then also broadcast with the operation as 'Creation'.
        /// </summary>
        /// <param name="shapeId">Id of the shape to be added to the server list</param>
        /// <param name="shapeItem">Type of the shape created</param>
        /// <param name="operation">Operation Performed - Creation</param>
        private void AddShapeToServerList(Guid shapeId, ShapeItem shapeItem, Operation operation)
        {
            try
            {
                _mapping.Add(shapeId, shapeItem);
                Trace.WriteLine("[White-Board] " + "inside AddShapeToServerList" + shapeItem.Id);
                Trace.WriteLine("[White-Board] " + "inside AddShapeToServerList" + shapeItem.Geometry.GetType().Name);
                BroadcastToClients(shapeItem, operation);
            }
            catch(Exception e)
            {
                Trace.WriteLine("[White-Board] Error Occured in ServerSide: AddShapeToServerSide");
                Trace.WriteLine(e.Message);
            }
        }


        /// <summary>
        ///     This funciton will remove the ShapeItem corresponding to shapeId from the 
        ///     dictionary mapping and also broadcast with the operation as 'Deletion'.
        /// </summary>
        /// <param name="shapeId">Id of the shape to be removed from the server list</param>
        /// <param name="shapeItem">Type of the shape deleted</param>
        /// <param name="operation">Operation Performed - Deletion</param>
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
        /// <param name="shapeId">Id of the shape to be updated in the server list</param>
        /// <param name="shapeItem">Type of the shape updated</param>
        /// <param name="operation">Operation Performed - which led to the modification</param>
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
        /// <param name="shapeItem">Type of the shape deleted</param>
        /// <param name="operation">Operation Performed - Clear</param>
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

        // Function for broadcasting single shape to clients
        public void BroadcastToClients(ShapeItem shapeItem, Operation operation)
        {
            _communicator.Broadcast(shapeItem, operation);
        }

        // Function for broadcasting List of Shapes to clients
        public void BroadcastToClients(List<ShapeItem> shapeItems, Operation operation)
        {
            _communicator.Broadcast(shapeItems, operation);
        }


        /// <summary>
        ///     When a new user joins the session, the user needs to receive all the ShapeItems currently 
        ///     present on the server. These shapes are serialized and then transformed into a WBShape with
        ///     the 'NewUser' operation. Subsequently, the WBShape is exclusively sent to the user assocaited 
        ///     with the UserID.
        /// </summary>
        /// <param name="deserializedObject">Deserialized Object received from the network</param>
        public void InitializeUser(WBShape deserializedObject)
        {
            List<ShapeItem> shapeItems = _mapping.Values.ToList();
            List<SerializableShapeItem> serializableShapeItems = _serializer.SerializeShape(shapeItems);
            WBShape wBShape = new(serializableShapeItems, Operation.NewUser, deserializedObject.UserId);
            _communicator.Broadcast(wBShape, deserializedObject.UserId)
        }

    }
}
