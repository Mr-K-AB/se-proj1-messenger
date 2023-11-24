﻿/******************************************************************************
* Filename    = AuthenticationViewModel.cs
*
* Author      = Vinay Ingle
*
* Roll Number = 112001050
*
* Product     = Messenger
* 
* Project     = MainApp
*
* Description = View Model for the Authentication page.
* *****************************************************************************/

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessengerViewModels.Stores;
using MessengerApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MessengerDashboard;
using MessengerViewModels.ViewModels;
namespace MessengerApp.ViewModels
{

    [INotifyPropertyChanged]
    internal partial class AuthenticationViewModel
    {
        [ObservableProperty]
        private bool? _isButtonEnabled = true;


        /// <summary>
        /// An async Function which uses Dashboard Authenticator Function
        /// and then changes the view from AuthenticationView to MainWindow.
        /// </summary>
        [RelayCommand]
        public async Task AuthenticateButton_Click(CancellationToken token)
        {
            IsButtonEnabled = false;
            //await Task.Delay(3000);
            IsButtonEnabled = true;

            AuthenticationResult authResult = await Authenticator.Authenticate();

            if (authResult.IsAuthenticated == false)
            {
                return;
            }

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
