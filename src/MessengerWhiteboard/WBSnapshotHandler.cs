/***************************
* Filename    = WBSnapshotHandler.cs
*
* Author      = Syed Abdul Mateen
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file handles the implementation of
*               snapshot handling for whiteboard.
***************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        private static readonly Random s_random = new();
        public void SaveSnapshot()
        {
            string s;
            if (isServer)
            {
                if (machine is ServerState && machine != null)
                {
                    s = (machine as ServerState).OnSaveMessage(s_random.Next().ToString());
                    SavedSessions.Add(s);
                }
            }
        }
    }
}
