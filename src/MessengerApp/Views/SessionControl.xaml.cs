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

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for SessionControl.xaml
    /// </summary>
    public partial class SessionControl : UserControl
    {
        public SessionControl()
        {
            /*
            InitializeComponent();
            SessionsViewModel viewModel = new();

            SessionEntry entry = new() { SessionName = "S1", Summary = "All the best" };

            viewModel.Enteries = new List<SessionEntry>
            {
                entry
            };
            viewModel.SessionSummary = "Vinay Loves Hii Vinay";
            DataContext = viewModel;
            */
        }

        private void SubmissionsPage_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}
