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
        ViewModel ViewModel;
        private bool _buildingShape = false;

        public WhiteboardPage()
        {
            InitializeComponent();

            ViewModel = ViewModel.Instance;
            ViewModel.ShapeItems = new();
            DataContext = ViewModel;
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
            if (ViewModel.currentMode == ViewModel.WBModes.CreateMode)
            {
                Point p = e.GetPosition(sender as Canvas);
                ViewModel.StartShape(p);
                _buildingShape = true;
            }
            else if (ViewModel.currentMode == ViewModel.WBModes.SelectMode)
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
                ViewModel.BuildShape(p);
            }
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_buildingShape)
            {
                Debug.WriteLine("Mouse Up");
                Point p = e.GetPosition(sender as Canvas);
                ViewModel.EndShape(p);
                e.Handled = true;
                _buildingShape = false;
            }
            //Debug.WriteLine(ViewModel.ShapeItems[0].ToString());
        }

        public void SelectMode(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeMode(ViewModel.WBModes.SelectMode);
            Trace.WriteLine("Whiteboard View Model :: Mode changed to : " + ViewModel.currentMode);
        }
        public void RectangleMode(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeShapeMode("Rectangle");
            ViewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + ViewModel.shapeMode);
        }

        public void EllipseMode(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeShapeMode("Ellipse");
            ViewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + ViewModel.shapeMode);
        }


        private void CanvasMouseEnter(object sender, MouseEventArgs e)
        {
            if (ViewModel.activeTool != "Select")
                viewModel.UnselectAll();
            switch (this.activeTool)
            {
                case "Select":
                    Cursor = Cursors.Arrow;
                    break;
                case "Rectangle":
                    Cursor = Cursors.Cross;
                    break;
                default:
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void CanvasMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.currentTool != "Select")
                viewModel.UnselectAll();
            Cursor = Cursors.Arrow;

        }

        private void ColorGreen(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Green);
        }

        private void ColorRed(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Red);

        }

        private void ColorYellow(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Yellow);
        }

        private void ColorNull(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(null);
        }

        private void ColorBlue(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Blue);
        }

        private void ColorBlack(object sender, RoutedEventArgs e)
        {
            viewModel.ChangeFillBrush(Brushes.Black);
        }


    }
}
