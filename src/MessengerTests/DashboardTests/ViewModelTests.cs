using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerCloud;
using MessengerDashboard.UI.DataModels;
using MessengerDashboard.UI.Commands;
using MessengerDashboard.UI.ViewModels;
using MessengerTests.DashboardTests.Mocks;
using Moq;

namespace MessengerTests.DashboardTests
{
    [TestClass]
    public class ViewModelTests
    {
        [TestMethod]
        public void DashboardMemberViewModelTest()
        {
            MockClientSessionController client = new();
            DashboardMemberViewModel dashboardVM = new(client)
            {
                IsLocalSavingEnabled = true
            };
            client.ConnectToServer("0.0.0.0", 0, "Name", "1@gmail.com", "photo");
            client.SendExamModeRequestToServer();
            Assert.AreEqual(dashboardVM.Mode, "Exam");
            client.SendLabModeRequestToServer();
            client.SendRefreshRequestToServer();
            Assert.AreEqual(dashboardVM.Mode, "Lab");
            client.SendExitSessionRequestToServer();
            Assert.IsNotNull(dashboardVM.Users);
            Assert.IsNotNull(dashboardVM.Summary);
            Assert.IsNotNull(dashboardVM.Mode);
            Assert.IsNotNull(dashboardVM.PositiveChatCount);
            Assert.IsNotNull(dashboardVM.NegativeChatCount);
            Assert.IsNotNull(dashboardVM.NeutralChatCount);
            Assert.IsNotNull(dashboardVM.LabelPoint);
            Assert.IsNotNull(dashboardVM.PositiveLabel);
            Assert.IsNotNull(dashboardVM.NegativeLabel);
            Assert.IsNotNull(dashboardVM.NeutralLabel);
            Assert.IsNotNull(dashboardVM.OverallSentiment);
            Assert.IsNotNull(dashboardVM.TotalChatCount);
            Assert.IsNotNull(dashboardVM.DateTimes);
            Assert.IsNotNull(dashboardVM.UserCounts);
            Assert.IsNotNull(dashboardVM.MaxUserCount);
            Assert.IsNotNull(dashboardVM.UserActivities);
            Assert.IsNotNull(dashboardVM.UserNames);
            Assert.IsNotNull(dashboardVM.UserChatCounts);
            Assert.IsNotNull(dashboardVM.IsLocalSavingEnabled);
        }

        [TestMethod]
        public void DashboardInstructorViewModelTest()
        {
            MockClientSessionController client = new();
            DashboardInstructorViewModel dashboardVM = new(client)
            {
                IsLocalSavingEnabled = true,
                IsCloudSavingEnabled = true
            };
            client.ConnectToServer("0.0.0.0", 0, "Name", "1@gmail.com", "photo");
            client.SendExamModeRequestToServer();
            Assert.AreEqual(dashboardVM.Mode, "Exam");
            client.SendLabModeRequestToServer();
            client.SendRefreshRequestToServer();
            Assert.AreEqual(dashboardVM.Mode, "Lab");
            client.SendExitSessionRequestToServer();
            Assert.IsNotNull(dashboardVM.Users);
            Assert.IsNotNull(dashboardVM.Summary);
            Assert.IsNotNull(dashboardVM.Mode);
            Assert.IsNotNull(dashboardVM.PositiveChatCount);
            Assert.IsNotNull(dashboardVM.NegativeChatCount);
            Assert.IsNotNull(dashboardVM.NeutralChatCount);
            Assert.IsNotNull(dashboardVM.LabelPoint);
            Assert.IsNotNull(dashboardVM.PositiveLabel);
            Assert.IsNotNull(dashboardVM.NegativeLabel);
            Assert.IsNotNull(dashboardVM.NeutralLabel);
            Assert.IsNotNull(dashboardVM.OverallSentiment);
            Assert.IsNotNull(dashboardVM.TotalChatCount);
            Assert.IsNotNull(dashboardVM.DateTimes);
            Assert.IsNotNull(dashboardVM.UserCounts);
            Assert.IsNotNull(dashboardVM.MaxUserCount);
            Assert.IsNotNull(dashboardVM.UserActivities);
            Assert.IsNotNull(dashboardVM.UserNames);
            Assert.IsNotNull(dashboardVM.UserChatCounts);
            Assert.IsNotNull(dashboardVM.IsLocalSavingEnabled);
        }

        [TestMethod]
        public void TestCommandExecution()
        {
            // Arrange
            string cloudUrl = "http://localhost:7166/api/entity";
            Mock<SessionsViewModel> sessionsViewModelMock = new();
            RestClient restClient = new (cloudUrl);
            CloudCommand cloudCommand = new(restClient, sessionsViewModelMock.Object);
            cloudCommand.Execute(null);
            Thread.Sleep(1000);
            Assert.IsTrue(cloudCommand.CanExecute(null));
        }

        public void InitializationTest()
        {
            Assert.IsNotNull(new DashboardInstructorViewModel());
            Assert.IsNotNull(new DashboardViewModel());
            Assert.IsNotNull(new DashboardMemberViewModel());
        }
    }
}
