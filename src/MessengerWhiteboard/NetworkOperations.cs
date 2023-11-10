using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        public void CreateIncomingShape(ShapeItem newShape)
        {
            if(newShape == null)
            {
                return;
            }

            ShapeItems.Add(newShape);
        }

        public void ModifyIncomingShape(ShapeItem newShape)
        {
            if(newShape == null)
            {
                return;
            }

            _ = ShapeItems.Remove(ShapeItems.FirstOrDefault(x => x.Id == newShape.Id));
            ShapeItems.Add(newShape);
        }

        public void DeleteIncomingShape(ShapeItem newShape)
        {
            if(newShape == null)
            {
                return;
            }

            _ = ShapeItems.Remove(ShapeItems.FirstOrDefault(x => x.Id == newShape.Id));
        }
    }
}
