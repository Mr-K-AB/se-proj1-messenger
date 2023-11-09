using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessengerApp.Stores;
using MessengerApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MessengerApp.ViewModels
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
            await Task.Delay(3000);
            IsButtonEnabled = true;


            NavigationStore navigationStore = new();
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
