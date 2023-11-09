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
*               It has all the functions (to receive shapes) that must be implemented
*               by both machines.
***************************/

namespace MessengerWhiteboard
{
    public interface IShapeReceiver
    {
        void OnShapeReceived(ShapeItem shapeItem, Operation operation);
        void SetUserId(string userId);
    }
}
