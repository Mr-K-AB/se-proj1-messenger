/***************************
* Filename    = IShapeReceiver.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This is the IShapeReceiver Interface. 
*               It has all the functions (to receive shapes and messages) that 
*               must be implemented by Server and Client.
***************************/

using MessengerWhiteboard.Models;

namespace MessengerWhiteboard.Interfaces
{
    public interface IShapeReceiver
    {
        void OnShapeReceived(ShapeItem shapeItem, Operation operation);
        void SetUserId(string userId);
        public int GetMaxZindex(ShapeItem lastShape);
        //public void SetSnapshotNumber(int snapshotNumber);
        //public List<ShapeItem> OnLoadMessage(int snapshotNumber, string userId);
        //public int OnSaveMessage(string userId);

    }
}
