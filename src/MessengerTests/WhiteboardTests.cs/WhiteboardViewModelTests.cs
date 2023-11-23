using System;
using MessengerWhiteboard;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerWhiteboard.Models;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using static MessengerWhiteboard.ViewModel;
using System.Reflection.PortableExecutable;
using MessengerWhiteboard.Interfaces;
using Moq;

namespace MessengerTests
{
    [TestClass]
    public class WhiteboardViewModelTests
    {
        [TestMethod]
        public void AddShapeTest()
        {
            ViewModel _viewModel = new();
            
            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            // call the method to be tested
            _viewModel.AddShape(s);

            // Assert
            Assert.AreEqual(1, _viewModel.ShapeItems.Count);
            Assert.AreSame(s, _viewModel.ShapeItems[0]);
        }

        [TestMethod]
        public void RemoveShapeTest()
        {
            ViewModel _viewModel = new();

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s1 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            start = new(0, 0);
            end = new(200, 200);
            ShapeItem s2 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            _viewModel.AddShape(s1);
            _viewModel.AddShape(s2);
            _viewModel.RemoveShape(s1);

            // Assert
            Assert.AreEqual(1, _viewModel.ShapeItems.Count);
            Assert.AreSame(s2, _viewModel.ShapeItems[0]);
        }

        [TestMethod]
        public void ChangeModeTest()
        {
            ViewModel _viewModel = new();

            WBModes newMode = WBModes.CreateMode;
            _viewModel.ChangeMode(newMode);
            Assert.AreEqual(newMode, _viewModel.currentMode);

            newMode = WBModes.SelectMode;
            _viewModel.ChangeMode(newMode);
            Assert.AreEqual(newMode, _viewModel.currentMode);

            newMode = WBModes.ViewMode;
            _viewModel.ChangeMode(newMode);
            Assert.AreEqual(newMode, _viewModel.currentMode);

            newMode = WBModes.UndoMode;
            _viewModel.ChangeMode(newMode);
            Assert.AreEqual(newMode, _viewModel.currentMode);

            newMode = WBModes.RedoMode;
            _viewModel.ChangeMode(newMode);
            Assert.AreEqual(newMode, _viewModel.currentMode);

            newMode = WBModes.DeleteMode;
            _viewModel.ChangeMode(newMode);
            Assert.AreEqual(newMode, _viewModel.currentMode);
        }


        [TestMethod]
        public void ChangeToolTest()
        {
            ViewModel _viewModel = new();
            
            string newTool = "Rectangle";

            _viewModel.ChangeTool(newTool);

            Assert.AreEqual(newTool, _viewModel.activeTool);
        }

