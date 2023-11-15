using System.Windows;
using System.Windows.Media;

namespace MessengerWhiteboard
{
    public class SerializableShapeItem
    {
        public string shapeType { get; set; }
        public string GeometryString { get; set; }
        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        public Rect boundary { get; set; }

        public Guid Id { get; set; }

        public Color color { get; set; }
        public int ZIndex { get; set; }
        public int StrokeThickness { get; set; }

        public List<Point> points{ get; set;}
    }
}
