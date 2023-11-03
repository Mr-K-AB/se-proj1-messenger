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

        private string userID = "tempUser";

        public bool isEnabled;
        public enum WBModes
        {
            CreateMode,
            ViewMode,
            DeleteMode,
            SelectMode
        }

        private WBModes currentMode;
        //shape attributes
        Brush fillBrush;                                            // stores color of the object (fill colour)
        Brush borderBrush;                                 // stores color of the border
        int strokeWidth;  
        
        List<ShapeItem> selectedShapes;                                         // thickness of the stroke

        public ViewModel()
        {
            ShapeItems = new();
            this.currentMode = WBModes.ViewMode;
            if(this.userID == "tempUser")
            {
                this.isEnabled = false;
            }
            SetUserID();
            this.fillBrush = null;                                            // stores color of the object (fill colour)
            this.borderBrush = Brushes.Black;                                 // stores color of the border
            this.strokeWidth = 1;
            this.selectedShapes = new List<ShapeItem>;
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

        public string GetUserID()
        {
            // return this.userID;
            return "user1";
        }

        public void SetUserID()
        {
            this.userID = this.GetUserID();
            this.isEnabled = true;
        }

        public void ChangeStrokeWidth(int width)
        {
            this.strokeWidth = width;
            Trace.WriteLine("Whiteboard View Model :: Width changed to : " + width);
            this.UpdateStrokeWidth();
        }

        public void ChangeBorderBrush(string bcolour)
        {
            this.borderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(bcolour));
            Trace.WriteLine("Whiteboard View Model :: border colour changed to : " + bcolour);
            this.UpdateBorderBrush();
        }

        public void ChangeFillBrush(string fcolour)
        {
            this.fillBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(fcolour));
            Trace.WriteLine("Whiteboard View Model :: fill colour changed to : " + fcolour);
            this.UpdateFillBrush();
        }

        public void UpdateStrokeWidth()
        {
            for(int i = 0; i < selectedShapes.Count(); i++)
            {
                ShapeItem newShape = this.selectedShapes[i].DeepClone();
                newShape.strokeThickness = this.strokeWidth;
                this.selectedShapes[i] = newShape;
            }
        }

        public void UpdateBorderBrush(string bcolour)
        {
            for(int i = 0; i < selectedShapes.Count(); i++)
            {
                ShapeItem newShape = this.selectedShapes[i].DeepClone();
                newShape.Stroke = this.borderBrush;
                this.selectedShapes[i] = newShape;
            }
        }

        public void UpdateFillBrush(string fcolour)
        {
            for(int i = 0; i < selectedShapes.Count(); i++)
            {
                ShapeItem newShape = this.selectedShapes[i].DeepClone();
                newShape.Fill = this.fillBrush;
                this.selectedShapes[i] = newShape;
            }
        }
    }
}
