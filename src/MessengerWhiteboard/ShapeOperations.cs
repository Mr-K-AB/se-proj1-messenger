using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace MessengerWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        public ShapeItem CreateShape(string shapeType, Point start, Point end, Color color)
        {
            Rect boundingBox = new(start, end);
            Geometry geometry = new RectangleGeometry(boundingBox);
            return new ShapeItem(shapeType, geometry, boundingBox, color, 1, 1);
        }

    }
}
