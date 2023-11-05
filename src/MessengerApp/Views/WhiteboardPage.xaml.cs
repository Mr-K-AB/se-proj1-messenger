using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MessengerWhiteboard;

namespace MessengerApp
{
    public partial class WhiteboardPage : Page
    {
        readonly ViewModel _viewModel;
        private bool _buildingShape = false;

        public WhiteboardPage()
        {
            InitializeComponent();

            _viewModel = ViewModel.Instance;
            _viewModel.ShapeItems = new();
            DataContext = _viewModel;
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
            if (_viewModel.currentMode == ViewModel.WBModes.CreateMode)
            {
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.StartShape(p);
                _buildingShape = true;
            }
            else if (_viewModel.currentMode == ViewModel.WBModes.SelectMode)
            {
                if (sender is not Canvas canvas)
                {
                    return;
                }

                HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
                DependencyObject element = hitTestResult.VisualHit;
                Debug.WriteLine(element);
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_buildingShape)
            {
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.BuildShape(p);
            }
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_buildingShape)
            {
                Debug.WriteLine("Mouse Up");
                Point p = e.GetPosition(sender as Canvas);
                _viewModel.EndShape(p);
                e.Handled = true;
                _buildingShape = false;
            }
            //Debug.WriteLine(ViewModel.ShapeItems[0].ToString());
        }

        public void SelectMode(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeMode(ViewModel.WBModes.SelectMode);
            _viewModel.ChangeTool("Select");
            Trace.WriteLine("Whiteboard View Model :: Mode changed to : " + _viewModel.currentMode);
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

        private void ColorGreen(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeFillBrush(Brushes.Green);
        }

        private void ColorRed(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeFillBrush(Brushes.Red);

        }

        private void ColorYellow(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeFillBrush(Brushes.Yellow);
        }

        private void ColorNull(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeFillBrush(null);
        }

        private void ColorBlue(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeFillBrush(Brushes.Blue);
        }

        private void ColorBlack(object sender, RoutedEventArgs e)
        {
            _viewModel.ChangeFillBrush(Brushes.Black);
        }


    }
}
