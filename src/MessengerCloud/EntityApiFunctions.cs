/******************************************************************************
* Filename    = EntityApiFunctions.cs
*
* Author      = Shubh Pareek
*
* Roll Number = 112001039
*
* Product     = Messenger 
* 
* Project     = MessengerCloud
*
* Description = A class for entity api functions.
*****************************************************************************/

using Azure;
using Azure.Data.Tables;
using MessengerCloud;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessengerCloud
{
    /// <summary>
    /// Custom Azure functions APIs class.
    /// </summary>
    public static class EntityApi
    {
        private const string TableName = "Entities";
        private const string ConnectionName = "AzureWebJobsStorage";
        private const string Route = "entity";

        /// <summary>
        /// Azure Function to create a new entity.
        /// </summary>
        /// <param name="req">The HTTP request containing the entity information.</param>
        /// <param name="entityTable">The Azure Table Storage collector for entities.</param>
        /// <param name="log">The logger for logging information.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("CreateEntity")]
        public static async Task<IActionResult> CreateEntity(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route)] HttpRequest req,
                [Table(TableName, Connection = ConnectionName)] IAsyncCollector<Entity> entityTable,
                ILogger log)
        {
            Trace.WriteLine("[EntityApi]: create entity called");

            // Read the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Debug.WriteLine("request Body is ", requestBody);

            // Deserialize the JSON body into an EntityInfoWrapper object
            EntityInfoWrapper info = JsonSerializer.Deserialize<EntityInfoWrapper>(requestBody);

            // Create an Entity object from the EntityInfoWrapper
            Entity value = new(info);
            Debug.WriteLine("val inside api ", value);

            // Add the entity to the Azure Table Storage
            await entityTable.AddAsync(value);

            Trace.WriteLine("[EntityApi]: entity created");
            return new OkObjectResult(value);
        }

        /// <summary>
        /// Azure Function to get an entity by its ID.
        /// </summary>
        /// <param name="req">The HTTP request containing the entity ID.</param>
        /// <param name="entity">The retrieved entity from Azure Table Storage.</param>
        /// <param name="log">The logger for logging information.</param>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("GetEntityById")]
        public static IActionResult GetEntityById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/{id}")] HttpRequest req,
        [Table(TableName, Entity.PartitionKeyName, "{id}", Connection = ConnectionName)] Entity entity,
        ILogger log,
        string id)
        {
            Trace.WriteLine("[EntityApi]: get entity called");
            log.LogInformation($"Getting entity {id}");

            // Check if the entity is not found
            if (entity == null)
            {
                log.LogInformation($"Entity {id} not found");
                return new NotFoundResult();
            }

            Trace.WriteLine("[EntityApi]: entity sent");
            return new OkObjectResult(entity);
        }

        /// <summary>
        /// Azure Function to get all entities.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="tableClient">The Azure Table Storage client for entities.</param>
        /// <param name="log">The logger for logging information.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("GetEntities")]
        public static async Task<IActionResult> GetEntitiesAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)] HttpRequest req,
        [Table(TableName, Connection = ConnectionName)] TableClient tableClient,
        ILogger log)
        {
            Trace.WriteLine("[EntityApi]: get entities called");
            log.LogInformation("Getting all entity items");

            // Query Azure Table Storage for all entities
            Page<Entity> page = await tableClient.QueryAsync<Entity>().AsPages().FirstAsync();

            Trace.WriteLine("[EntityApi]: entities returned");
            return new OkObjectResult(page.Values);
        }

        /// <summary>
        /// Azure Function to delete an entity by its ID.
        /// </summary>
        /// <param name="req">The HTTP request containing the entity ID.</param>
        /// <param name="entityClient">The Azure Table Storage client for entities.</param>
        /// <param name="log">The logger for logging information.</param>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("DeleteEntity")]
        public static async Task<IActionResult> DeleteEntity(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/{id}")] HttpRequest req,
        [Table(TableName, ConnectionName)] TableClient entityClient,
        ILogger log,
        string id)
        {
            Trace.WriteLine("[EntityApi]: Delete entity called");
            log.LogInformation($"Deleting entity by {id}");

            try
            {
                // Delete the entity from Azure Table Storage
                await entityClient.DeleteEntityAsync(Entity.PartitionKeyName, id, ETag.All);
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            Trace.WriteLine("[EntityApi]: Deleted entity ");
            return new OkResult();
        }

        /// <summary>
        /// Azure Function to delete all entities.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="entityClient">The Azure Table Storage client for entities.</param>
        /// <param name="log">The logger for logging information.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [FunctionName("DeleteEntities")]
        public static async Task<IActionResult> DeleteEntities(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route)] HttpRequest req,
        [Table(TableName, ConnectionName)] TableClient entityClient,
        ILogger log)
        {
            Trace.WriteLine("[EntityApi]: Delete all called ");
            log.LogInformation($"Deleting all entity items");

            try
            {
                // Delete all entities from Azure Table Storage
                await entityClient.DeleteAsync();
            }
            catch (RequestFailedException e) when (e.Status == 404)
            {
                return new NotFoundResult();
            }

            Trace.WriteLine("[EntityApi]: Deleted all ");
            return new OkResult();
        }
    }
}
