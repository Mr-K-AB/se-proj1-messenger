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
namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        private static readonly Random s_random = new();
        public void SaveSnapshot(string snapshotName)
        {
            string s;
            if (isServer)
            {
                if (machine is ServerState && machine != null)
                {
                    s = (machine as ServerState).OnSaveMessage(snapshotName);
                    SavedSessions.Add(s);
                }
            }
        }
    }
}
