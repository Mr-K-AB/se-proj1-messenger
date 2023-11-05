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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWhiteboard
{
    internal interface IClientCommunicator
    {
        /// <summary>
        ///     Send the Shape across the network.
        /// </summary>
        /// <param name="serverShape">Update from client to server side</param>
        void SendToServer(ServerShape serverShape);
    }
}
