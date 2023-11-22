 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using MessengerCloud;
using MessengerDashboard;
using MessengerDashboard.UI.ViewModels;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for SessionControl.xaml
    /// </summary>
    public partial class SessionControl : UserControl
    {
        private readonly RestClient _restClient;
        private const string BaseUrl = @"http://localhost:7166/api/entity";
        public SessionControl()
        {
            InitializeComponent();
            SessionsViewModel viewModel = new();
            DataContext = viewModel;
            _restClient = new(BaseUrl);
        }

        private void SubmissionsPage_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread something = new(async () => {
                IReadOnlyList<Entity>? task = await _restClient.GetEntitiesAsync();
                IReadOnlyList<Entity>? results = task;
                Debug.WriteLine("done");
            })
            { IsBackground = true };
            something.Start();

        }
    }
}
