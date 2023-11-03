using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MessengerWhiteboard;

namespace MessengerApp
{
    public partial class WhiteboardPage : Page
    {
        ViewModel viewModel;

        public WhiteboardPage()
        {
            InitializeComponent();
            viewModel = viewModel.Instance;
            this.DataContext = viewModel;
        }

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
    }
}
