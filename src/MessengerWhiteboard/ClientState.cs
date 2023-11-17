/***************************
* Filename    = ClientState.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This represents Cliend Side Implementation. 
*               It handles client-side operations when receiving shapes or messages
*               from the ViewModel and send them to the Serverusing interface.
***************************/

using System.Diagnostics;
using MessengerWhiteboard.Interfaces;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public class ClientState : IShapeReceiver
    {
        // Using ClientCommunicator to send shape/message to Server
        IClientCommunicator _communicator;
        readonly Serializer _serializer;
        private static ClientState? s_instance;
        //ClientSnapshotHandler _clientSnapshotHandler;

        /// <summary>
        ///     Making sure there is a single instance of the client on a particular machine.
        /// </summary>
        public static ClientState Instance
        {
            get
            {
                s_instance ??= new ClientState ();

                return s_instance;
            }
        }

        // Constructor for Client side
        private ClientState()
        {
            _communicator = ClientCommunicator.Instance;
            _serializer = new Serializer();
            //_clientSnapshotHandler = new ClientSnapshotHandler();
            InitializeUser();
        }

        /// <summary>
        ///     This function sets a Client Communicator for testing purpose.
        /// </summary>
        /// <param name="communicator">Client Communicator needed to be tested.</param>
        public void SetCommunicator(IClientCommunicator communicator)
        {
            _communicator = communicator;
        }

        /// <summary>
        ///     This function gives client snap-shot handler for testing purpose.
        /// </summary>
        /// <returns>Snap-shot handler.</returns>
        //public ClientSnapshotHandler GetSnapshotHandler()
        //{
        //    return _clientSnapshotHandler;
        //}

        /// <summary>
        ///     When an action is executed, both the operation and the associated
        ///     shape are transferred from the View Model to the ClientSide.
        ///     This function is responsible for transmitting the ShapeItem and Operation to the server.
        /// </summary>
        /// <param name="shapeItem,">The ShapeItem to be sent to the server.</param>
        /// <param name="operation">The Operation to be sent to the server.</param>
        public void OnShapeReceived(ShapeItem shapeItem, Operation operation)
        {
            List<ShapeItem> shapeItems = new()
            {
                shapeItem
            };

            List<SerializableShapeItem> serializedShapes = _serializer.SerializeShapes(shapeItems);
            WBShape wBShape = new(serializedShapes, operation);
            Debug.Print("OnShapeReceived: {0}", shapeItem.ShapeType);
            _communicator.SendToServer(wBShape);
        }

        public string _userId;

        /// <summary>
        ///     This function sets the User ID.
        /// </summary>
        /// <param name="userID">Value of the user ID to be set.</param>
        public void SetUserId(string userId)
        {
            _userId = userId;
        }

        /// <summary>
        ///     When a new user joins the session, their details, including the user ID,
        ///     are relayed to the server using the client communicator in the form of a WBShape.
        /// </summary>
        public void InitializeUser()
        {
            WBShape wBShape = new(null, Operation.NewUser, _userId);
            _communicator.SendToServer(wBShape);
        }

        /// <summary>
        ///     This function returns the Zindex of the last shape.
        /// </summary>
        /// <param name="lastShape">Shape whose Zindex is needed.</param>
        /// <returns>Zindex.</returns>
        public int GetMaxZindex(ShapeItem lastShape)
        {
            return lastShape.ZIndex;
        }

        /// <summary>
        ///     This function sets the snapshot number.
        /// </summary>
        /// <param name="snapshotNumber">Value of the snap-shot number to be set.</param>
        //public void SetSnapshotNumber(int snapshotNumber)
        //{
        //    _clientSnapshotHandler.SnapshotNumber = snapshotNumber;
        //}

        /// <summary>
        ///     This function restores a snap-shot, for which we use RestoreSnapshot function.
        /// </summary>
        /// <param name="snapshotNumber">The snap-shot number of the snap-shot which needs to be restoreed.</param>
        /// <param name="userId">User ID of the user who wants to restore the snap-shot.</param>
        /// <returns>Null value.</returns>
        //public List<ShapeItem> OnLoadMessage(int snapshotNumber, string userId)
        //{
        //    _clientSnapshotHandler.RestoreSnapshot(snapshotNumber, userId);
        //    return null;
        //}

        /// <summary>
        ///     This function saves the snap-shot, for which we use SaveSnapshot function.
        /// </summary>
        /// <param name="userId">User ID of the user who wants to save the snap-shot.</param>
        /// <returns>Snap-shot number of the new snap-shot created.</returns>
        //public int OnSaveMessage(string userId)
        //{
        //    return _clientSnapshotHandler.SaveSnapshot(userId);
        //}
    }
}
