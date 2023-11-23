/***********************************
*Filename = WhiteboardControl.xaml.cs
*
*Author   = Thogata Jagadeesh
*
* Product     = Messenger
* 
* Project     = Whiteboard
*
* Description = Whiteboard View
*************************************/
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MessengerWhiteboard;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for WhiteboardControl.xaml
    /// </summary>
    public partial class WhiteboardControl : UserControl
    {
        ViewModel _viewModel => ViewModel.Instance;
        private bool _buildingShape = false;

        public WhiteboardControl()
        {
            InitializeComponent();

            DataContext = ViewModel.Instance;
            _viewModel.ShapeItems = new();
        }


        /// <summary>
        /// Mouse Click Event for the WhiteBorad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the position where the mouse was clicked on the canvas.
            Point p = e.GetPosition(sender as Canvas);
            if (_viewModel.currentMode == ViewModel.WBModes.CreateMode)
            {
                // Start creating a new shape at the clicked position.
                _viewModel.StartShape(p);
                _buildingShape = true;
            }
            else if (_viewModel.currentMode == ViewModel.WBModes.SelectMode)
            {
                // Ensure the sender is a Canvas object.
                if (sender is not Canvas canvas)
                {
                    return;
                }

                // Perform a hit test to find the UI element at the clicked position.
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, p);
                DependencyObject? element = hitTestResult.VisualHit;
                //Debug.WriteLine(element);

                // Check if the hit UI element is a Path (shape)
                if (element is Path)
                {
                    _viewModel.lastDownPoint = p; // Store the clicked position.
                    //Debug.WriteLine(element.GetValue(UidProperty).ToString());
                    _viewModel.SelectShape(element.GetValue(UidProperty).ToString());
                }
            }
            else if (_viewModel.currentMode == ViewModel.WBModes.DeleteMode)
            {
                if (sender is not Canvas canvas)
                {
                    return;
                }
                _viewModel.lastDownPoint = p;


                HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, p);
                DependencyObject element = hitTestResult.VisualHit;
                //Debug.WriteLine(element);
                if (element is Path)
                {
                    _viewModel.lastDownPoint = p;
                    //Debug.WriteLine(element.GetValue(UidProperty).ToString());
                    _viewModel.SelectShape(element.GetValue(UidProperty).ToString());
                }
            }
        }

        /// <summary>
        /// Event capturing the mouse move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            // Check if a shape is currently being built (during a drag operation).
            if (_buildingShape)
            {
                // Get the current mouse position on the canvas.
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.BuildShape(p); // Update the shape being built with the new position.

            }
            // Check if the 'Select' tool is active and an initial point has been set.
            else if (_viewModel.lastDownPoint != null && _viewModel.activeTool == "Select")
            {
                //Debug.WriteLine(_viewModel.lastDownPoint);
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.BuildShape(p);
            }
            // Check if the 'Delete' tool is active and an initial point has been set.
            else if (_viewModel.lastDownPoint != null && _viewModel.activeTool == "Delete")
            {
                Point p = e.GetPosition(sender as Canvas);

                // Check if there is no temporary shape being manipulated.
                if (_viewModel._tempShape == null)
                {
                    if (sender is not Canvas canvas)
                    {
                        return;
                    }
                    // Perform a hit test to find the UI element at the clicked position.
                    HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, p);
                    DependencyObject element = hitTestResult.VisualHit;
                    //Debug.WriteLine(element);
                    if (element is System.Windows.Shapes.Path)
                    {
                        _viewModel.lastDownPoint = p;
                        //Debug.WriteLine(element.GetValue(UidProperty).ToString());
                        _viewModel.SelectShape(element.GetValue(UidProperty).ToString());
                    }
                }
                else
                {
                    _viewModel.BuildShape(p);
                }
            }
        }

        /// <summary>e.Handled = true; 
        /// Canvas Mouseup  or mouse release event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Check if a shape is currently being built.
            Point p = e.GetPosition(sender as Canvas);
            if (_buildingShape)
            {
                //Debug.WriteLine("Mouse Up");
                _viewModel.EndShape(p); // Finalize the shape being built at the current mouse position.
                e.Handled = true;       // Mark the event as handled to prevent further processing.
                _buildingShape = false; // This flag indicates shape has been built
            }

            // Reset the stored 'last down point' to indicate no active drag operation.
            if (_viewModel.activeTool == "Select" && _viewModel.lastDownPoint != null)
            {
                _viewModel.EndShape(p);
                e.Handled = true;
            }
            _viewModel.lastDownPoint = null;
            //Debug.WriteLine(ViewModel.ShapeItems[0].ToString());
        }

        /// <summary>
        /// Function corresponding to the select mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeMode(ViewModel.WBModes.SelectMode);
            _viewModel.ChangeTool("Select");
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Function corresponding to the rectangle icon in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RectangleMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Rectangle");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Cursor = Cursors.Cross;
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Ellipse icon function in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EllipseMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Ellipse");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Cursor = Cursors.Cross;
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Line icon function in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LineMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Line");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Cursor = Cursors.Cross;
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Text box click function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Text");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Cursor = Cursors.IBeam;
            Trace.WriteLine("Whiteboard View Model :: Active tool changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Undo button function in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UndoMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Undo");
            _viewModel.ChangeMode(ViewModel.WBModes.UndoMode);
            Trace.WriteLine("Whiteboard View Model :: Active tool changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Function called when clicking the redo button in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RedoMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Redo");
            _viewModel.ChangeMode(ViewModel.WBModes.RedoMode);
            Trace.WriteLine("Whiteboard View Model :: Active tool changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// FUnction corresponding to the pen icon in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CurveMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Curve");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Cursor = Cursors.Pen;
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Toolbar Delete button function 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeleteMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Delete");
            _viewModel.ChangeMode(ViewModel.WBModes.DeleteMode);
            Cursor = Cursors.Arrow;
            Trace.WriteLine("Whiteboard View Model :: Active tool changed to : " + _viewModel.activeTool);
        }

        /// <summary>
        /// Function corresponding to the event where the mouse enters the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseEnter(object sender, MouseEventArgs e)
        {
            //if (_viewModel.activeTool != "Select")
            //{
            //    _viewModel.UnselectAll();
            //}

            // Change the cursor style based on the active tool in the viewModel.
            // This provides a visual cue to the user about the current mode of operation.
            Cursor = _viewModel.activeTool switch
            {
                "Select" => Cursors.Arrow,   //If the 'Select' tool is active, use the arrow cursor.
                "Rectangle" => Cursors.Cross,// If the 'Rectangle' tool is active, use the cross cursor.
                _ => Cursors.Arrow,          // Default to the arrow cursor for other cases.
            };
        }

        /// <summary>
        /// Function to invoke when the mouse leaves the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            //if (_viewModel.activeTool != "Select")
            //{
            //    _viewModel.UnselectAll();
            //}

            Cursor = Cursors.Arrow;

        }

        /// <summary>
        /// Functions for changing the stroke colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeStrokeColor(object sender, RoutedEventArgs e)
        {
            //Debug.Print((e.Source as Button).Name);
            if (e.Source is not RadioButton radioButton)
            {
                return;
            }
            Brush bcolor = radioButton.Background;
            _viewModel.ChangeStrokeBrush(bcolor);
        }

        /// <summary>
        /// Function for changing the fill color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeFillColor(object sender, RoutedEventArgs e)
        {
            //Debug.Print((e.Source as Button).ToolTip);
            if (e.Source is not RadioButton radioButton)
            {
                return;
            }
            Brush bcolor = radioButton.Background;
            _viewModel.ChangeFillBrush(bcolor);
        }

        /// <summary>
        /// Function corresponding to the clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearScreen(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you REALLY want to clear the screen?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _viewModel.ClearScreen();
            }
        }

        /// <summary>
        /// Undo button function in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo(object sender, RoutedEventArgs e)
        {
            _viewModel.CallUndo();
        }

        /// <summary>
        /// Function called when clicking the redo button in the toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Redo(object sender, RoutedEventArgs e)
        {
            _viewModel.CallRedo();
        }

        /// <summary>
        /// Function called when saving session  button is clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMode(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveSnapshot();
        }

    }
}
