using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MessengerWhiteboard
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        public BindingList<ShapeItem> ShapeItems { get; set; }
        ShapeItem? _tempShape;

        private string _userID = "tempUser";

        //public string shapeMode = "Rectangle";
        public string activeTool = "Select";

        public bool isEnabled;
        public enum WBModes
        {
            CreateMode,
            ViewMode,
            DeleteMode,
            SelectMode
        }

        public WBModes currentMode;
        //shape attributes
        public Brush fillBrush;                                            // stores color of the object (fill colour)
        //Brush borderBrush;                                 // stores color of the border
        int _strokeWidth;                                       // thickness of the stroke

        public ViewModel()
        {
            ShapeItems = new();
            currentMode = WBModes.ViewMode;
            if(_userID == "tempUser")
            {
                isEnabled = false;
            }
            SetUserID();
            //this.fillBrush = null;                                            // stores color of the object (fill colour)
            //this.borderBrush = Brushes.Black;                                 // stores color of the border
            _strokeWidth = 1;
        }

        private static ViewModel? s_instance;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


        public static ViewModel Instance
        {
            get
            {
                if (s_instance != null)
                {
                    return s_instance;
                }
                s_instance = new ViewModel();
                return s_instance;
            }
        }

        public void AddShape(ShapeItem shape)
        {
            Debug.WriteLine("Inside AddShape");
            ShapeItems.Add(shape);
        }

        public void RemoveShape(ShapeItem shape)
        {
            ShapeItems.Remove(shape);
        }
        
        public void ChangeMode(WBModes mode)
        {
            Trace.WriteLine("Whiteboard View Model :: Active mode changed to : " + mode);
            currentMode = mode; 
        }

        public void ChangeTool(string tool)
        {
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + tool);
            activeTool = tool;
        }

        public string GetUserID()
        {
            // return this.userID;
            return "user1";
        }

        public void SetUserID()
        {
            _userID = GetUserID();
            isEnabled = true;
        }

        public void ChangeStrokeWidth(int width)
        {
            _strokeWidth = width;
            Trace.WriteLine("Whiteboard View Model :: Width changed to : " + width);
            //this.UpdateStrokeWidth();
        }

        //public void ChangeBorderBrush(string bcolour)
        //{
        //    this.borderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(bcolour));
        //    Trace.WriteLine("Whiteboard View Model :: border colour changed to : " + bcolour);
        //    this.UpdateBorderBrush();
        //}

        public void ChangeFillBrush(Brush color)
        {
            fillBrush = color;
            Trace.WriteLine("Whiteboard View Model :: fill colour changed to : " + color);
            //this.UpdateFillBrush();
        }

        //public void UpdateStrokeWidth()
        //{
        //    for(int i = 0; i < selectedShapes.Count(); i++)
        //    {
        //        ShapeItem newShape = this.selectedShapes[i].DeepClone();
        //        newShape.strokeThickness = this.strokeWidth;
        //        this.selectedShapes[i] = newShape;
        //    }
        //}

        //public void UpdateBorderBrush(string bcolour)
        //{
        //    for(int i = 0; i < selectedShapes.Count(); i++)
        //    {
        //        ShapeItem newShape = this.selectedShapes[i].DeepClone();
        //        newShape.Stroke = this.borderBrush;
        //        this.selectedShapes[i] = newShape;
        //    }
        //}

        //public void UpdateFillBrush(string fcolour)
        //{
        //    for (int i = 0; i < selectedShapes.Count(); i++)
        //    {
        //        ShapeItem newShape = this.selectedShapes[i].DeepClone();
        //        newShape.Fill = this.fillBrush;
        //        this.selectedShapes[i] = newShape;
        //    }
        //}
    }
}
