using MessengerDashboard;
using MessengerDashboard.UI.ViewModels;
using MessengerScreenshare.Server;
using MessengerViewModels.Stores;
using MessengerViewModels.ViewModels;

namespace MessengerTests.ViewModelTests
{
    [TestClass]
    public class ViewModelTests
    {
        [TestMethod]
        public void HomeViewModelTest()
        {

            AuthenticationResult authResult = new()
            {
                IsAuthenticated = true,
                UserImage = "testimage",
                UserEmail = "vinay@gmail.com",
                UserName = "Test"
            };

            NavigationStore navigationStore = new()
            {
                AuthResult = authResult
            };

            HomeViewModel homeViewModel = new(navigationStore);

            Assert.AreEqual(homeViewModel.UserName, authResult.UserName);
            Assert.AreEqual(homeViewModel.UserImage, authResult.UserImage);
            Assert.AreEqual(homeViewModel.WelcomeMessage, "Welcome To the home page");

            homeViewModel.JoinMeetIP = "192.168.0.128";
            homeViewModel.JoinMeetPort = 23924;
            Assert.AreEqual(homeViewModel.JoinMeetIP, "192.168.0.128");
            Assert.AreEqual(homeViewModel.JoinMeetPort ,23924);

            homeViewModel.NavigateServerMeetCommand.Execute(null);
            if(navigationStore.CurrentViewModel is ServerMeetViewModel)
            {
                Assert.IsTrue(true);
            }

            homeViewModel.NavigateClientMeetCommand.Execute(null);
            if(navigationStore.CurrentViewModel is ClientMeetViewModel)
            {
                Assert.IsTrue(true);
            }

        }

        [TestMethod]
        public void ClientMeetViewModelTest()
        {
            AuthenticationResult authResult = new()
            {
                IsAuthenticated = true,
                UserImage = "testimage",
                UserEmail = "vinay@gmail.com",
                UserName = "Test"
            };

            NavigationStore navigationStore = new()
            {
                AuthResult = authResult
            };

            ClientMeetViewModel clientMeetViewModel = new(navigationStore)
            {
                IP = "127.0.0.1",
                Port = 2334,
            };

            Assert.AreEqual(clientMeetViewModel.IP, "127.0.0.1");
            Assert.AreEqual(clientMeetViewModel.Port, 2334);


        }

        [TestMethod]
        public void ServerMeetViewModelTest()
        {
            AuthenticationResult authResult = new()
            {
                IsAuthenticated = true,
                UserImage = "testimage",
                UserEmail = "vinay@gmail.com",
                UserName = "Test"
            };

            NavigationStore navigationStore = new()
            {
                AuthResult = authResult,
                SubViewModel = new DashboardInstructorViewModel()
            };

            ServerMeetViewModel serverMeetViewModel = new(navigationStore)
            {
                IP = "127.0.0.1",
                Port = 2334
            };

            navigationStore.CurrentViewModel = serverMeetViewModel;

            Assert.AreEqual(serverMeetViewModel.IP, "127.0.0.1");
            Assert.AreEqual(serverMeetViewModel.Port, 2334);

            serverMeetViewModel.NavigateServerDashboardCommand.Execute(null);
            if (navigationStore.SubViewModel is not DashboardInstructorViewModel)
            {
                Assert.Fail();
            }
           
            serverMeetViewModel.NavigateServerScreenshareCommand.Execute(null);

            if (navigationStore.SubViewModel is not ScreenshareServerViewModel)
            {
                Assert.Fail();
            }
            serverMeetViewModel.NavigateServerWhiteboardCommand.Execute(null);

            if (navigationStore.SubViewModel is not MessengerWhiteboard.ViewModel)
            {
                Assert.Fail();
            }

            serverMeetViewModel.NavigateHomeCommand.Execute(null);
            if(navigationStore.CurrentViewModel is not HomeViewModel)
            {
                Assert.Fail();
            }


        }

        [TestMethod]
        public void MainNavigationTest()
        {
            AuthenticationResult authResult = new()
            {
                IsAuthenticated = true,
                UserImage = "testimage",
                UserEmail = "vinay@gmail.com",
                UserName = "Test"
            };

            NavigationStore navigationStore = new()
            {
                AuthResult = authResult,
                SubViewModel = new DashboardInstructorViewModel()
            };
            MainViewModel mainViewModel = new(navigationStore);

            navigationStore.CurrentViewModel = mainViewModel;

            if(mainViewModel.CurrentViewModel is not MainViewModel)
            {
                Assert.Fail();
            }

        }
    }
}
