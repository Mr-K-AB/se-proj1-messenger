using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        public ShapeItem CreateShape(string shapeType, Point start, Point end, Brush brush)
        {
            Rect boundingBox = new(start, end);
            Geometry geometry;
            if (shapeType == "Rectangle")
            {
                geometry = new RectangleGeometry(boundingBox);
            }
            else
            {
                //Debug.WriteLine("inside createshape Ellipse");
                geometry = new EllipseGeometry(boundingBox);
            }
            //Debug.WriteLine(geometry);
            ShapeItem newShape = new()
            {
                ShapeType = shapeType,
                Geometry = geometry,
                boundary = boundingBox,
                StrokeThickness = 1,
                ZIndex = 1,
                Fill = brush,
                Stroke = Brushes.Black,
                Id = Guid.NewGuid()
            };

            return newShape;
            //return new ShapeItem(shapeType, geometry, boundingBox, color, 1, 1);
        }

        public void StartShape(Point a)
        {
            //Debug.WriteLine((fillBrush as SolidColorBrush).Color.ToString());
            _tempShape = CreateShape(activeTool, a, a, fillBrush);
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
                        Debug.WriteLine(_tempShape.boundary);
                        lastDownPoint = a;
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
                //tempShape.EditShape(tempShape.boundary.TopLeft, a);
                //ShapeItems[ShapeItems.Count - 1] = tempShape;
            }
        }



    }
}
