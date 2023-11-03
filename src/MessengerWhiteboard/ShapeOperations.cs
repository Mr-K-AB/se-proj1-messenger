using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

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
                geometry = new EllipseGeometry(boundingBox);
            }
            return new ShapeItem(shapeType, geometry, boundingBox, color, 1, 1);
        }

        public void StartShape(Point a)
        {
            tempShape = CreateShape("Rectangle", a, a, Colors.Black);
        }

        public void EndShape(Point a)
        {
            if(tempShape != null)
            {
                tempShape.EditShape(tempShape.boundary.TopLeft, a);
                AddShape(tempShape);
            }
        }



    }
}
