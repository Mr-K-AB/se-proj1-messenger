using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MessengerWhiteboard
{
    public class Serializer
    {
        public SerializableShapeItem SerializeShape(ShapeItem shape)
        {
            SerializableShapeItem serializableShape = new SerializableShapeItem
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

        public ShapeItem DeserializeShape(SerializableShapeItem serializableShape)
        {
            ShapeItem shape = new ShapeItem
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
    }
}
