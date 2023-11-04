using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        public ShapeItem CreateShape(string shapeType, Point start, Point end, Color color)
        {
            Rect boundingBox = new(start, end);
            Geometry geometry;
            if (shapeType == "Rectangle")
            {
                geometry = new RectangleGeometry(boundingBox);
            }
            else
            {
                Debug.WriteLine("inside createshape Ellipse");
                geometry = new EllipseGeometry(boundingBox);
            }
            Debug.WriteLine(geometry);
            ShapeItem newShape = new ShapeItem
            {
                ShapeType = shapeType,
                Geometry = geometry,
                boundary = boundingBox,
                color = color,
                StrokeThickness = 1,
                ZIndex = 1,
                Fill = Brushes.Black,
                Stroke = Brushes.Black
            };

            return newShape;
            //return new ShapeItem(shapeType, geometry, boundingBox, color, 1, 1);
        }

        public void StartShape(Point a)
        {
            tempShape = CreateShape(shapeMode, a, a, Colors.Black);
            AddShape(tempShape);
        }

        public void BuildShape(Point a)
        {
            if (tempShape != null)
            {
                tempShape.EditShape(tempShape.boundary.TopLeft, a);
                ShapeItems[ShapeItems.Count - 1] = tempShape;
                Debug.WriteLine(ShapeItems[ShapeItems.Count - 1].Geometry.Bounds);
            }
        }

        public void EndShape(Point a)
        {
            if(tempShape != null)
            {
                //tempShape.EditShape(tempShape.boundary.TopLeft, a);
                //ShapeItems[ShapeItems.Count - 1] = tempShape;
            }
        }



    }
}
