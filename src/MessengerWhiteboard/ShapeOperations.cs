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
            _tempShape = CreateShape(activeTool, a, a, fillBrush, strokeBrush, StrokeThickness);
            AddShape(_tempShape);
        }

        public void SelectShape(string uid)
        {
            foreach (ShapeItem shape in ShapeItems)
            {
                //Debug.WriteLine(shape.Id.ToString());
                if (shape.Id.ToString() == uid)
                {
                    _tempShape = shape;
                }
            }
        }

        public void BuildShape(Point a)
        {
            if (_tempShape != null)
            {
                //Debug.WriteLine(activeTool);
                if (activeTool == "Select")
                {
                    if (lastDownPoint != null)
                    {
                        Point x = lastDownPoint.Value;
                        double dX = a.X - x.X;
                        double dY = a.Y - x.Y;
                        _tempShape.MoveShape(new Point(_tempShape.boundary.TopLeft.X + dX, _tempShape.boundary.TopLeft.Y + dY),
                               new Point(_tempShape.boundary.BottomRight.X + dX, _tempShape.boundary.BottomRight.Y + dY));
                        //Debug.WriteLine(_tempShape.boundary);
                        lastDownPoint = a;
                    }
                    machine.OnShapeReceived(_tempShape, Operation.ModifyShape);

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
                Trace.WriteLine("EndShape: ", _tempShape.ShapeType);
                machine.OnShapeReceived(_tempShape, Operation.Creation);
                UndoStackElement undoStackElement = new(_tempShape, _tempShape, Operation.Creation);
                InsertIntoStack(undoStackElement);
                //tempShape.EditShape(tempShape.boundary.TopLeft, a);
                //ShapeItems[ShapeItems.Count - 1] = tempShape;
            }
            _tempShape = null;
        }



    }
}
