/// <credits>
/// <author>
/// <name>Shubh Pareek</name>
/// <rollnumber>112001039</rollnumber>
/// </author>
/// </credits>
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MessengerCloud;

namespace MessengerCloud
{
    public class RestClient
    {
        private readonly HttpClient _entityClient;
        private readonly string _url;

        /// <summary>
        /// Creates an instance of the RestClient class.
        /// </summary>
        /// <param name="url">The base URL of the http client.</param>
        public RestClient(string url)
        {
            _entityClient = new();
            _url = url;
            Trace.WriteLine("[RestClient]: new client created ");
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get a particular entity.
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <returns>The entity with the given id.</returns>
        public async Task<Entity?> GetEntityAsync(string? id)
        {
            Trace.WriteLine("[RestClient]: get entity async called ");
            HttpResponseMessage response = await _entityClient.GetAsync(_url + $"/{id}");
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            Entity? entity = JsonSerializer.Deserialize<Entity>(result, options);
            Trace.WriteLine("[RestClient]: entity deserialized ");
            return entity;
        }

        /// <summary>
        /// Makes a "GET" call to our Azure function APIs to get all entities.
        /// </summary>
        /// <returns>All the entities created so far</returns>
        public async Task<IReadOnlyList<Entity>?> GetEntitiesAsync()
        {
            Trace.WriteLine("[RestClient]: get entities called ");
            HttpResponseMessage response = await _entityClient.GetAsync(_url);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            IReadOnlyList<Entity>? entities = JsonSerializer.Deserialize<IReadOnlyList<Entity>>(result, options);
            Trace.WriteLine("[RestClient]: got entities, and returned   ");

            return entities;
        }

        /// <summary>
        /// Makes a "POST" call to our Azure function APIs.
        /// </summary>
        /// <param name="name">Name of the entity.</param>
        /// <returns>A new entity with the given name.</returns>
        public async Task<Entity?> PostEntityAsync(EntityInfoWrapper info)
        {
            Trace.WriteLine("[RestClient]: Post entity called ");
            string json = System.Text.Json.JsonSerializer.Serialize(info);

            // Convert the JSON string to a ByteArrayContent
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _entityClient.PostAsync(_url, content);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,

            };

            Entity? entity = JsonSerializer.Deserialize<Entity>(result, options);
            Trace.WriteLine("[RestClient]: posted successfully");
            return entity;
        }

        /// <summary>
        /// Makes a "DELETE" call to our Azure function APIs to delete a particular entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Task.</returns>
        public async Task DeleteEntityAsync(string? id)
        {
            Trace.WriteLine("[RestClient]: Delete entity called ");
            Debug.WriteLine("deleting this ", id);
            using HttpResponseMessage response = await _entityClient.DeleteAsync(_url + $"/{id}");
            Trace.WriteLine("[RestClient]: Delete entity Done ");
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Makes a "DELETE" call to our Azure function APIs to delete all entities.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task DeleteEntitiesAsync()
        {
            Trace.WriteLine("[RestClient]: Delete entities called ");
            Debug.WriteLine("deleting all ");
            using HttpResponseMessage response = await _entityClient.DeleteAsync(_url);
            Trace.WriteLine("[RestClient]: Delete entities done ");
            response.EnsureSuccessStatusCode();
        }
    }

}
