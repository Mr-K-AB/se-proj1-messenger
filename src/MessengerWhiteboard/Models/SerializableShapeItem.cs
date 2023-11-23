/***************************
* Filename    = SerializableShapeItem.cs
*
* Author      = Kaustubh Chavan
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file defines the SerializableShapeItem class, representing a serializable
                version of a ShapeItem. It includes properties to store shape-related information
                for serialization and deserialization purposes.
* 
***************************/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MessengerWhiteboard.Models
{
    /// <summary>
    /// The SerializableShapeItem class represents a serializable version of a ShapeItem.
    /// </summary>
    public class SerializableShapeItem
    {
        /// <summary>
        /// Gets or sets the type of the shape.
        /// </summary>
        public string shapeType { get; set; }

        /// <summary>
        /// Gets or sets the serialized string representation of the shape's geometry.
        /// </summary>
        public string GeometryString { get; set; }

        /// <summary>
        /// Gets or sets the fill brush of the shape.
        /// </summary>
        public Brush Fill { get; set; }

        /// <summary>
        /// Gets or sets the stroke brush of the shape.
        /// </summary>
        public Brush Stroke { get; set; }

        /// <summary>
        /// Gets or sets the bounding box of the shape.
        /// </summary>
        public Rect boundary { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the shape.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the color of the shape.
        /// </summary>
        public Color color { get; set; }

        /// <summary>
        /// Gets or sets the Z-index of the shape.
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness of the shape.
        /// </summary>
        public double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the list of points of the shape.
        /// </summary>
        public List<Point> points { get; set; }
    }
}
