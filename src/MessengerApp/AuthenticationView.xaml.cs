using MessengerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MessengerApp.Views
{
    /// <summary>
    /// Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : Window
    {
        readonly AuthenticationViewModel _authViewModel = new();
        public AuthenticationView()
        {
            DataContext = _authViewModel;
            InitializeComponent();
        }
    }
}
