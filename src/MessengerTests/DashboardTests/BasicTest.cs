/******************************************************************************
* Filename    = RestClient.cs
*
* Author      = Shubh Pareek
*
* Roll Number = 112001039
*
* Product     = Messenger 
* 
* Project     = Messenger Tests
*
* Description =   Basic tests for small methods .
* * *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessengerCloud;
using MessengerDashboard;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace MessengerTests.DashboardTests.DashboardTests
{
    /// <summary>
    /// Basic tests for small methods .
    /// </summary>
    [TestClass]
    public class BasicTest
    {


        private Analysis _analysisCloud;
        readonly List<string> _sentences = new() { "Hi", "Hello", "Wow" };
        //private readonly LocalSave _save = new();
        public BasicTest()
        {
            _analysisCloud = new(
                 new()
                ,
                 new()
                ,
                 2
                ,
                 2
                );
            _analysisCloud.TimeStampToUserCountMap.Add(DateTime.Now, 3);
            _analysisCloud.UserIdToUserActivityMap.Add(4, new UserActivity() { EntryTime = DateTime.Now, ExitTime = DateTime.Now, UserEmail = "hello", UserChatCount = 2, UserName = "shubh" });
        }
        /// <summary>
        /// checking if the class is serializable or not  
        /// </summary>
        [TestMethod]
        public void Test1()
        {
            _analysisCloud = new(
                 new()
                ,
                 new()
                ,
                 2
                ,
                 2
                );
            //_analysisCloud.TimeStampToUserCountMap.Add(DateTime.Now, 3);
            _analysisCloud.UserIdToUserActivityMap.Add(4, new UserActivity() { /*EntryTime = DateTime.Now, ExitTime = DateTime.Now, */UserEmail = "hello", UserChatCount = 2, UserName = "shubh" });
            string ans = JsonSerializer.Serialize(_analysisCloud);
            Analysis res = JsonSerializer.Deserialize<Analysis>(ans);
            Assert.IsTrue(res != null);


        }
        [TestMethod]
        public void localSaveDeleteTesting()
        {
            Logger.LogMessage("Deleting all entries from our local table storage.");


            LocalSave.DeleteFile();
            // Get the local application data folder
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDataFolder = Path.Combine(localAppData, "Messenger");

            // Combine the "Messenger" folder with the "sessionInfo.txt" file
            string path = Path.Combine(appDataFolder, "sessionInfo.txt");

            // Check if the file exists
            if (File.Exists(path))
            {
                Assert.Fail("file exists ");
            }

        }



        /// <summary>
        /// Tests creating and getting an local entity.
        /// </summary>
        [TestMethod]
        public void LocalSaveTesting()
        {

            // Create an entity.
            Logger.LogMessage("Create an entity.");
            var info = new EntityInfoWrapper(_sentences, 1, 2, 3, "Positive", "-1", _analysisCloud);
            List<EntityInfoWrapper> infos = new()
            {
                info
            };

            LocalSave.AddEntity(info);
            List<EntityInfoWrapper> res = LocalSave.ReadFromFile();
            Assert.AreEqual(JsonSerializer.Serialize(infos[0].Sentences), JsonSerializer.Serialize(res[0].Sentences));
        }






    }
}
