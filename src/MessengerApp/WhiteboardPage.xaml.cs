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
        ViewModel ViewModel;

        public WhiteboardPage()
        {
            InitializeComponent();
            ViewModel = ViewModel.Instance;
            this.DataContext = ViewModel;
        }
    }
}
