/***************************
* Filename    = Serializer.cs
*
* Author      = Kaustubh Chavan
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file contains the implementation of the Serializer class,
                which is responsible for serializing and deserializing ShapeItem
                and WBShape objects for the Messenger Whiteboard project.
* 
***************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MessengerWhiteboard.Models;
using Newtonsoft.Json;

namespace MessengerWhiteboard
{
    /// <summary>
    /// The Serializer class provides methods for serializing and deserializing
    /// ShapeItem and WBShape objects.
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Serializes a ShapeItem object into a SerializableShapeItem.
        /// </summary>
        /// <param name="shape">The ShapeItem to be serialized.</param>
        /// <returns>The serialized SerializableShapeItem.</returns>
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

        /// <summary>
        /// Serializes a list of ShapeItem objects into a list of SerializableShapeItems.
        /// </summary>
        /// <param name="shapes">The list of ShapeItems to be serialized.</param>
        /// <returns>The list of serialized SerializableShapeItems.</returns>
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

        /// <summary>
        /// Marshals a list of ShapeItem objects into a JSON string.
        /// </summary>
        /// <param name="shapes">The list of ShapeItems to be marshaled.</param>
        /// <returns>The JSON string representing the marshaled shapes.</returns>
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

        /// <summary>
        /// Unmarshals a JSON string into a list of ShapeItem objects.
        /// </summary>
        /// <param name="jsonString">The JSON string to be unmarshaled.</param>
        /// <returns>The list of unmarshaled ShapeItems.</returns>
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
                Trace.WriteLine("[Whiteboard] Error Occurred: Serializer:DeserializeShapeItems");
                Trace.WriteLine(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Serializes a WBShape object into a JSON string.
        /// </summary>
        /// <param name="shape">The WBShape to be serialized.</param>
        /// <returns>The JSON string representing the serialized WBShape.</returns>
        public string SerializeWBShape(WBShape shape)
        {
            return JsonConvert.SerializeObject(shape);
        }

        /// <summary>
        /// Deserializes a JSON string into a WBShape object.
        /// </summary>
        /// <param name="jsonString">The JSON string to be deserialized.</param>
        /// <returns>The deserialized WBShape object.</returns>
        public WBShape DeserializeWBShape(string jsonString)
        {
            return JsonConvert.DeserializeObject<WBShape>(jsonString);
        }

        /// <summary>
        /// Deserializes a SerializableShapeItem into a ShapeItem.
        /// </summary>
        /// <param name="serializableShape">The SerializableShapeItem to be deserialized.</param>
        /// <returns>The deserialized ShapeItem.</returns>
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

        /// <summary>
        /// Deserializes a list of SerializableShapeItems into a list of ShapeItems.
        /// </summary>
        /// <param name="serializableShapes">The list of SerializableShapeItems to be deserialized.</param>
        /// <returns>The list of deserialized ShapeItems.</returns>
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
