using System;
using System.Collections.Generic;
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
using MessengerWhiteboard;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for WhiteboardControl.xaml
    /// </summary>
    public partial class WhiteboardControl : UserControl
    {
        readonly ViewModel _viewModel;
        private bool _buildingShape = false;

        public WhiteboardControl(int serverID)
        {
            InitializeComponent();

            _viewModel = ViewModel.Instance;
            _viewModel.ShapeItems = new();
            DataContext = _viewModel;
            _viewModel.SetUserID(serverID);
        }

        //private void SampleRectangleClick(object sender, RoutedEventArgs e)
        //{
        //    //change active tool to rectangle
        //    this.ViewModel.UnselectAll();
        //    this.ViewModel.ChangeMode(ViewModel.WBModes.CreateMode);
        //}

        //private void SampleDeleteClick(object sender, RoutedEventArgs e)
        //{
        //    //change active tool to rectangle
        //    this.viewModel.UnselectAll();
        //    this.viewModel.ChangeMode(ViewModel.WBModes.DeleteMode);
        //}

        //private void SampleSelectClick(object sender, RoutedEventArgs e)
        //{
        //    //change active tool to rectangle
        //    this.viewModel.UnselectAll();
        //    this.viewModel.ChangeMode(ViewModel.WBModes.SelectMode);
        //}

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(sender as Canvas);
            if (_viewModel.currentMode == ViewModel.WBModes.CreateMode)
            {
                _viewModel.StartShape(p);
                _buildingShape = true;
            }
            else if (_viewModel.currentMode == ViewModel.WBModes.SelectMode)
            {
                if (sender is not Canvas canvas)
                {
                    return;
                }

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
                if (element is System.Windows.Shapes.Path)
                {
                    _viewModel.lastDownPoint = p;
                    //Debug.WriteLine(element.GetValue(UidProperty).ToString());
                    _viewModel.SelectShape(element.GetValue(UidProperty).ToString());
                }
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_buildingShape)
            {
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.BuildShape(p);
            }
            else if (_viewModel.lastDownPoint != null && _viewModel.activeTool == "Select")
            {
                //Debug.WriteLine(_viewModel.lastDownPoint);
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.BuildShape(p);
            }
            else if (_viewModel.lastDownPoint != null && _viewModel.activeTool == "Delete")
            {
                Point p = e.GetPosition(sender as Canvas);

                if (_viewModel._tempShape == null)
                {
                    if (sender is not Canvas canvas)
                    {
                        return;
                    }

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

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_buildingShape)
            {
                //Debug.WriteLine("Mouse Up");
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.EndShape(p);
                e.Handled = true;
                _buildingShape = false;
            }
            _viewModel.lastDownPoint = null;
            //Debug.WriteLine(ViewModel.ShapeItems[0].ToString());
        }

        public void SelectMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeMode(ViewModel.WBModes.SelectMode);
            _viewModel.ChangeTool("Select");
        }
        public void RectangleMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Rectangle");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void EllipseMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Ellipse");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void LineMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Line");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void TextMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Text");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void UndoMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Undo");
            _viewModel.ChangeMode(ViewModel.WBModes.UndoMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void RedoMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Redo");
            _viewModel.ChangeMode(ViewModel.WBModes.RedoMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void CurveMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Curve");
            _viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }

        public void DeleteMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeTool("Delete");
            _viewModel.ChangeMode(ViewModel.WBModes.DeleteMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + _viewModel.activeTool);
        }


        private void CanvasMouseEnter(object sender, MouseEventArgs e)
        {
            //if (_viewModel.activeTool != "Select")
            //{
            //    _viewModel.UnselectAll();
            //}

            Cursor = _viewModel.activeTool switch
            {
                "Select" => Cursors.Arrow,
                "Rectangle" => Cursors.Cross,
                _ => Cursors.Arrow,
            };
        }

        private void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            //if (_viewModel.activeTool != "Select")
            //{
            //    _viewModel.UnselectAll();
            //}

            Cursor = Cursors.Arrow;

        }

        private void ChangeStrokeColor(object sender, RoutedEventArgs e)
        {
            //Debug.Print((e.Source as Button).Name);
            Brush bcolor = (e.Source as RadioButton).Background;
            _viewModel.ChangeStrokeBrush(bcolor);
        }

        private void ChangeFillColor(object sender, RoutedEventArgs e)
        {
            //Debug.Print((e.Source as Button).ToolTip);
            Brush bcolor = (e.Source as RadioButton).Background;
            _viewModel.ChangeFillBrush(bcolor);
        }

        private void ClearScreen(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearScreen();
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            _viewModel.CallUndo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            _viewModel.CallRedo();
        }

        //private void SaveMode(object sender, RoutedEventArgs e)
        //{
        //    _viewModel.SaveSession(new Random());
        //}

    }
}
