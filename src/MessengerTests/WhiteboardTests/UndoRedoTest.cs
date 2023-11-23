/***************************
* Filename    = UndoRedoTest.cs
*
* Author      = Niharika Malvia
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This is for testing UndoRedo.cs file.
*               It contains all the test cases to check the functioning of Undo Redo.
***************************/

using System.Windows;
using MessengerWhiteboard;
using MessengerWhiteboard.Models;

namespace MessengerTests.WhiteboardTests
{
    [TestClass]
    public class UndoRedoTest
    {
        readonly ViewModel _viewModel;
        readonly Stack<UndoStackElement> _undoStack;
        readonly Stack<UndoStackElement> _redoStack;

        public UndoRedoTest()
        {
            _viewModel = ViewModel.Instance;
            _viewModel.SetUserID(1);
            _undoStack = _viewModel.undoStackElements;
            _redoStack = _viewModel.redoStackElements;
        }

        [TestMethod]
        public void TestEmptyStack()
        {
            _viewModel.ClearScreen();
            Assert.AreEqual(_undoStack.Count, 0);
            Assert.AreEqual(_redoStack.Count, 0);
        }

        [TestMethod]
        public void TestUndoStack()
        {
            _viewModel.ClearScreen();
            Guid uid = new();

            ShapeItem newShape = Utils.CreateShape("Rectangle", new Point(0, 0), new Point(10, 10), _viewModel.fillBrush, _viewModel.strokeBrush, 1, uid);
            UndoStackElement element = new(newShape, newShape, Operation.Creation);
            _viewModel.InsertIntoStack(element);
            Assert.AreEqual(_undoStack.Count, 1);
            Assert.AreEqual(_redoStack.Count, 0);

            UndoStackElement popElement = _viewModel.Undo();
            Assert.AreEqual(_undoStack.Count, 0);
            Assert.AreEqual(_redoStack.Count, 1);
            Assert.AreEqual(popElement.PreviousShapeItem, newShape);

            popElement = _viewModel.Redo();
            Assert.AreEqual(_redoStack.Count, 0);
            Assert.AreEqual(newShape, popElement.NewShapeItem);
        }

        [TestMethod]
        public void TestModifyShape()
        {
            _viewModel.ClearScreen();
            Guid uid = new();
            ShapeItem newShape = Utils.CreateShape("Rectangle", new Point(0, 0), new Point(10, 10), _viewModel.fillBrush, _viewModel.strokeBrush, 1, uid);
            UndoStackElement element = new(newShape, newShape, Operation.Creation);
            _viewModel.InsertIntoStack(element);
            Assert.AreEqual(_undoStack.Count, 1);
            Assert.AreEqual(_redoStack.Count, 0);

            ShapeItem newShape2 = Utils.CreateShape("Rectangle", new Point(0, 0), new Point(10, 10), _viewModel.fillBrush, _viewModel.strokeBrush, 2, uid);
            UndoStackElement element2 = new(newShape2, newShape, Operation.ModifyShape);
            _viewModel.InsertIntoStack(element2);
            Assert.AreEqual(_undoStack.Count, 2);
            Assert.AreEqual(_redoStack.Count, 0);

            UndoStackElement popElement = _viewModel.Undo();
            Assert.AreEqual(_undoStack.Count, 1);
            Assert.AreEqual(_redoStack.Count, 1);
            Assert.AreEqual(popElement.PreviousShapeItem, newShape);

            popElement = _viewModel.Redo();
            Assert.AreEqual(_redoStack.Count, 0);
            Assert.AreEqual(newShape2, popElement.NewShapeItem);
        }
    }
}
