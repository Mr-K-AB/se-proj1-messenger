using System.Windows;
using System.Windows.Media;


namespace MessengerWhiteboard.Models
{
    /// <summary>
    /// Represents a drawable shape item with various properties like type, geometry, and appearance.
    /// </summary>
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

        /// <summary>
        /// Edits the shape based on two points. The behavior depends on the shape type.
        /// </summary>
        /// <param name="a">The starting point (topleft) for editing the shape.</param>
        /// <param name="b">The ending point (bottomright) for editing the shape.</param>
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

        /// <summary>
        /// Moves the shape to a new location defined by two points.
        /// </summary>
        /// <param name="a">The new top-left point of the shape's bounding box.</param>
        /// <param name="b">The new bottom-right point of the shape's bounding box.</param>
        /// <remarks>
        /// This method is applicable for Rectangle and Ellipse shapes, updating their position.
        /// </remarks>
        public void MoveShape(Point a, Point b)
        {

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

        /// <summary>
        /// Resizes the shape based on new top-left and bottom-right points.
        /// </summary>
        /// <param name="newTopLeft">The new top-left point of the shape's bounding box.</param>
        /// <param name="newBottomRight">The new bottom-right point of the shape's bounding box.</param>
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

        /// <summary>
        /// Provides a string representation of the shape item.
        /// </summary>
        /// <returns>A string that represents the shape type.</returns>
        public override string ToString()
        {
            return $"{ShapeType}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current shape item.
        /// </summary>
        /// <param name="obj">The object to compare with the current shape item based on Guid.</param>
        /// <returns>true if the specified object is equal to the current shape item; otherwise, false.</returns>
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

        /// <summary>
        /// Creates a new ShapeItem that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
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
