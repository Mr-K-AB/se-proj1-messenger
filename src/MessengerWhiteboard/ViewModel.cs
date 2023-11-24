using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using MessengerWhiteboard.Interfaces;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        public BindingList<ShapeItem> ShapeItems { get; set; }  // list to store the shapes to be rendered
        public List<ShapeItem> HighlightShapeItems;     // list to store the corners of a shape to be resized
        public BindingList<string> SavedSessions { get; set; }  // session list 
        public ShapeItem? _tempShape;   // shape under operation 
        public ShapeItem? _selectedShape; // shape which is currently selected
        public ShapeItem? _selectedCorner;  // corner shape which is use for tracking resize

        public string _userID = "tempUser"; // user id 
        public bool isServer = true;    // flag to check server status 

        public string activeTool = "Select";    // active tool 
        public System.Windows.Point? lastDownPoint = null;  // storing the latest mouse down point

        // Canvas status
        public bool isEnabled;

        // Enum for Modes
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
        
        // Shape attributes
        public Brush fillBrush = Brushes.Transparent;
        public Brush strokeBrush = Brushes.Black;
        public double StrokeThickness { get; set; }
        // Brush borderBrush;                                 // stores color of the border
        public int _strokeWidth;                                       // thickness of the stroke
        
        public IShapeReceiver machine;
        public bool _testing = false;

        // Constructor for ViewModel
        public ViewModel()
        {
            ShapeItems = new();
            SavedSessions = new();
            HighlightShapeItems = new();
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

        // INotifyPropertyChanged properties
        private static ViewModel? s_instance;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Method to be implemented for INotifyPropertyChanged
        /// </summary> 
        /// <param name="property"></param>
        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Provides a global point of access to the ViewModel instance.
        /// </summary>
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

        /// <summary>
        /// Property indicating whether the current context is a server or not.
        /// </summary>
        public bool IsServer
        {
            get { return isServer; }
            set
            {
                if (isServer != value)
                {
                    isServer = value;
                    OnPropertyChanged(nameof(IsServer));
                }
            }
        }

        /// <summary>
        /// Adds a given shape to the collection of shape items.
        /// </summary>
        /// <param name="shape">The ShapeItem to be added to the collection.</param>
        public void AddShape(ShapeItem shape)
        {
            //Debug.WriteLine("Inside AddShape");
            ShapeItems.Add(shape);
            Debug.Print(ShapeItems[^1].points.First().ToString());
        }

        /// <summary>
        /// Removes a given shape from the collection of shape items.
        /// </summary>
        /// <param name="shape">The ShapeItem to be removed from the collection.</param>
        public void RemoveShape(ShapeItem shape)
        {
            ShapeItems.Remove(shape);
        }

        /// <summary>
        /// Changes the current mode of the whiteboard.
        /// </summary>
        /// <param name="mode">The mode to which the whiteboard will be set.</param>
        public void ChangeMode(WBModes mode)
        {
            Trace.WriteLine("Whiteboard View Model :: Active mode changed to : " + mode);
            currentMode = mode;
        }

        /// <summary>
        /// Changes the active drawing tool used in the whiteboard.
        /// </summary>
        /// <param name="tool">The name of the tool to be set as active.</param>
        public void ChangeTool(string tool)
        {
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + tool);
            activeTool = tool;
        }

        /// <summary>
        /// Retrieves the user ID. Currently, this method is hardcoded to return "user1".
        /// </summary>
        /// <returns>The user ID.</returns>
        public string GetUserID()
        {
            // return this.userID;
            return "user1";
        }

        /// <summary>
        /// Sets the user ID and updates the machine state based on the given user ID.
        /// </summary>
        /// <param name="_userid">The user ID to be set.</param>
        public void SetUserID(int _userid)
        {
            _userID = _userid.ToString();
            //_userID = GetUserID();
            if (_userid == 1)
            {
                isServer = false;
            }
            if (isServer)
            {
                machine = ServerState.Instance;
            }
            else
            {
                machine = ClientState.Instance;
            }
            machine.SetUserId(_userID);
        }

        /// <summary>
        /// Changes the stroke width for the drawing tools.
        /// </summary>
        /// <param name="width">The new stroke width to be set.</param>
        public void ChangeStrokeWidth(int width)
        {
            _strokeWidth = width;
            Trace.WriteLine("Whiteboard View Model :: Width changed to : " + width);
        }

        /// <summary>
        /// Changes the stroke brush color used in the whiteboard.
        /// </summary>
        /// <param name="bcolour">The new brush color to be set for strokes.</param>
        public void ChangeStrokeBrush(Brush bcolour)
        {
            strokeBrush = bcolour;
            Trace.WriteLine("Whiteboard View Model :: border colour changed to : " + bcolour);
        }

        /// <summary>
        /// Changes the fill brush color used in the whiteboard.
        /// </summary>
        /// <param name="fcolour">The new brush color to be set for filling shapes.</param>
        public void ChangeFillBrush(Brush fcolour)
        {
            fillBrush = fcolour;
            Trace.WriteLine("Whiteboard View Model :: fill colour changed to : " + fcolour);
        }

        /// <summary>
        /// Clears all shapes from the screen, and resets the undo and redo stacks.
        /// </summary>
        public void ClearScreen()
        {
            ShapeItems.Clear();
            undoStackElements.Clear();
            redoStackElements.Clear();
            machine.OnShapeReceived(null, Operation.Clear);
        }
    }
}
