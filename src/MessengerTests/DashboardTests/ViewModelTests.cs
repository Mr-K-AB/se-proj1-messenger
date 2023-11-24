/******************************************************************************
* Filename    = ViewModelTests.cs
*
* Author      = Satish Patidar 
*
* Roll number = 112001037
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = Testing for View Model of Dashboard and Session
*****************************************************************************/

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
using MessengerDashboard.Client;
using MessengerDashboard;

namespace MessengerTests.DashboardTests
{
    /// <summary>
    /// Test class for Dashboard View Models
    /// </summary>
    [TestClass]
    public class ViewModelTests
    {
        /// <summary>
        /// Test for Dashboard Member View Model
        /// </summary>
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
            dashboardVM.Users = new();
            Assert.IsNotNull(dashboardVM.Users);
            dashboardVM.Summary = "text";
            Assert.IsNotNull(dashboardVM.Summary);
            dashboardVM.Mode = "Lab";
            Assert.IsNotNull(dashboardVM.Mode);
            Assert.IsNotNull(dashboardVM.PositiveChatCount);
            Assert.IsNotNull(dashboardVM.NegativeChatCount);
            Assert.IsNotNull(dashboardVM.NeutralChatCount);
            Assert.IsNotNull(dashboardVM.LabelPoint);
            Assert.IsNotNull(dashboardVM.OverallSentiment);
            dashboardVM.DateTimes = new();
            Assert.IsNotNull(dashboardVM.DateTimes);
            dashboardVM.TotalChatCount = 0;
            Assert.AreEqual(dashboardVM.TotalChatCount, 0);
            Assert.IsNotNull(dashboardVM.TotalChatCount);
            Assert.IsNotNull(dashboardVM.UserCounts);
            dashboardVM.MaxUserCount = 4;
            Assert.AreEqual(dashboardVM.MaxUserCount, 4);
            Assert.IsNotNull(dashboardVM.MaxUserCount);
            dashboardVM.UserActivities = new();
            Assert.IsNotNull(dashboardVM.UserActivities);
            dashboardVM.IsLocalSavingEnabled = true;
            Assert.IsNotNull(dashboardVM.IsLocalSavingEnabled);
            Assert.IsTrue(dashboardVM.IsLocalSavingEnabled);
            Assert.IsNotNull(dashboardVM.IsVisible);
            dashboardVM.IsVisible = true;
            Assert.IsTrue(dashboardVM.IsVisible);
        }

