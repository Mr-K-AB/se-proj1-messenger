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

        // stores selected shapes
        ShapeItem? _tempShape;
        // public List<string> selectedShapes;                   

        private string _userID = "tempUser";

        //public string shapeMode = "Rectangle";
        public string activeTool;
        public System.Windows.Point? lastDownPoint = null;


        public bool isEnabled;
        public enum WBModes
        {
            CreateMode,
            ViewMode,
            DeleteMode,
            SelectMode,
            UndoMode, 
            RedoMode
        }

        public WBModes currentMode;
        
        //shape attributes
        public Brush _fillBrush;                                  // stores color of the object (fill colour)
        public Brush _borderBrush;                                // stores color of the border
        int _strokeWidth;                                        // thickness of the stroke

        public ViewModel()
        {
            ShapeItems = new();
            this.currentMode = WBModes.ViewMode;
            this.activeTool = "Select";
           
            if(_userID == "tempUser")
            {
                isEnabled = false;
            }
            
            SetUserID();

            this._strokeWidth = 1;
            this._borderBrush = Brushes.Black;
            this._fillBrush = null;
            // this.selectedShapes = new List<string>();
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
            //Debug.WriteLine("Inside AddShape");
            ShapeItems.Add(shape);
        }

        public void RemoveShape(ShapeItem shape)
        {
            ShapeItems.Remove(shape);
        }

        //public void SelectShape(string shUID)
        //{
            
        //    // This is valid only for single shape selection
        //    if (selectedShapes.Count() > 0)
        //    {
        //        if (selectedShapes.Contains(shUID))
        //        {
        //            selectedShapes.Remove(shUID);
        //        }
        //        else
        //        {
        //            selectedShapes.Add(shUID);
        //        }
        //    }
        //    else
        //    {
        //        selectedShapes.Add(shUID);
        //    }
        //}
        
        public void ChangeMode(WBModes mode)
        {
            Trace.WriteLine("Whiteboard View Model :: Active mode changed to : " + mode);
            currentMode = mode; 
        }

        public void ChangeTool(string tool)
        {
            Trace.WriteLine("Whiteboard View Model :: Active tool changed to : " + tool);
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
            this.UpdateShape("Stroke");
        }

        public void ChangeBorderBrush(Brush color)
        {
            _borderBrush = color;
            Trace.WriteLine("Whiteboard View Model :: border colour changed to : " + color);
            this.UpdateShape("StrokeThickness");
        }

        public void ChangeFillBrush(Brush color)
        {
            _fillBrush = color;
            Trace.WriteLine("Whiteboard View Model :: fill colour changed to : " + color);
            this.UpdateShape("Fill");
        }

        public void UpdateShape(string property)
        {
            // This will work in case of single shape only 
            string shUID = (string)_tempShape.Id;

            for (int i = 0; i < ShapeItems.Count(); i++)
            {
                if (shUID == ((string)ShapeItems[i].Id))
                {
                    ShapeItem newShape = ShapeItems[i].DeepClone();
                    // ShapeItem newShape = CreateShape()

                    switch (property)
                    {
                        case "Stroke":
                            newShape.Stroke = this._borderBrush;
                            break;
                        case "StrokeThickness":
                            newShape.StrokeThickness = this._strokeWidth;
                            break;
                        case "Fill":
                            newShape.Fill = this._fillBrush; 
                            break;
                    }
                    
                    ShapeItems[i] = newShape;

                    // TODO: Call render shape to render the updated shape
                }
            }
        }

  
    }
}
