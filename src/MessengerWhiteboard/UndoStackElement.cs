/***************************
* Filename    = UndoStackElement.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file contains the elements for Undo and Redo Stacks. 
***************************/

namespace MessengerWhiteboard
{
    public class UndoStackElement
    {
        /// <summary>
        ///     This is the constructor for creating an Undo Stack Element.
        /// </summary>
        /// <param name="previousShapeItem">The previous state of the shape</param>
        /// <param name="newShapeItem">The new state of the shape</param>
        /// <param name="operation">Operation that is responsible for this change of the state</param>
        public UndoStackElement(ShapeItem previousShapeItem, ShapeItem newShapeItem, Operation operation)
        {
            PreviousShapeItem = previousShapeItem;
            NewShapeItem = newShapeItem;
            Operation = operation;
        }

        public ShapeItem PreviousShapeItem { get; set; }
        public ShapeItem NewShapeItem { get; set; }
        public Operation Operation { get; set; }
    }
}