        [TestMethod]
        public void ClearScreenTest()
        {
            ViewModel _viewModel = new()
            {
                machine = ClientState.Instance,
                _testing = true
            };

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s1 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            start = new(0, 0);
            end = new(200, 200);
            ShapeItem s2 = _viewModel.CreateShape("Ellipse", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            _viewModel.AddShape(s1);
            _viewModel.machine.OnShapeReceived(s1, Operation.Creation);
            _viewModel.AddShape(s2);
            _viewModel.machine.OnShapeReceived(s1, Operation.Creation);

            // call the method to be tested
            _viewModel.ClearScreen();

            // Assert
            Assert.AreEqual(0, _viewModel.ShapeItems.Count);
        }

        [TestMethod]
        public void ChangeStrokeWidthTest()
        {
            // Arrange
            ViewModel _viewModel = new();
            int newWidth = 5;

            // Act
            _viewModel.ChangeStrokeWidth(newWidth);

            // Assert
            Assert.AreEqual(newWidth, _viewModel._strokeWidth);
        }

        [TestMethod]
        public void ChangeStrokeBrushTest()
        {
            // Arrange
            ViewModel _viewModel = new();
            Brush newBrush = Brushes.Blue;

            // Act
            _viewModel.ChangeStrokeBrush(newBrush);

            // Assert
            Assert.AreEqual(newBrush, _viewModel.strokeBrush);
        }

        [TestMethod]
        public void ChangeFillBrushTest()
        {
            // Arrange
            ViewModel _viewModel = new();
            Brush newBrush = Brushes.Blue;

            // Act
            _viewModel.ChangeFillBrush(newBrush);

            // Assert
            Assert.AreEqual(newBrush, _viewModel.fillBrush);
        }

        [TestMethod]
        public void SetUserIDTest()
        {
            ViewModel _viewModel = new();
            // Arrange
            int userId = 1; // This will set isServer to false

            // Act
            _viewModel.SetUserID(userId);

            // Assert
            Assert.AreEqual(userId.ToString(), _viewModel._userID);
            Assert.IsFalse(_viewModel.isServer);
            Assert.IsInstanceOfType(_viewModel.machine, typeof(ClientState)); 
        }

        [TestMethod]
        public void SetUserIDTest2()
        {
            ViewModel _viewModel = new();
            // Arrange
            int userId = 2; // Any value other than 1 sets isServer to true

            // Act
            _viewModel.SetUserID(userId);

            // Assert
            Assert.AreEqual(userId.ToString(), _viewModel._userID);
            Assert.IsTrue(_viewModel.isServer);
            Assert.IsInstanceOfType(_viewModel.machine, typeof(ServerState)); 
        }


        [TestMethod]
        public void SelectShapeTest()
        {
            ViewModel _viewModel = new();

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s1 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            start = new(0, 0);
            end = new(200, 200);
            ShapeItem s2 = _viewModel.CreateShape("Ellipse", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            _viewModel.AddShape(s1);
            _viewModel.AddShape(s2);

            // call the method to be tested
            _viewModel.SelectShape(s1.Id.ToString());
            Assert.AreEqual(s1, _viewModel._tempShape);

            // call the method to be tested
            _viewModel.SelectShape(s2.Id.ToString());
            Assert.AreEqual(s2, _viewModel._tempShape);
        }

        [TestMethod]
        public void UnselectShapeTest()
        {
            ViewModel _viewModel = new();

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s1 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            start = new(0, 0);
            end = new(200, 200);
            ShapeItem s2 = _viewModel.CreateShape("Ellipse", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            _viewModel.AddShape(s1);
            _viewModel.AddShape(s2);

            // call the method to be tested
            _viewModel.SelectShape(s1.Id.ToString());
            Assert.AreEqual(s1, _viewModel._tempShape);
            _viewModel.UnselectShape();
            Assert.IsNull(_viewModel._tempShape);

            // call the method to be tested
            _viewModel.SelectShape(s2.Id.ToString());
            Assert.AreEqual(s2, _viewModel._tempShape);
            _viewModel.UnselectShape();
            Assert.IsNull(_viewModel._tempShape);
        }

        [TestMethod]
        public void StartShapeTest()
        {
            ViewModel _viewModel = new();
            Point startPoint = new(10, 10);

            // Act
            _viewModel.StartShape(startPoint);

            // Assert
            Assert.IsNotNull(_viewModel._tempShape);
            Assert.AreEqual(1, _viewModel.ShapeItems.Count);
        }

        [TestMethod]
        public void GenerateRectangleBBTest()
        {
            // Arrange
            ViewModel _viewModel = new();
            Point start = new(10, 10);
            Point end = new(20, 20);
            Brush fill = Brushes.Green;
            Brush stroke = Brushes.Blue;
            string name = "TestRectangle";

            // Act
            ShapeItem result = _viewModel.GenerateRectangleBB(start, end, fill, stroke, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Rectangle", result.ShapeType);
            Assert.AreEqual(new Rect(start, end), result.boundary);
            Assert.AreEqual(fill, result.Fill);
            Assert.AreEqual(stroke, result.Stroke);
            Assert.AreEqual(name, result.TextString);
        }


        [TestMethod]
        public void CreateShapeRectangleTest()
        {
            // Arrange
            ViewModel _viewModel = new();
            string shapeType = "Rectangle";
            Point start = new(0, 0);
            Point end = new(100, 100);
            Brush fillBrush = Brushes.Red;
            Brush borderBrush = Brushes.Blue;
            double strokeThickness = 1.0;

            // Act
            ShapeItem result = _viewModel.CreateShape(shapeType, start, end, fillBrush, borderBrush, strokeThickness);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(shapeType, result.ShapeType);
            Assert.IsInstanceOfType(result.Geometry, typeof(RectangleGeometry));
            Assert.AreEqual(new Rect(start, end), result.boundary);
            Assert.AreEqual(fillBrush, result.Fill);
            Assert.AreEqual(borderBrush, result.Stroke);
            Assert.AreEqual(strokeThickness, result.StrokeThickness);
        }

        [TestMethod]
        public void CreateShapeEllipseTest()
        {
            // Arrange
            ViewModel _viewModel = new();
            string shapeType = "Ellipse";
            Point start = new(0, 0);
            Point end = new(100, 100);
            Brush fillBrush = Brushes.Green;
            Brush borderBrush = Brushes.Orange;
            double strokeThickness = 2.0;

            // Act
            ShapeItem result = _viewModel.CreateShape(shapeType, start, end, fillBrush, borderBrush, strokeThickness);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(shapeType, result.ShapeType);
            Assert.IsInstanceOfType(result.Geometry, typeof(EllipseGeometry));
            Assert.AreEqual(new Rect(start, end), result.boundary);
            Assert.AreEqual(fillBrush, result.Fill);
            Assert.AreEqual(borderBrush, result.Stroke);
            Assert.AreEqual(strokeThickness, result.StrokeThickness);
        }

        [TestMethod]
        public void HighlightShapeTest()
        {
            // Arrange
            ViewModel _viewModel = new();

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s1 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            // Act
            _viewModel.HighlightShape(s1);

            // Assert
            Assert.AreEqual(3, _viewModel.HighlightShapeItems.Count);
            Assert.AreEqual("hsBody", _viewModel.HighlightShapeItems[0].TextString);
            Assert.AreEqual("hsTopLeft", _viewModel.HighlightShapeItems[1].TextString);
            Assert.AreEqual("hsBottomRight", _viewModel.HighlightShapeItems[2].TextString);
        }


        [TestMethod]
        public void UnhighlightShapeTest()
        {
            // Arrange
            ViewModel _viewModel = new();

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem s1 = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            // Act
            _viewModel.HighlightShape(s1);

            // Assert
            Assert.AreEqual(3, _viewModel.HighlightShapeItems.Count);
            Assert.AreEqual("hsBody", _viewModel.HighlightShapeItems[0].TextString);
            Assert.AreEqual("hsTopLeft", _viewModel.HighlightShapeItems[1].TextString);
            Assert.AreEqual("hsBottomRight", _viewModel.HighlightShapeItems[2].TextString);

            // Act
            _viewModel.UnhighlightShape(s1);

            // Assert
            Assert.AreEqual(0, _viewModel.HighlightShapeItems.Count);
        }


        [TestMethod]
        public void MoveShapeTest()
        {

            // Arrange
            ViewModel _viewModel = new()
            {
                activeTool = "Select"
            };

            Point start = new(0, 0);
            Point end = new(100, 100);
            _viewModel._tempShape = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");

            _viewModel.lastDownPoint = new Point(10, 10);
            Point newPoint = new(20, 20);

            // Act
            _viewModel.BuildShape(newPoint);

            // Assert
            Rect expectedBoundary = new(10, 10, 100, 100);
            Assert.AreEqual(expectedBoundary, _viewModel._tempShape.boundary);
        }

        [TestMethod]
        public void ResizeShapeTest()
        {
            // Arrange
            ViewModel _viewModel = new()
            {
                activeTool = "Select"
            };


            Point start = new(0, 0);
            Point end = new(100, 100);
            _viewModel._tempShape = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");
            _viewModel.HighlightShape(_viewModel._tempShape);

            _viewModel.lastDownPoint = new Point(100, 100);
            _viewModel._selectedCorner = _viewModel.HighlightShapeItems[2];
            Point newPoint = new(110, 110);

            // Act
            _viewModel.BuildShape(newPoint);

            // Assert
            var expectedBoundary = new Rect(0, 0, 110, 110);
            Assert.AreEqual(expectedBoundary, _viewModel._tempShape.boundary);
        }

        [TestMethod]
        public void DeleteShapeTest()
        {
            // Arrange
            Mock<IShapeReceiver> _mockShapeReceiver = new();
            ViewModel _viewModel = new()
            {
                activeTool = "Delete",
                machine = _mockShapeReceiver.Object
            };

            Point start = new(0, 0);
            Point end = new(100, 100);
            ShapeItem shapeToDelete = _viewModel.CreateShape("Rectangle", start, end, Brushes.Yellow, Brushes.Black, 1, "tempShape");
            _viewModel.AddShape(shapeToDelete);
            _viewModel._tempShape = shapeToDelete;

            // Set up the mock behavior
            _mockShapeReceiver.Setup(m => m.OnShapeReceived(It.IsAny<ShapeItem>(), Operation.Deletion));

            // Act
            _viewModel.BuildShape(new Point(0, 0)); // The point can be arbitrary in this case

            // Assert
            Assert.IsFalse(_viewModel.ShapeItems.Contains(shapeToDelete));
            Assert.IsNull(_viewModel._tempShape);
            _mockShapeReceiver.Verify(m => m.OnShapeReceived(shapeToDelete, Operation.Deletion), Times.Once());
        }
    }
}
