using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MessengerWhiteboard;

namespace MessengerApp
{
    public partial class WhiteboardPage : Page
    {
        ViewModel viewModel;

        public WhiteboardPage()
        {
            InitializeComponent();

            ViewModel = ViewModel.Instance;
            ViewModel.ShapeItems = new();
            this.DataContext = ViewModel;

        private void SampleRectangleClick(object sender, RoutedEventArgs e)
        {
            //change active tool to rectangle
            this.viewModel.UnselectAll();
            this.viewModel.ChangeMode(ViewModel.WBModes.CreateMode);
        }

        private void SampleDeleteClick(object sender, RoutedEventArgs e)
        {
            //change active tool to rectangle
            this.viewModel.UnselectAll();
            this.viewModel.ChangeMode(ViewModel.WBModes.DeleteMode);
        }

        private void SampleSelectClick(object sender, RoutedEventArgs e)
        {
            //change active tool to rectangle
            this.viewModel.UnselectAll();
            this.viewModel.ChangeMode(ViewModel.WBModes.SelectMode);
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(sender as Canvas);
            ViewModel.StartShape(p);
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(sender as Canvas);
            ViewModel.EndShape(p);
            e.Handled = true;
        }
    }
}
