







using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/*
using PlexShareWhiteboard;
using PlexShareWhiteboard.BoardComponents;
*/



namespace MessengerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /*
        WhiteBoardViewModel viewModel;
        string currentTool;
        bool singleTrigger = true;
        */

        public MainWindow()
        {
            /*
            InitializeComponent();
            viewModel = WhiteBoardViewModel.Instance;

            Trace.WriteLine("[WhiteBoard] White Board Page is initialised serverId: " + serverID + "viewModel.userId : " + viewModel.userId);

            if (serverID == 0)
                viewModel.isServer = true;
            else
                viewModel.isServer = false;
            */


            /*if (!viewModel.userId.Equals("init") && serverID != 0)
            {
                Trace.WriteLine("[WhiteBoard] recalling setuserid");
                // this might be that the dashboard had called this before
                // default it is not a server so we need to reiniiliase only if this is server
                int passId = Int32.Parse(viewModel.userId);
                viewModel.SetUserId(passId);
            }
            viewModel.ShapeItems = new ObservableCollection<ShapeItem>();
            this.DataContext = viewModel;
            this.currentTool = "Select";
            this.RestorFrameDropDown.SelectionChanged += RestorFrameDropDownSelectionChanged;




            private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
            {
                if (viewModel.canDraw)
                {
                    var a = e.GetPosition(sender as Canvas);
                    viewModel.ShapeStart(a);
                    singleTrigger = true;
                    if (viewModel.select.ifSelected)
                    {

                        string shapeName = viewModel.select.selectedObject.Geometry.GetType().Name;
                        if (shapeName == "EllipseGeometry" || shapeName == "RectangleGeometry")
                        {
                            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                                this.StrokeToolBar.Visibility = Visibility.Collapsed;
                            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                                this.ShapeToolBar.Visibility = Visibility.Collapsed;
                            if (this.ShapeSelectionToolBar.Visibility == Visibility.Collapsed)
                                this.ShapeSelectionToolBar.Visibility = Visibility.Visible;
                        }

                        else if (shapeName == "PathGeometry" || shapeName == "LineGeometry")
                        {
                            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                                this.ShapeToolBar.Visibility = Visibility.Collapsed;
                            if (this.StrokeToolBar.Visibility == Visibility.Collapsed)
                                this.StrokeToolBar.Visibility = Visibility.Visible;
                            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            if (this.ShapeToolBar.Visibility == Visibility.Visible)
                                this.ShapeToolBar.Visibility = Visibility.Collapsed;
                            if (this.StrokeToolBar.Visibility == Visibility.Visible)
                                this.StrokeToolBar.Visibility = Visibility.Collapsed;
                            if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                                this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        if (this.ShapeToolBar.Visibility == Visibility.Visible)
                            this.ShapeToolBar.Visibility = Visibility.Collapsed;
                        if (this.StrokeToolBar.Visibility == Visibility.Visible)
                            this.StrokeToolBar.Visibility = Visibility.Collapsed;
                        if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                            this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                    }
                }
            }*/

            /* /// <summary>
             /// Event capturing the mouse move
             /// </summary>
             /// <param name="sender"></param>
             /// <param name="e"></param>
             private void CanvasMouseMove(object sender, MouseEventArgs e)
             {
                 var a = e.GetPosition(sender as Canvas);
                 viewModel.ShapeBuilding(a);

             }
            */

            /*private void CanvasMouseEnter(object sender, MouseEventArgs e)
            {
                if (this.currentTool != "Select")
                    viewModel.UnHighLightIt();
                switch (this.currentTool)
                {
                    case "Select":
                        Cursor = Cursors.Arrow;
                        break;
                    case "Rectangle":
                        Cursor = Cursors.Cross;
                        break;
                    case "Ellipse":
                        Cursor = Cursors.Cross;
                        break;
                    case "Freehand":
                        Cursor = Cursors.Pen;
                        break;
                    case "Eraser":
                        Cursor = Cursors.Arrow;
                        break;
                    case "text":
                        Cursor = Cursors.Arrow;
                        break;
                    case "Line":
                        Cursor = Cursors.Cross;
                        break;
                    default:
                        Cursor = Cursors.Arrow;
                        break;
                }
            }*/
/*
            /// <summary>
            /// Function corresponding to the rectangle icon in the toolbar
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void RectangleCreateMode(object sender, RoutedEventArgs e)
            {
                viewModel.UnHighLightIt();
                viewModel.select.ifSelected = false;
                this.currentTool = "Rectangle";
                if (this.StrokeToolBar.Visibility == Visibility.Visible)
                    this.StrokeToolBar.Visibility = Visibility.Collapsed;
                if (this.ShapeSelectionToolBar.Visibility == Visibility.Visible)
                    this.ShapeSelectionToolBar.Visibility = Visibility.Collapsed;
                if (this.ShapeToolBar.Visibility == Visibility.Collapsed)
                    this.ShapeToolBar.Visibility = Visibility.Visible;
                viewModel.ChangeMode("create_rectangle");
            }
*/

        }
    }
}
