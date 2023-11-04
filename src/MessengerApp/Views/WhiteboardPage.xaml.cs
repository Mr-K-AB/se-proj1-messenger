using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private bool buildingShape = false;

        public WhiteboardPage()
        {
            InitializeComponent();

            ViewModel = ViewModel.Instance;
            ViewModel.ShapeItems = new();
            this.DataContext = ViewModel;
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
            if(ViewModel.currentMode == ViewModel.WBModes.CreateMode)
            {
                var p = e.GetPosition(sender as Canvas);
                ViewModel.StartShape(p);
                buildingShape = true;
            }
            else if(ViewModel.currentMode == ViewModel.WBModes.SelectMode)
            {
                var canvas = sender as Canvas;
                if (canvas == null)
                    return;

                HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
                var element = hitTestResult.VisualHit;
                Debug.WriteLine(element);
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if(buildingShape)
            {
                var p = e.GetPosition(sender as Canvas);
                ViewModel.BuildShape(p);
            }
        }
        
        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(buildingShape)
            {
                Debug.WriteLine("Mouse Up");
                var p = e.GetPosition(sender as Canvas);
                ViewModel.EndShape(p);
                e.Handled = true;
                buildingShape = false;
            }
            //Debug.WriteLine(ViewModel.ShapeItems[0].ToString());
        }

        public void SelectMode(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeMode(ViewModel.WBModes.SelectMode);
            Trace.WriteLine("Whiteboard View Model :: Mode changed to : " + this.ViewModel.currentMode);
        }
        public void RectangleMode(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeShapeMode("Rectangle");
            ViewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + this.ViewModel.shapeMode);
        }

        public void EllipseMode(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeShapeMode("Ellipse");
            ViewModel.ChangeMode(ViewModel.WBModes.CreateMode);
            Trace.WriteLine("Whiteboard View Model :: Active shape changed to : " + this.ViewModel.shapeMode);
        }
    }
}
