using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWhiteboard
{
    internal class ShapeItem
    {
        public string ShapeType { get; set; }
        public string Stroke { get; set; }
        public string Fill { get; set; }
        public double StrokeThickness { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public ShapeItem()
        {
            ShapeType = "Line";
            Stroke = "Black";
            Fill = "Black";
            StrokeThickness = 1;
            Height = 1;
            Width = 1;
            X1 = 0;
            Y1 = 0;
            X2 = 0;
            Y2 = 0;
        }
        public ShapeItem(string shapeType, string stroke, string fill, double strokeThickness, double height, double width, double x1, double y1, double x2, double y2)
        {
            ShapeType = shapeType;
            Stroke = stroke;
            Fill = fill;
            StrokeThickness = strokeThickness;
            Height = height;
            Width = width;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }
        public override string ToString()
        {
            return $"{ShapeType} {Stroke} {Fill} {StrokeThickness} {Height} {Width} {X1} {Y1} {X2} {Y2}";
        }
    }
}
