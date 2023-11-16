using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;

namespace MessengerWhiteboard
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        public BindingList<ShapeItem> ShapeItems { get; set; }
        public ShapeItem? _tempShape;

        private string _userID = "tempUser";
        public bool isServer = true;

        //public string shapeMode = "Rectangle";
        public string activeTool = "Select";
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
        public Brush fillBrush = Brushes.Transparent;
        public Brush strokeBrush = Brushes.Black;
        public double StrokeThickness { get; set; }
        //Brush borderBrush;                                 // stores color of the border
        int _strokeWidth;                                       // thickness of the stroke
        public IShapeReceiver machine;

        public ViewModel()
        {
            ShapeItems = new();
            currentMode = WBModes.ViewMode;
            if (_userID == "tempUser")
            {
                isEnabled = false;
            }
            //this.fillBrush = null;                                            // stores color of the object (fill colour)
            //this.borderBrush = Brushes.Black;                                 // stores color of the border
            _strokeWidth = 1;
            StrokeThickness = 1;
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
            Debug.Print(ShapeItems[^1].points.First().ToString());
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

        public void SetUserID(int _userid)
        {
            _userID = _userid.ToString();
            //_userID = GetUserID();
            if(_userid == 1)
            {
                isServer = false;
            }
            if(isServer)
            {
                machine = ServerState.Instance;
            }
            else
            {
                machine = ClientState.Instance;
            }
            machine.SetUserId(_userID);
        }

        public void ChangeStrokeWidth(int width)
        {
            _strokeWidth = width;
            Trace.WriteLine("Whiteboard View Model :: Width changed to : " + width);
            //this.UpdateStrokeWidth();
        }

        public void ChangeStrokeBrush(Brush bcolour)
        {
            strokeBrush = bcolour;
            Trace.WriteLine("Whiteboard View Model :: border colour changed to : " + bcolour);
            //this.UpdateBorderBrush();
        }

        public void ChangeFillBrush(Brush fcolour)
        {
            fillBrush = fcolour;
            Trace.WriteLine("Whiteboard View Model :: fill colour changed to : " + fcolour);
            //this.UpdateFillBrush();
        }

        public void ClearScreen()
        {
            ShapeItems.Clear();
            machine.OnShapeReceived(null, Operation.Clear);
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
        //    for (int i = 0; i < selectedShapes.Count(); i++)
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
