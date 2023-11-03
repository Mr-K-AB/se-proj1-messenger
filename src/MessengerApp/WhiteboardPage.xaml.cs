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
        ViewModel ViewModel;

        public WhiteboardPage()
        {
            InitializeComponent();
            ViewModel = ViewModel.Instance;
            ViewModel.ShapeItems = new();
            this.DataContext = ViewModel;
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
