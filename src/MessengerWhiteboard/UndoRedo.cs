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
        public void CallUndo()
        {
            UndoStackElement sendingShape = Undo();
            if (sendingShape != null)
            {
                machine.OnShapeReceived(sendingShape.PreviousShapeItem, sendingShape.Operation);
            }
        }

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

            if (head.Operation == Operation.Creation)
            {
                DeleteIncomingShape(head.PreviousShapeItem);
                modifiedObject.Operation = Operation.Deletion;
            }
            else if (head.Operation == Operation.Deletion)
            {
                //head.NewShapeItem.ZIndex = machine.GetMaxZindex(head.NewShapeItem);
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

        public UndoStackElement Redo()
        {
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

        public void InsertIntoStack(UndoStackElement newShape)
        {
            undoStackElements.Push(newShape);
            Debug.WriteLine(newShape.Operation + " Operation" + newShape.NewShapeItem.Id + "id inserted \n");
        }
    }
}
