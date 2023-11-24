/******************************************************************************
* Filename    = CloudTests.cs
*
* Author      = Shubh Pareek
*
* Roll Number = 112001039
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description =  Tests for messenger cloud .
* *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessengerCloud;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;


namespace MessengerTests.CloudTests
{
    /// <summary>
    /// for doing tests on messenger cloud module 
    /// </summary>
    [TestClass]
    
    public class CloudTests
    {
        private const string BaseUrl = @"http://localhost:7166/api/entity";
        //for testing api functions 
        private readonly HttpClient _entityClient;
        //for testing cloud rest client 
        private readonly RestClient _restClient;
        //dummy variables for sake of passing information 
        private readonly Analysis _analysisCloud;
        readonly List<string> _sentences = new() { "Hi", "Hello", "Wow" };


        public CloudTests()
        {
            _restClient = new(BaseUrl);
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
            _entityClient = new HttpClient();
        }

        /// <summary>
        /// Tests creating and getting an entity.
        /// </summary>
        [TestMethod]
        public async Task TestPostAndGet()
        {
            // delete all 
            Logger.LogMessage("Deleting all entries from our Azure table storage.");
            await _restClient.DeleteEntitiesAsync();

            // Create an entity.
            Logger.LogMessage("Create an entity.");
            var info = new EntityInfoWrapper(_sentences, 1, 2, 3, "Positive", "-1", _analysisCloud);
            Entity? postEntity = await _restClient.PostEntityAsync(info);
            // Get the entity.
            Logger.LogMessage("Get the entity.");
            Entity? getEntity = await _restClient.GetEntityAsync(postEntity?.Id);

            // Validate.
            Logger.LogMessage("Validate.");
            Assert.AreEqual(postEntity?.Id, getEntity?.Id);
            //Assert.AreEqual(postEntity?.Name, getEntity?.Name);
            //now delete
            Logger.LogMessage("Delete the entity.");
            await _restClient.DeleteEntityAsync(postEntity?.Id);

            // Validate.
            // Trying to get the entity should throw an exception.
            try
            {
                // Get the entity.
                Logger.LogMessage("Getting the entity.");
                getEntity = await _restClient.GetEntityAsync(postEntity?.Id);
                Assert.Fail("Trying to get a deleted entity did not throw an exception.");
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                Logger.LogMessage("Rightly got the expected exception trying to get a deleted entity.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Unexpected exception type. Message = {ex.Message}");
            }
        }
        /// <summary>
        /// adds entities, deletes them 
        /// </summary>
        /// <returns>nothing</returns>
        [TestMethod]
        public async Task TestDeleteAllAndGetAll()
        {
            // Delete any existing entities.
            Logger.LogMessage("Delete any existing entities.");
            await _restClient.DeleteEntitiesAsync();

            // Create three entities.
            Logger.LogMessage("Create three entities.");
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, 3, "Neutral", "-1", _analysisCloud));
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, 4, "Negative", "0", _analysisCloud));
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, 5, "Positive", "1", _analysisCloud));

            // Validate.
            Logger.LogMessage("Validate.");
            IReadOnlyList<Entity>? entities = await _restClient.GetEntitiesAsync();
            Assert.AreEqual(entities?.Count, 3);
            // Delete any existing entities.
            Logger.LogMessage("Delete any existing entities.");
            await _restClient.DeleteEntitiesAsync();
        }
        /// <summary>
        /// for deleting by id and getting by id .
        /// </summary>
        [TestMethod]

        public async Task TestGetidAndDelid()
        {
            //delete so that it doesn't cause any issues 
            await _restClient.DeleteEntitiesAsync();
            _ = await _restClient.PostEntityAsync(new EntityInfoWrapper(_sentences, 1, 2, 5, "Positive", "1", _analysisCloud));
            //get the same entity anda validate 
            Logger.LogMessage("Validate.");
            Entity resultt = await _restClient.GetEntityAsync("1");
            //delete it 
            await _restClient.DeleteEntityAsync("1");
            Assert.AreEqual(resultt.Id, "1");
        }
        /// <summary>
        /// Test method to evaluate the functionality of various API operations.
        /// </summary>
        [TestMethod]
        public async Task TestApiFunctions()
        {
            // Delete all entities using the API
            Debug.WriteLine("deleting all ");
            using HttpResponseMessage response = await _entityClient.DeleteAsync(BaseUrl);
            response.EnsureSuccessStatusCode();

            // Create a new EntityInfoWrapper object for testing
            var info = new EntityInfoWrapper(_sentences, 1, 2, 3, "Positive", "-1", _analysisCloud);
            string json = System.Text.Json.JsonSerializer.Serialize(info);

            // Convert the JSON string to a ByteArrayContent
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Post the new entity to the API
            using HttpResponseMessage responsee = await _entityClient.PostAsync(BaseUrl, content);
            responsee.EnsureSuccessStatusCode();
            string result = await responsee.Content.ReadAsStringAsync();

            // Deserialize the API response to an Entity object
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            Entity? entity = JsonSerializer.Deserialize<Entity>(result, options);

            // Retrieve all entities from the API
            HttpResponseMessage responseee = await _entityClient.GetAsync(BaseUrl);
            responseee.EnsureSuccessStatusCode();
            result = await responseee.Content.ReadAsStringAsync();
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            // Deserialize the API response to a list of Entity objects
            IReadOnlyList<Entity>? entities = JsonSerializer.Deserialize<IReadOnlyList<Entity>>(result, options);

            // Retrieve a specific entity by ID from the API
            HttpResponseMessage responseeee = await _entityClient.GetAsync(BaseUrl + $"/-1");
            responseeee.EnsureSuccessStatusCode();
            result = await responseeee.Content.ReadAsStringAsync();
            options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            // Deserialize the API response to an Entity object
            entity = JsonSerializer.Deserialize<Entity>(result, options);

            // Delete the specific entity with ID "-1" using the API
            Debug.WriteLine("deleting this -1");
            using HttpResponseMessage responseeeee = await _entityClient.DeleteAsync(BaseUrl + $"/-1");
        }

        /// <summary>
        /// Test method to verify the behavior of the Entity class.
        /// </summary>
        [TestMethod]
        public void TestEntity()
        {
            // Create a new EntityInfoWrapper object for testing
            var info = new EntityInfoWrapper(_sentences, 1, 2, 3, "Positive", "-1", _analysisCloud);

            // Create an Entity object using the EntityInfoWrapper
            Entity entity = new(info);

            // Assert that the Entity object is not null
            Assert.IsNotNull(entity);

            // Assert that the ID of the Entity matches the expected value "-1"
            Assert.AreEqual(entity.Id, "-1");
        }

    }



}
