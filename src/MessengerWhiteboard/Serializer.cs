using System.Diagnostics;
using System.Windows.Media;
using MessengerWhiteboard.Models;
using Newtonsoft.Json;

namespace MessengerWhiteboard
{
    public class Serializer
    {
        public SerializableShapeItem SerializeShape(ShapeItem shape)
        {
            if (shape == null)
            {
                return null;
            }
            SerializableShapeItem serializableShape = new()
            {
                shapeType = shape.ShapeType,
                GeometryString = shape.Geometry.ToString(),
                Fill = shape.Fill,
                Stroke = shape.Stroke,
                StrokeThickness = shape.StrokeThickness,
                boundary = shape.boundary,
                Id = shape.Id,
                color = shape.color,
                ZIndex = shape.ZIndex,
                points = shape.points
            };
            return serializableShape;
        }

        public List<SerializableShapeItem> SerializeShapes(List<ShapeItem> shapes)
        {
            if (shapes == null)
            {
                return null;
            }
            List<SerializableShapeItem> serializableShapes = new();
            foreach (ShapeItem shape in shapes)
            {
                serializableShapes.Add(SerializeShape(shape));
            }
            return serializableShapes;
        }

        public string MarhsalShapes(List<ShapeItem> shapes)
        {
            if (shapes == null)
            {
                return null;
            }
            List<SerializableShapeItem> serializableShapes = new();
            foreach (ShapeItem shape in shapes)
            {
                serializableShapes.Add(SerializeShape(shape));
            }
            return JsonConvert.SerializeObject(serializableShapes);
        }

        public List<ShapeItem> UnMarshalShapes(string jsonString)
        {
            try
            {
                List<SerializableShapeItem>? boardShapes = JsonConvert.DeserializeObject<List<SerializableShapeItem>>(
                    jsonString
                );
                if (boardShapes == null)
                {
                    return null;
                }
                List<ShapeItem> result = DeserializeShapes(boardShapes);
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Whiteboard] Error Occured: Serializer:DeserializeShapeItems");
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        public string SerializeWBShape(WBShape shape)
        {
            return JsonConvert.SerializeObject(shape);
        }

        public WBShape DeserializeWBShape(string jsonString)
        {
            return JsonConvert.DeserializeObject<WBShape>(jsonString);
        }

        public ShapeItem DeserializeShape(SerializableShapeItem serializableShape)
        {
            if (serializableShape == null)
            {
                return null;
            }
            Geometry g = new RectangleGeometry(serializableShape.boundary);
            if (serializableShape.shapeType == "Ellipse")
            {
                g = new EllipseGeometry(serializableShape.boundary);
            }
            else if (serializableShape.shapeType == "Line")
            {
                g = new LineGeometry(serializableShape.points[0], serializableShape.points[1]);
            }
            else if (serializableShape.shapeType == "Curve")
            {
                //PathGeometry pathGeometry = new();
                //pathGeometry.AddGeometry(new LineGeometry(serializableShape.points[0], serializableShape.points[1]));
                //for(int i = 1; i < serializableShape.points.Count - 1; i++)
                //{
                //    pathGeometry.AddGeometry(new LineGeometry(serializableShape.points[i], serializableShape.points[i + 1]));
                //}
                //g = pathGeometry;
                g = Geometry.Parse(serializableShape.GeometryString);
            }
            ShapeItem shape = new()
            {
                ShapeType = serializableShape.shapeType,
                Geometry = g,
                Fill = serializableShape.Fill,
                Stroke = serializableShape.Stroke,
                StrokeThickness = serializableShape.StrokeThickness,
                boundary = serializableShape.boundary,
                Id = serializableShape.Id,
                color = serializableShape.color,
                ZIndex = serializableShape.ZIndex,
                points = serializableShape.points
            };
            Debug.Print($"Client Geometry: {shape.Geometry}");
            return shape;
        }

        public List<ShapeItem> DeserializeShapes(List<SerializableShapeItem> serializableShapes)
        {
            if (serializableShapes == null)
            {
                return null;
            }
            List<ShapeItem> shapes = new();
            foreach (SerializableShapeItem serializableShape in serializableShapes)
            {
                shapes.Add(DeserializeShape(serializableShape));
            }
            return shapes;
        }
    }
}
