using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessengerTestUI.Stores;
using MessengerTestUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MessengerDashboard;

namespace MessengerTestUI.ViewModels
{
    [INotifyPropertyChanged]
    internal partial class AuthenticationViewModel
    {
        [ObservableProperty]
        private bool? _isButtonEnabled = true;

        [RelayCommand]
        public async Task AuthenticateButton_Click(CancellationToken token)
        {
            IsButtonEnabled = false;
            //await Task.Delay(3000);
            IsButtonEnabled = true;

            //AuthenticationResult authResult = await Authenticator.Authenticate();
            AuthenticationResult authResult = new()
            {
                IsAuthenticated = true,
                UserEmail = "mocktest@gmail.com",
                UserName = "Application",
                UserImage = @"https://lh3.googleusercontent.com/a/ACg8ocJKjXYyRFOA-tiDwFz2dvu65CH5s1V-bsuu5aG_PMvmrA\\u003ds96-c\"
            };

            NavigationStore navigationStore = new()
            {
                AuthResult = authResult
            };
            navigationStore.CurrentViewModel = new HomeViewModel(navigationStore);
            
            var newWindow = new MainWindow
            {
                DataContext = new MainViewModel(navigationStore)
            };
            newWindow.Show();


            //used null propogation
            Application.Current.MainWindow?.Close();
        }
    }
}
