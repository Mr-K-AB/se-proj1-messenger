using System;
using System.Collections.Generic;
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
using MessengerDashboard;
using LiveCharts;
using LiveCharts.Wpf;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for DashboardControl.xaml
    /// </summary>
    public partial class DashboardControl : UserControl
    {
        public DashboardControl()
        {
            InitializeComponent();
            DashboardViewModel viewModel = new();
            User user = new() { UserName = "Pratham", UserPicturePath = @"C:\\Users\\DELL\\Downloads\\Telegram Desktop\\MM.jpg" };

            viewModel.Users = new List<User>
            {
                user
            };
            viewModel.Summary = "Vinay Loves Hii Vinay";
            viewModel.Mode = "Lab Mode";
            DataContext = viewModel;
        }
    }
}
