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
    /// <summary>
    ///     Implements all the functions required for a machine.
    /// </summary>
    public interface IShapeReceiver
    {
        void OnShapeReceived(ShapeItem newShape);
        void setUserId(int userId);

    }
}
