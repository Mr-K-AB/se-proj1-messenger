using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWhiteboard
{
    internal class ViewModel
    {
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }

        public ViewModel()
        {
            ShapeItems = new ObservableCollection<ShapeItem>();
        }

        public void AddShape(ShapeItem shape)
        {
            ShapeItems.Add(shape);
        }

        public void RemoveShape(ShapeItem shape)
        {
            ShapeItems.Remove(shape);
        }


    }
}
