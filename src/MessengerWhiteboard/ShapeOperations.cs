using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        public ShapeItem CreateShape(string shapeType, Point start, Point end, Brush fillBrush, Brush borderBrush, double strokeThickness, string textData = "Text")
        {
            Rect boundingBox = new(start, end);
            Geometry geometry;
            if (shapeType == "Rectangle")
            {
                geometry = new RectangleGeometry(boundingBox);
            }
            else if (shapeType == "Ellipse")
            {
                //Debug.WriteLine("inside createshape Ellipse");
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
            //Debug.WriteLine(geometry);
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
            //return new ShapeItem(shapeType, geometry, boundingBox, color, 1, 1);
        }

        public void StartShape(Point a)
        {
            //Debug.WriteLine((fillBrush as SolidColorBrush).Color.ToString());
            UnselectShape();
            _tempShape = CreateShape(activeTool, a, a, fillBrush, strokeBrush, StrokeThickness);
            AddShape(_tempShape);
        }

        public void SelectShape(string? uid)
        {
            if (uid == null)
            {
                return;
            }
            // First check if the uid belongs to highlighted shapes 
            foreach (ShapeItem shape in HighlightShapeItems)
            {
                //Debug.WriteLine(shape.Id.ToString());
                if (shape.Id.ToString() == uid)
                {
                    // Handle resize 
                    _selectedCorner = shape;
                    return;
                }
            }


            // Condition when the selected shape is same as previously selected 
            if (_tempShape != null)
            {
                if (_tempShape.Id.ToString() == uid)
                {
                    // UnselectShape();
                    return;
                }
                else
                {
                    UnselectShape();
                    foreach (ShapeItem shape in ShapeItems)
                    {
                        //Debug.WriteLine(shape.Id.ToString());
                        if (shape.Id.ToString() == uid)
                        {
                            _tempShape = shape;
                            HighlightShape(_tempShape);
                            break;
                        }
                    }
                }

            }
            else
            {
                foreach (ShapeItem shape in ShapeItems)
                {
                    //Debug.WriteLine(shape.Id.ToString());
                    if (shape.Id.ToString() == uid)
                    {
                        _tempShape = shape;
                        HighlightShape(_tempShape);
                        break;
                    }
                }
            }
        }

        public void UnselectShape()
        {
            if (_tempShape != null)
            {
                UnhighlightShape(_tempShape);
                _tempShape = null;
            }
        }

        public void BuildShape(Point a)
        {
            if (_tempShape != null)
            {
                //Debug.WriteLine(activeTool);
                if (activeTool == "Select")
                {
                    // Unhighlighting to avoid uneccessary updation of corners
                    UnhighlightShape(_tempShape);
                    if (lastDownPoint != null && _selectedCorner == null)
                    {
                        Point x = lastDownPoint.Value;
                        double dX = a.X - x.X;
                        double dY = a.Y - x.Y;
                        _tempShape.MoveShape(new Point(_tempShape.boundary.TopLeft.X + dX, _tempShape.boundary.TopLeft.Y + dY),
                               new Point(_tempShape.boundary.BottomRight.X + dX, _tempShape.boundary.BottomRight.Y + dY));
                        lastDownPoint = a;
                    }
                    else if (lastDownPoint != null && _selectedCorner != null)
                    {
                        Point x = lastDownPoint.Value;
                        double dX = a.X - x.X;
                        double dY = a.Y - x.Y;

                        Point newTopLeft;
                        Point newBottomRight;

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
                else if (activeTool == "Delete")
                {
                    if (_tempShape != null)
                    {
                        machine.OnShapeReceived(_tempShape, Operation.Deletion);
                        ShapeItems.Remove(_tempShape);
                        _tempShape = null;
                    }
                }
                else
                {
                    _tempShape.EditShape(_tempShape.boundary.TopLeft, a);
                    ShapeItems[ShapeItems.Count - 1] = _tempShape;
                    //Debug.WriteLine(ShapeItems[ShapeItems.Count - 1].Geometry.Bounds);
                }
            }
        }

        public void EndShape(Point a)
        {
            if (_tempShape != null)
            {
                if (currentMode == WBModes.CreateMode)
                {
                    Trace.WriteLine("EndShape: ", _tempShape.ShapeType);
                    machine.OnShapeReceived(_tempShape, Operation.Creation);
                    UndoStackElement undoStackElement = new(_tempShape, _tempShape, Operation.Creation);
                    InsertIntoStack(undoStackElement);
                    _tempShape = null;
                }
                else if (currentMode == WBModes.SelectMode)
                {
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

        public ShapeItem GenerateRectangleBB(System.Windows.Point start, System.Windows.Point end, Brush fill, Brush stroke, string name)
        {
            ShapeItem s = CreateShape("Rectangle", start, end, fill, stroke, 1, name);
            return s;
        }

        public void HighlightShape(ShapeItem s)
        {
            if (HighlightShapeItems.Count > 0)
            {
                return;
            }

            double x = s.boundary.X;
            double y = s.boundary.Y;
            double height = s.boundary.Height;
            double width = s.boundary.Width;
            int blobSize = 5;

            Point start = new(x, y);
            Point end = new(x + width, y + height);
            ShapeItem hsBody = GenerateRectangleBB(start, end, null, Brushes.Yellow, "hsBody");

            start = new(x - blobSize / 2, y - blobSize / 2);
            end = new(x + blobSize, y + blobSize);
            ShapeItem hsTopLeft = GenerateRectangleBB(start, end, Brushes.Black, Brushes.Yellow, "hsTopLeft");

            //start = new(x + width - blobSize / 2, y - blobSize / 2);
            //end = new(x + width + blobSize, y + blobSize);
            //ShapeItem hsTopRight = GenerateRectangleBB(start, end, Brushes.Black, Brushes.Yellow);

            //start = new(x - blobSize / 2, y + height - blobSize / 2);
            //end = new(x + blobSize, y + height + blobSize);
            //ShapeItem hsBottomLeft = GenerateRectangleBB(start, end, Brushes.Black, Brushes.Yellow);

            start = new(x + width - blobSize / 2, y + height - blobSize / 2);
            end = new(x + width + blobSize, y + height + blobSize);
            ShapeItem hsBottomRight = GenerateRectangleBB(start, end, Brushes.Black, Brushes.Yellow, "hsBottomRight");

            HighlightShapeItems.Add(hsBody);
            HighlightShapeItems.Add(hsTopLeft);
            // HighlightShapeItems.Add(hsTopRight);
            // HighlightShapeItems.Add(hsBottomLeft);
            HighlightShapeItems.Add(hsBottomRight);

            foreach (ShapeItem hs in HighlightShapeItems)
            {
                AddShape(hs);
            }
        }

        public void UnhighlightShape(ShapeItem s)
        {
            foreach (ShapeItem hs in HighlightShapeItems)
            {
                RemoveShape(hs);
            }

            HighlightShapeItems.Clear();
        }
    }
}
