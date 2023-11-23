/***************************
* Filename    = NetworkOperations.cs
*
* Author      = Syed Abdul Mateen
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file contains all the operations that can be executed on
*               incoming shapes from network.
***************************/
using System.Diagnostics;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        public void CreateIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
            {
                return;
            }
            Debug.Print($"CreateIncomingShape: {newShape.StrokeThickness}");
            //Debug.Print($"TransparentStroke: {Brushes.Black}");


            if (ShapeItems.Contains(newShape))
            {
                Debug.Print($"Removed shape");
                _ = ShapeItems.Remove(newShape);
            }

            ShapeItems.Add(newShape);
            Debug.Print(ShapeItems.Count.ToString());
        }

        public void ModifyIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
            {
                return;
            }
            Debug.Print("ModifyIncomingShape");

            _ = ShapeItems.Remove(newShape);
            ShapeItems.Add(newShape);
        }

        public void DeleteIncomingShape(ShapeItem newShape)
        {
            if (newShape == null)
            {
                return;
            }

            _ = ShapeItems.Remove(ShapeItems.FirstOrDefault(x => x.Id == newShape.Id));
        }

        public void ClearIncomingShapes()
        {
            ShapeItems.Clear();
        }
    }
}
