using System.Windows;
using System.Windows.Media;


namespace MessengerWhiteboard.Models
{
    public class ShapeItem
    {
        public string ShapeType { get; set; }
        public Geometry Geometry { get; set; }
        public double StrokeThickness { get; set; }
        public int ZIndex { get; set; }

        public Brush Fill { get; set; }
        public Brush Stroke { get; set; }
        // public double Height { get; set; }
        // public double Width { get; set; }
        public Color color { get; set; }
        public Rect boundary { get; set; }
        public Guid Id { get; set; }
        public List<Point> points { get; set; }
        public string TextString { get; set; }


        //public ShapeItem(string shapeType, Geometry geometry, Rect bb, Color c, double strokeThickness, int zIndex)
        //{
        //    ShapeType = shapeType;
        //    Geometry = geometry;
        //    //Stroke = stroke;
        //    //Fill = fill;
        //    //Height = height;
        //    //Width = width;
        //    boundary = bb;
        //    color = c;
        //    StrokeThickness = strokeThickness;
        //    ZIndex = zIndex;
        //}

        public void EditShape(Point a, Point b)
        {
            //Debug.WriteLine(ShapeType);
            if (ShapeType == "Rectangle")
            {
                Rect boundingBox = new(a, b);
                Geometry.SetValue(RectangleGeometry.RectProperty, boundingBox);
                //Geometry geometry = new RectangleGeometry(boundingBox);
                boundary = boundingBox;
            }
            else if (ShapeType == "Ellipse")
            {
                Rect boundingBox = new(a, b);
                //Geometry.SetValue(EllipseGeometry., boundingBox);
                Geometry geometry = new EllipseGeometry(boundingBox);
                boundary = boundingBox;
                Geometry = geometry;
            }
            else if (ShapeType == "Curve")
            {
                (Geometry as PathGeometry).AddGeometry(new LineGeometry(points[^1], b));
                points.Add(b);
            }
            else if (ShapeType == "Line")
            {
                Geometry = new LineGeometry(a, b);
                points[^1] = b;
            }
        }

        public void MoveShape(Point a, Point b)
        {
            //Rect boundingBox = new(a, b);
            //double dX = boundingBox.X - boundary.X;
            //double dY = boundingBox.Y - boundary.Y;
            //if (Geometry.Transform is TranslateTransform)
            //{
            //    dX = (Geometry.Transform as TranslateTransform).X + boundingBox.X - boundary.X;
            //    dY = (Geometry.Transform as TranslateTransform).Y + boundingBox.Y - boundary.Y;
            //}
            //Geometry.Transform = new TranslateTransform(dX, dY);
            //// Geometry.Transform.SetValue(TranslateTransform.XProperty, dX);
            //// Geometry.Transform.SetValue(TranslateTransform.YProperty, dY);
            //boundary = boundingBox;

            if (ShapeType == "Rectangle" || ShapeType == "Ellipse")
            {
                Rect newBoundingBox = new(a, b);
                if (ShapeType == "Rectangle")
                {
                    Geometry.SetValue(RectangleGeometry.RectProperty, newBoundingBox);
                    boundary = newBoundingBox;
                }
                else if (ShapeType == "Ellipse")
                {
                    //Geometry geometry = new EllipseGeometry(newBoundingBox);
                    //Geometry = geometry;
                    //boundary = newBoundingBox;

                    Point center = new(newBoundingBox.X + newBoundingBox.Width / 2, newBoundingBox.Y + newBoundingBox.Height / 2);
                    double radiusX = newBoundingBox.Width / 2;
                    double radiusY = newBoundingBox.Height / 2;

                    Geometry.SetValue(EllipseGeometry.CenterProperty, center);
                    Geometry.SetValue(EllipseGeometry.RadiusXProperty, radiusX);
                    Geometry.SetValue(EllipseGeometry.RadiusYProperty, radiusY);

                    boundary = newBoundingBox;
                }
            }
        }

        public void ResizeShape(Point newTopLeft, Point newBottomRight)
        {
            if (ShapeType == "Rectangle" || ShapeType == "Ellipse")
            {
                Rect newBoundingBox = new(newTopLeft, newBottomRight);
                if (ShapeType == "Rectangle")
                {
                    Geometry.SetValue(RectangleGeometry.RectProperty, newBoundingBox);
                    boundary = newBoundingBox;
                }
                else if (ShapeType == "Ellipse")
                {
                    //Geometry geometry = new EllipseGeometry(newBoundingBox);
                    //Geometry = geometry;
                    //boundary = newBoundingBox;

                    Point center = new(newBoundingBox.X + newBoundingBox.Width / 2, newBoundingBox.Y + newBoundingBox.Height / 2);
                    double radiusX = newBoundingBox.Width / 2;
                    double radiusY = newBoundingBox.Height / 2;

                    Geometry.SetValue(EllipseGeometry.CenterProperty, center);
                    Geometry.SetValue(EllipseGeometry.RadiusXProperty, radiusX);
                    Geometry.SetValue(EllipseGeometry.RadiusYProperty, radiusY);

                    boundary = newBoundingBox;
                }
            }
        }

        public override string ToString()
        {
            return $"{ShapeType}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is ShapeItem)
            {
                ShapeItem shape = obj as ShapeItem;
                return Id == shape.Id;
            }
            return false;
        }

        public ShapeItem Clone()
        {
            ShapeItem newShape = new()
            {
                ShapeType = ShapeType,
                Geometry = Geometry.Clone(),
                boundary = boundary,
                StrokeThickness = StrokeThickness,
                ZIndex = ZIndex,
                Fill = Fill,
                Stroke = Stroke,
                Id = Id,
                points = points,
                TextString = TextString,
            };
            return newShape;
        }
    }
}
