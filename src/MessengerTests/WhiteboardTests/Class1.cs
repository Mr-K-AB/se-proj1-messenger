using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerWhiteboard.Models;

namespace MessengerTests.WhiteboardTests
{
    [TestClass]
    public class Utils
    {
        public bool Compare(ShapeItem shape1, ShapeItem shape2)
        {
            if(shape1 == null && shape2 == null)
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
                shape1.Fill == shape2.Fill&&
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

            for(int i = 0; i < list1.Count; i++)
            {
                if (!Compare(list1[i], list2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
