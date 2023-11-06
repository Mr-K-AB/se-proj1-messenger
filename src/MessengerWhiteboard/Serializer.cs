using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;

namespace MessengerWhiteboard
{
    public class Serializer
    {
        public SerializableShapeItem SerializeShape(ShapeItem shape)
        {
            SerializableShapeItem serializableShape = new()
            {
                GeometryString = shape.Geometry.ToString(),
                Fill = shape.Fill,
                Stroke = shape.Stroke,
                boundary = shape.boundary,
                Id = shape.Id,
                color = shape.color,
                ZIndex = shape.ZIndex
            };
            return serializableShape;
        }

        public List<SerializableShapeItem> SerializeShapes(List<ShapeItem> shapes)
        {
            List<SerializableShapeItem> serializableShapes = new();
            foreach (ShapeItem shape in shapes)
            {
                serializableShapes.Add(SerializeShape(shape));
            }
            return serializableShapes;
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
            ShapeItem shape = new()
            {
                Geometry = Geometry.Parse(serializableShape.GeometryString),
                Fill = serializableShape.Fill,
                Stroke = serializableShape.Stroke,
                boundary = serializableShape.boundary,
                Id = serializableShape.Id,
                color = serializableShape.color,
                ZIndex = serializableShape.ZIndex
            };
            return shape;
        }

        public List<ShapeItem> DeserializeShapes(List<SerializableShapeItem> serializableShapes)
        {
            List<ShapeItem> shapes = new();
            foreach (SerializableShapeItem serializableShape in serializableShapes)
            {
                shapes.Add(DeserializeShape(serializableShape));
            }
            return shapes;
        }
    }
}
