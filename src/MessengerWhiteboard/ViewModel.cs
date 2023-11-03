using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerWhiteboard
{
    public partial class ViewModel
    {
        public ObservableCollection<ShapeItem> ShapeItems { get; set; }
        ShapeItem? tempShape;

        public string userID = "user1";

        public enum WBModes
        {
            CreateMode,
            ViewMode,
            DeleteMode,
            SelectMode
        }

        private WBModes currentMode;


        public ViewModel()
        {
            ShapeItems = new();
            currentMode = WBModes.ViewMode;

        }

        private static ViewModel? _instance;

        public static ViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ViewModel();
                }
                return _instance;
            }
        }

        public void AddShape(ShapeItem shape)
        {
            ShapeItems.Add(shape);
        }

        public void RemoveShape(ShapeItem shape)
        {
            ShapeItems.Remove(shape);
        }
        
        public void ChangeMode(WBModes mode)
        {
            Trace.WriteLine("Whiteboard View Model :: Active mode changed to : " + mode);
            this.currentMode = mode; 
        }

    }
}
