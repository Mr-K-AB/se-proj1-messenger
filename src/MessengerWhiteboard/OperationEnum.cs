/***************************
* Filename    = OperationEnum.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file contains all the operations that can be executes on the 
*               Whiteboard, and includes certain messages that need to be conveyed.
***************************/

namespace MessengerWhiteboard
{
    /// <summary>
    ///         Type of operations that can be performed on the ShapeItems
    ///         and various kinds of messages that can be sent
    /// </summary>
    public enum Operation
    {
        Creation,
        Deletion,
        ModifyShape,
        NewUser,
        CreateSnapshot,
        RestoreSnapshot,
        Clear
    }
}