        /// <summary>
        /// Test for Dashboard Instructor View Model
        /// </summary>
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
            Assert.IsNotNull(dashboardVM.OverallSentiment);
            Assert.IsNotNull(dashboardVM.TotalChatCount);
            Assert.IsNotNull(dashboardVM.DateTimes);
            Assert.IsNotNull(dashboardVM.UserCounts);
            Assert.IsNotNull(dashboardVM.MaxUserCount);
            Assert.IsNotNull(dashboardVM.UserActivities);
            Assert.IsNotNull(dashboardVM.IsLocalSavingEnabled);
            dashboardVM.IsCloudSavingEnabled = true;
            Assert.IsTrue(dashboardVM.IsCloudSavingEnabled);
        }

        /// <summary>
        /// Test for Command Execution
        /// </summary>
        [TestMethod]
        public void TestCommandExecution()
        {
            // Arrange
            try
            {
                string cloudUrl = "http://localhost:7166/api/entity";
                Mock<SessionsViewModel> sessionsViewModelMock = new();
                RestClient restClient = new(cloudUrl);

                CloudCommand cloudCommand = new(restClient, sessionsViewModelMock.Object);
                cloudCommand.Execute(null);
                Thread.Sleep(1000);
                Assert.IsTrue(cloudCommand.CanExecute(null));

                SessionsViewModel sessionsViewModel = new()
                {
                    IsLocalClicked = true
                };
                DeleteAllCommand deleteAllCommand = new(restClient, sessionsViewModel);
                deleteAllCommand.Execute(null);
                Assert.IsTrue(deleteAllCommand.CanExecute(null));
                sessionsViewModel.IsLocalClicked = false;
                deleteAllCommand.Execute(null);
                Assert.IsTrue(deleteAllCommand.CanExecute(null));

                EndMeetCommand endMeetCommand = new();
                endMeetCommand.Execute(null);
                Assert.IsTrue(endMeetCommand.CanExecute(null));

                EntityInfoWrapper entity = new();
                sessionsViewModel.IsLocalClicked = false;
                ExpandCommand expandCommand = new(sessionsViewModel, entity);
                expandCommand.Execute(null);
                Assert.IsTrue(expandCommand.CanExecute(null));

                LocalCommand localCommand = new(sessionsViewModel);
                LocalCommand localCommand1 = new();
                localCommand.Execute(null);
                Assert.IsTrue(localCommand.CanExecute(null));

                RefreshCommand refreshCommand = new();
                refreshCommand.Execute(null);
                Assert.IsTrue(refreshCommand.CanExecute(null));

                SwitchModeCommand switchModeCommand = new();
                switchModeCommand.Execute(null);
                Assert.IsTrue(switchModeCommand.CanExecute(null));
                IClientSessionController client = DashboardFactory.GetClientSessionController();
                client.SessionInfo.SessionMode = SessionMode.Exam;
                switchModeCommand.Execute(null);
                Assert.IsTrue(switchModeCommand.CanExecute(null));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Test for Initialization
        /// </summary>
        public void InitializationTest()
        {
            Assert.IsNotNull(new DashboardInstructorViewModel());
            Assert.IsNotNull(new DashboardViewModel());
            Assert.IsNotNull(new DashboardMemberViewModel());
        }

        /// <summary>
        /// Test for Session View Model
        /// </summary>
        [TestMethod]
        public void SessionVMTest()
        {
            SessionsViewModel sessionsViewModel = new();
            Assert.IsNotNull(sessionsViewModel.LocalCommand);
            Assert.IsNotNull(sessionsViewModel.CloudCommand);
            Assert.IsNotNull(sessionsViewModel.DeleteAllCommand);

            sessionsViewModel.NegativeChatCount = 5;
            Assert.AreEqual(sessionsViewModel.NegativeChatCount, 5);

            List<TimeStampUserCountEntry> list = new();
            TimeStampUserCountEntry entry = new(DateTime.Now, 1);
            list.Add(entry);
            sessionsViewModel.TimeStampUserCountEntries = list;
            Assert.AreEqual(list, sessionsViewModel.TimeStampUserCountEntries);

            List<UserActivityEntry> userlist = new();
            UserActivityEntry entry1 = new(1, 1, "Sam", "abc123@gmail.com", DateTime.Now, DateTime.Now);
            userlist.Add(entry1);
            sessionsViewModel.UserActivities = userlist;
            Assert.AreEqual(userlist, sessionsViewModel.UserActivities);

            List<SessionEntry> sessionEntries = new();
            SessionEntry entry2 = new("1", new ExpandCommand(new(), new()));
            sessionEntries.Add(entry2);
            sessionsViewModel.Sessions = sessionEntries;
            Assert.AreEqual(sessionEntries, sessionsViewModel.Sessions);

            sessionsViewModel.SessionSummary = "We got full marks";
            Assert.AreEqual(sessionsViewModel.SessionSummary, "We got full marks");

            sessionsViewModel.PositiveChatCount = 5;
            Assert.AreEqual(sessionsViewModel.PositiveChatCount, 5);

            sessionsViewModel.TotalUserCount = 5;
            Assert.AreEqual(sessionsViewModel.TotalUserCount, 5);

            sessionsViewModel.NeutralChatCount = 5;
            Assert.AreEqual(sessionsViewModel.NeutralChatCount, 5);

            sessionsViewModel.OverallSentiment = "Yes you will get full marks";
            Assert.AreEqual(sessionsViewModel.OverallSentiment, "Yes you will get full marks");

            sessionsViewModel.IsLocalClicked = true;
            Assert.AreEqual(sessionsViewModel.IsLocalClicked, true);
        }
    }
}
