﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace MessengerWhiteboard
{
    public class ShapeItem
    {
        public string ShapeType { get; set; }
        public Geometry Geometry { get; set; }
        public string Stroke { get; set; }
        public string Fill { get; set; } 
        public double StrokeThickness { get; set; }
        public int ZIndex { get; set; }
        // public double Height { get; set; }
        // public double Width { get; set; }
        public Color color { get; set; }
        public Rect boundary { get; set; }
        public ShapeItem(string shapeType, Geometry geometry, Rect bb, Color c, double strokeThickness, int zIndex)
        {
            ShapeType = shapeType;
            Geometry = geometry;
            Stroke = stroke;
            Fill = fill;
            //Height = height;
            //Width = width;
            boundary = bb;
            color = c;
            StrokeThickness = strokeThickness;
            ZIndex = zIndex;
        }

        public void EditShape(Point a, Point b)
        {
            if(ShapeType == "Rectangle")
            {
                Rect boundingBox = new(a, b);
                Geometry.SetValue(RectangleGeometry.RectProperty, boundingBox);
                //Geometry geometry = new RectangleGeometry(boundingBox);
                boundary = boundingBox;
            }
            else
            {
                Rect boundingBox = new(a, b);
                Geometry geometry = new EllipseGeometry(boundingBox);
                boundary = boundingBox;
                Geometry = geometry;
            }
        }
        //public override string ToString()
        //{
        //    return $"{ShapeType} {X1} {Y1} {X2} {Y2}";
        //}
    }
}