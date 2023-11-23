using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Media;
using MessengerDashboard.UI.ViewModels;
using MessengerWhiteboard;
using MessengerWhiteboard.Models;

namespace MessengerTests.WhiteboardTests
{
    [TestClass]
    public class Utils
    {
        public bool Compare(ShapeItem shape1, ShapeItem shape2)
        {
            if (shape1 == null && shape2 == null)
            {
                return true;
            }
            if (shape1 == null || shape2 == null)
            {
                return false;
            }

            return shape1.Id == shape2.Id &&
                shape1.ShapeType == shape2.ShapeType &&
                shape1.Geometry == shape2.Geometry &&
                shape1.StrokeThickness == shape2.StrokeThickness &&
                shape1.ZIndex == shape2.ZIndex &&
                shape1.Fill == shape2.Fill &&
                shape1.Stroke == shape2.Stroke &&
                shape1.color == shape2.color &&
                shape1.points == shape2.points;
        }

        public bool Compare(List<ShapeItem> list1, List<ShapeItem> list2)
        {
            if (list1 == null && list2 == null)
            {
                return true;
            }
            if (list1 == null || list2 == null)
            {
                return false;
            }

            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++)
            {
                if (!Compare(list1[i], list2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CompareBoardServerShapes(WBShape shape1, WBShape shape2)
        {
            Serializer serializer = new();
            if (shape1 == null && shape2 == null)
            {
                return true;
            }

            if (shape1.UserId != shape2.UserId|| shape1.Op != shape2.Op || shape1.SnapshotID!= shape2.SnapshotID)
            {
                return false;
            }

            List<ShapeItem> shapeItems1 = serializer.DeserializeShapes(shape1.ShapeItems);
            List<ShapeItem> shapeItems2 = serializer.DeserializeShapes(shape2.ShapeItems);

            return Compare(shapeItems1, shapeItems2);
        }


        public ShapeItem CreateShape(string shapeType, Point start, Point end, Brush fillBrush, Brush borderBrush, double strokeThickness, string uid, string textData = "Text")
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
                    Brushes.Black,
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
                Id = new(uid),
                points = new List<Point> { start, end },
                TextString = textData,
            };

            return newShape;
            //return new ShapeItem(shapeType, geometry, boundingBox, color, 1, 1);
        }

        public ShapeItem CreateRandomShape()
        {
            Random random = new();
            Dictionary<int, string> shapeTypes = new()
            {
                {0,"RectangleGeometry"},
                {1,"EllipseGeometry" }
            };
            Point start = new(random.Next(0, 100), random.Next(0, 100));
            Point end = new(random.Next(0, 100), random.Next(0, 100));
            return CreateShape(shapeTypes[random.Next(0, 2)], start, end, Brushes.Black, Brushes.Transparent, 1, "hello");
        }

        public List<ShapeItem> GenerateRandomBoardShapes(int n)
        {
            List<ShapeItem> boardShapes = new();

            for (int i = 0; i < n; i++)
            {
                boardShapes.Add(CreateRandomShape());
            }
            return boardShapes;
        }
    }
}
