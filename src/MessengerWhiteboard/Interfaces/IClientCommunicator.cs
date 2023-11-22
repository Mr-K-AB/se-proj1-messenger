/***************************
* Filename    = IClientCommunicator.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This is the IClientCommunicator Interface. 
*               The Interface deals with sending objects across the network.
***************************/

using MessengerWhiteboard.Models;

namespace MessengerWhiteboard.Interfaces
{
    public interface IClientCommunicator
    {
        /// <summary>
        ///     Send the Shape across the network.
        /// </summary>
        /// <param name="serverShape">Update from client to server side</param>
        void SendToServer(WBShape wBShape);
    }
}
