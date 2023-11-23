/***************************
* Filename    = ShapeOperations.cs
*
* Author      = Sanjh Maheshwari
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = It encapsulates managing the creation, selection, transformation, and deletion of
                various shape items. This class facilitates user interactions with the whiteboard,
                offering functionalities like drawing, resizing,and moving shapes, as well as 
                managing shape selections and subsequent highlighting of resize handlers. 
                Overall, this class is crucial for the interactive and functional aspects of the 
                whiteboard, providing a seamless user experience in graphical manipulation and 
                collaboration.
***************************/

using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        /// <summary>
        /// Method for creating a ShapeItem object given following design parameters
        /// </summary>
        /// <param name="shapeType">The type of shape to create (e.g., Rectangle, Ellipse, Curve, Line).</param>
        /// <param name="start">The starting point for the shape's bounding box or line.</param>
        /// <param name="end">The ending point for the shape's bounding box or line.</param>
        /// <param name="fillBrush">The brush used for filling the shape.</param>
        /// <param name="borderBrush">The brush used for the shape's border.</param>
        /// <param name="strokeThickness">The thickness of the shape's stroke/border.</param>
        /// <param name="textData">Optional text data for the shape, with a default value of "Text".</param>
        /// <returns>Returns a new ShapeItem object configured with the specified parameters.</returns>
        public ShapeItem CreateShape(string shapeType, Point start, Point end, Brush fillBrush, Brush borderBrush, double strokeThickness, string textData = "Text")
        {
            // Create the bounding box encompassing the shape
            Rect boundingBox = new(start, end);

            // Create the geometry according to the shape type
            Geometry geometry;
            if (shapeType == "Rectangle")
            {
                geometry = new RectangleGeometry(boundingBox);
            }
            else if (shapeType == "Ellipse")
            {
                geometry = new EllipseGeometry(boundingBox);
            }
            else if (shapeType == "Curve")
            {
                geometry = new PathGeometry();
            }
            else if (shapeType == "Line")
            {
                geometry = new LineGeometry(start, end);
            }
            else
            {
                // Initialize the text in the shape
                FormattedText formattedText = new(
                    textData,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    16,
                    strokeBrush,
                    1.0
                    );

                geometry = formattedText.BuildGeometry(start);
            }
            
            // Initialize the ShapeItem with new Guid and other params 
            ShapeItem newShape = new()
            {
                ShapeType = shapeType,
                Geometry = geometry,
                boundary = boundingBox,
                StrokeThickness = strokeThickness,
                ZIndex = 1,
                Fill = fillBrush,
                Stroke = borderBrush,
                Id = Guid.NewGuid(),
                points = new List<Point> { start, end },
                TextString = textData,
            };

            return newShape;
        }

        /// <summary>
        /// Method to start creation of new ShapeItem
        /// </summary>
        /// <param name="a">The current position of cursor</param>
        public void StartShape(Point a)
        {            
            // Unselect any previous shape
            UnselectShape();

            // Start creation of temporory shape
            _tempShape = CreateShape(activeTool, a, a, fillBrush, strokeBrush, StrokeThickness);
            
            // Add shape to the ShapeItems list
            AddShape(_tempShape);
        }

        /// <summary>
        /// Method to select the shape given the unique Guid
        /// </summary>
        /// <param name="uid">The unique identifier (Guid) of the shape to be selected. Can be null.</param>
        public void SelectShape(string? uid)
        {
          
            if (uid == null)
            {
                return;
            }

            // First check if the uid belongs to highlighted shapes 
            foreach (ShapeItem shape in HighlightShapeItems)
            {
                if (shape.Id.ToString() == uid)
                {
                    // Handle resize 
                    if(shape.ShapeType == "Curve")
                    {
                        return;
                    }
                    // Attach the shape to the _selectedCorner variable and return
                    _selectedCorner = shape;
                    return;
                }
            }


            // If there is some shape already selected 
            if (_tempShape != null)
            {
                // Condition when the selected shape is same as previously selected 
                if (_tempShape.Id.ToString() == uid)
                {
                    // We do nothing
                    return;
                }
                // Condition when the selected shape is new we unselect the previous and select to new one 
                else
                {
                    UnselectShape();
                    foreach (ShapeItem shape in ShapeItems)
                    {
                        if (shape.Id.ToString() == uid)
                        {
                            _tempShape = shape;
                            if(shape.ShapeType == "Curve")
                            {
                                break;
                            }
                            HighlightShape(_tempShape);
                            break;
                        }
                    }
                }

            }
            // Login for handling selection of new shape
            else
            {
                foreach (ShapeItem shape in ShapeItems)
                {
                    if (shape.Id.ToString() == uid)
                    {
                        _tempShape = shape;
                        if (shape.ShapeType == "Curve")
                        {
                            break;
                        }
                        HighlightShape(_tempShape);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Method to unselect the currently selected shape
        /// </summary>
        public void UnselectShape()
        {
            if (_tempShape != null)
            {
                // Unhighlight the shape and set _tempShape to null value
                UnhighlightShape(_tempShape);
                _tempShape = null;
            }
        }

        /// <summary>
        /// Method to perform different transformations to the shape like move, resize and delete
        /// </summary>
        /// <param name="a">Current position of the cursor</param>
        public void BuildShape(Point a)
        {
            if (_tempShape != null)
            {
                if (activeTool == "Select")
                {
                    // Unhighlighting to avoid uneccessary updation of corners
                    UnhighlightShape(_tempShape);
                    
                    // Check if mouse down was called previously and it was a corner or not
                    if (lastDownPoint != null && _selectedCorner == null)
                    {
                       // Get the last mouse down point
                        Point x = lastDownPoint.Value;
                        double dX = a.X - x.X;
                        double dY = a.Y - x.Y;

                        // Move Shape as no corner was selected
                        _tempShape.MoveShape(new Point(_tempShape.boundary.TopLeft.X + dX, _tempShape.boundary.TopLeft.Y + dY),
                               new Point(_tempShape.boundary.BottomRight.X + dX, _tempShape.boundary.BottomRight.Y + dY));
                        lastDownPoint = a;
                    }
                    else if (lastDownPoint != null && _selectedCorner != null)
                    {
                        // Get the last mouse down point
                        Point x = lastDownPoint.Value;
                        double dX = a.X - x.X;
                        double dY = a.Y - x.Y;

                        Point newTopLeft;
                        Point newBottomRight;

                        // Resize Shape as corner was selected by checking which corner was selected
                        if (_selectedCorner.TextString == "hsBottomRight")
                        {
                            newTopLeft = new(_tempShape.boundary.TopLeft.X, _tempShape.boundary.TopLeft.Y);
                            newBottomRight = new(_tempShape.boundary.BottomRight.X + dX, _tempShape.boundary.BottomRight.Y + dY);
                            _tempShape.ResizeShape(newTopLeft, newBottomRight);
                        }
                        else if (_selectedCorner.TextString == "hsTopLeft")
                        {
                            newTopLeft = new(_tempShape.boundary.TopLeft.X + dX, _tempShape.boundary.TopLeft.Y + dY);
                            newBottomRight = new(_tempShape.boundary.BottomRight.X, _tempShape.boundary.BottomRight.Y);
                            _tempShape.ResizeShape(newTopLeft, newBottomRight);
                        }

                        lastDownPoint = a;
                    }
                }

                // Handle delete operation by removing from ShapeItems nad setting _tempShape to null
                else if (activeTool == "Delete")
                {
                    if (_tempShape != null)
                    {
                        machine.OnShapeReceived(_tempShape, Operation.Deletion);
                        ShapeItems.Remove(_tempShape);
                        _tempShape = null;
                    }
                }

                // Handle mouse move during creation of shape by editing the shape
                else
                {
                    _tempShape.EditShape(_tempShape.boundary.TopLeft, a);
                    ShapeItems[ShapeItems.Count - 1] = _tempShape;
                }
            }
        }

        /// <summary>
        /// Method to end creation or transformation of ShapeItem
        /// </summary>
        /// <param name="a">Current position of the cursor</param>
        public void EndShape(Point a)
        {
            if (_tempShape != null)
            {
                // Check if we are in CreateMode or SelecteMode
                if (currentMode == WBModes.CreateMode)
                {
                    // Server is notified of the creation process and UndoStack is updated
                    Trace.WriteLine("EndShape: ", _tempShape.ShapeType);
                    machine.OnShapeReceived(_tempShape, Operation.Creation);
                    UndoStackElement undoStackElement = new(_tempShape, _tempShape, Operation.Creation);
                    InsertIntoStack(undoStackElement);
                    _tempShape = null;
                }
                else if (currentMode == WBModes.SelectMode)
                {
                    // Server is notified of the modification process and UndoStack is updated
                    Trace.WriteLine("EndShape: ", _tempShape.ShapeType);
                    machine.OnShapeReceived(_tempShape, Operation.ModifyShape);
                    UndoStackElement undoStackElement = new(_tempShape, _tempShape, Operation.ModifyShape);
                    InsertIntoStack(undoStackElement);
                    HighlightShape(_tempShape);
                    _selectedCorner = null;
                }
                //tempShape.EditShape(tempShape.boundary.TopLeft, a);
                //ShapeItems[ShapeItems.Count - 1] = tempShape;
            }
        }

        /// <summary>
        /// Method to generate a rectangular bouding box which are used as visual handles for resize
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="fill"></param>
        /// <param name="stroke"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShapeItem GenerateRectangleBB(System.Windows.Point start, System.Windows.Point end, Brush fill, Brush stroke, string name)
        {
            ShapeItem s = CreateShape("Rectangle", start, end, fill, stroke, 1, name);
            return s;
        }

        /// <summary>
        /// Method to highlight given the ShapeItem
        /// </summary>
        /// <param name="s">ShapeItem to be highlighted</param>
        public void HighlightShape(ShapeItem s)
        {
            // Check if shape is already highlighted 
            if (HighlightShapeItems.Count > 0)
            {
                return;
            }

            double x = s.boundary.X;
            double y = s.boundary.Y;
            double height = s.boundary.Height;
            double width = s.boundary.Width;
            int blobSize = 5;

            // Create the corners according to coordinates of the given ShapeItem and blobsize
            
            // highlight body
            Point start = new(x, y);
            Point end = new(x + width, y + height);
            ShapeItem hsBody = GenerateRectangleBB(start, end, null, Brushes.Yellow, "hsBody");

            // highlight top left 
            start = new(x - blobSize / 2, y - blobSize / 2);
            end = new(x + blobSize, y + blobSize);
            ShapeItem hsTopLeft = GenerateRectangleBB(start, end, Brushes.Black, Brushes.Yellow, "hsTopLeft");

            // highlight bottom right
            start = new(x + width - blobSize / 2, y + height - blobSize / 2);
            end = new(x + width + blobSize, y + height + blobSize);
            ShapeItem hsBottomRight = GenerateRectangleBB(start, end, Brushes.Black, Brushes.Yellow, "hsBottomRight");

            // Add to list containing all the highlighted corners
            HighlightShapeItems.Add(hsBody);
            HighlightShapeItems.Add(hsTopLeft);
            HighlightShapeItems.Add(hsBottomRight);

            // Add to ShapeItems list for rendering
            foreach (ShapeItem hs in HighlightShapeItems)
            {
                AddShape(hs);
            }
        }

        /// <summary>
        /// Method to unhighlight a shape
        /// </summary>
        /// <param name="s">ShapeItem to be unhighlighted</param>
        public void UnhighlightShape(ShapeItem s)
        {
            // Remove all the highlighted corners 
            foreach (ShapeItem hs in HighlightShapeItems)
            {
                RemoveShape(hs);
            }

            // Clear the highlighted shape list
            HighlightShapeItems.Clear();
        }
    }
}
