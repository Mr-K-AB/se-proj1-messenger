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
*               It handles client-side operations when receiving shapes or 
*               messages from the ViewModel and send them to the Server.
***************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents.Serialization;

namespace MessengerWhiteboard
{
    public class ClientState : IShapeReceiver
    {
        // Using ClientCommunicator to send shape/message to Server
        IClientCommunicator _communicator;
        Serializer _serializer;
        private static ClientState _instance;

        /// <summary
        ///     Making sure there is a single instance of the client on a particular machine.
        /// </summary>
        public static ClientState Instance
        {
            get
            {
                _instance ??= new ClientState ();

                return _instance;
            }
        }

        // Constructor for Client side
        private ClientState()
        {
            _communicator = ClientCommunicator.Instance;
            _serializer = new Serializer();
            InitializeUser();
        }

        string _userID;

        // Sets the User ID
        public void SetUserID(string userID)
        {
            userID = userID;
        }

        /// <summary>
        ///     When an action is executed, both the operation and the associated
        ///     shape are transferred from the View Model to the ClientSide.
        ///     This function is responsible for transmitting the ShapeItem and Operation to the server.
        /// </summary>
        /// <param name="newShape">The ShapeItem to be sent to the server.</param>
        /// <param name="op">The Operation to be sent to the server.</param>
        public void OnShapeReceived(ShapeItem shapeItem, Operation operation)
        {
            List<ShapeItem> shapeItems = new()
            {
                shapeItem
            };

            var serializedShapes = _serializer.SerializeShapes(shapeItems);
            WBShape wBShape = new(serializedShapes, operation);
            _communicator.SendToServer(wBShape);
        }

        /// <summary>
        ///     When a new user joins the session, their details, including the user ID,
        ///     are relayed to the server using the client communicator in the form of a WBShape.
        /// </summary>
        public void InitializeUser()
        {
            WBShape wBShape = new(null, Operation.NewUser, _userID);
            _communicator.SendToServer(wBShape);
        }

        /// <summary>
        ///     This function sets a Client Communicator for testing purpose.
        /// </summary>
        public void SetCommunicator(IClientCommunicator communicator)
        {
            _communicator = communicator;
        }
    }
}
