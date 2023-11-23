/***************************
* Filename    = UndoRedo.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file handles the implementation of Undo
*               and Redo functionality.
***************************/

using System.Diagnostics;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {

        /// <summary>
        ///     This function is called when a user clicks on Undo.
        ///     Following things will happen:
        ///         1. A helper function will be called to call the Undo function.
        ///         2. The shape returned by this function will be used to get the shape and the operation.
        ///         3. The operation - which needs to be performed and the object - on which that operation has to be performed will be sent to the server.
        /// </summary>
        public void CallUndo()
        {
            UndoStackElement sendingShape = Undo();
            if (sendingShape != null)
            {
                machine.OnShapeReceived(sendingShape.PreviousShapeItem, sendingShape.Operation);
            }
        }

        /// <summary>
        ///     This function is called when a user clicks on Redo.
        ///     Following things will happen:
        ///         1. A helper function will be called to call the Redo function.
        ///         2. The shape returned by this function will be used to get the shape and the operation.
        ///         3. The operation - which needs to be performed and the object - on which that operation has to be performed will be sent to the server.
        /// </summary>
        public void CallRedo()
        {
            UndoStackElement sendingShape = Redo();
            if (sendingShape != null)
            {
                machine.OnShapeReceived(sendingShape.NewShapeItem, sendingShape.Operation);
            }
        }

        // Initialising the stacks for undo and redo
        public Stack<UndoStackElement> undoStackElements = new();
        public Stack<UndoStackElement> redoStackElements = new();

        /// <summary>
        ///     This function actually implements the Undo operation logic.
        ///     Following things will happen:
        ///         1. To get the top of the stack, the head of UndoStack will be popped.
        ///         2. A deep copy of the object will be saved as modifiedObject and will be sent to the server.
        ///         3. The inverse of the operation associated with the object will be performed and that will be the modifiedObject's operation.
        ///         4. The head to the Undo Stack will be pushed to the Redo Stack.
        ///         5. The modifiedObject will be returned to the helper function.
        /// </summary>
        /// <returns>The UndoStackElement containing the shape and the operation to be broadcasted.</returns>
        public UndoStackElement Undo()
        {
            // If Stack is empty
            if (undoStackElements.Count == 0)
            {
                return null;
            }

            UndoStackElement head = undoStackElements.Pop();
            if (head.PreviousShapeItem == null)
            {
                return null;
            }

            UndoStackElement modifiedObject = new(head.PreviousShapeItem, head.NewShapeItem, head.Operation);

            Trace.WriteLine("[White-Board] " + "\n" + head.Operation + "\n");

            // Depending on the object's operation, inverse of that is performed to modify ShapeList
            if (head.Operation == Operation.Creation)
            {
                DeleteIncomingShape(head.PreviousShapeItem);
                modifiedObject.Operation = Operation.Deletion;
            }
            else if (head.Operation == Operation.Deletion)
            {
                CreateIncomingShape(head.PreviousShapeItem);
                modifiedObject.Operation = Operation.Creation;
            }
            else if (head.Operation == Operation.ModifyShape)
            {
                ModifyIncomingShape(head.PreviousShapeItem);
                modifiedObject.Operation = Operation.ModifyShape;
            }

            redoStackElements.Push(head);
            Trace.WriteLine("[White-Board] " + "\n" + redoStackElements.Peek().Operation + " is pushed to Redo Stack \n");
            return modifiedObject;
        }

        /// <summary>
        ///     This function actually implements the Redo operation logic.
        ///     Following things will happen:
        ///         1. To get the top of the stack, the head of RedoStack will be popped.
        ///         2. The operation associated with the object will be performed.
        ///         4. The head to the Redo Stack will be pushed to the Undo Stack.
        ///         5. The Redo head will be returned to the helper function.
        /// </summary>
        /// <returns>The UndoStackElement containing the shape and the operation to be broadcasted.</returns>
        public UndoStackElement Redo()
        {
            // If Stack is empty
            if (redoStackElements.Count == 0)
            {
                return null;
            }

            UndoStackElement head = redoStackElements.Pop();

            if (head.Operation == Operation.Creation)
            {
                Trace.WriteLine("[White-Board] " + "\n Redo Creation " + head.NewShapeItem.Id + "\n");
                head.NewShapeItem.ZIndex = machine.GetMaxZindex(head.NewShapeItem);
                CreateIncomingShape(head.NewShapeItem);
            }
            else if (head.Operation == Operation.Deletion)
            {
                Trace.WriteLine("[White-Board] " + "\n Redo Deletion " + head.NewShapeItem.Id + "\n");
                DeleteIncomingShape(head.NewShapeItem);
            }
            else if (head.Operation == Operation.ModifyShape)
            {
                Trace.WriteLine("[White-Board] " + "\n Redo Modify-Shape " + head.NewShapeItem.Id + "\n");
                ModifyIncomingShape(head.NewShapeItem);
            }

            undoStackElements.Push(head);
            Trace.WriteLine("[White-Board] " + "\n" + undoStackElements.Peek().NewShapeItem.Id + " is pushed to Undo Stack \n");
            return head;
        }

        /// <summary>
        ///     Whenever client performs some action on White-Board, this function will insert the ShapeItem along with the operation performed.
        /// </summary>
        /// <param name="newShape">The UndoStackElement to be pushed on the Undo Stack.</param>
        public void InsertIntoStack(UndoStackElement newShape)
        {
            undoStackElements.Push(newShape);
            Debug.WriteLine(newShape.Operation + " Operation" + newShape.NewShapeItem.Id + "id inserted \n");
        }
    }
}
