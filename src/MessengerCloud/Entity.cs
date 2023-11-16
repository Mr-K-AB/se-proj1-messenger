/******************************************************************************
 * Filename    = Entity.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = CloudProgrammingDemo
 * 
 * Project     = ServerlessFunc
 *
 * Description = Defines a custom Azure Table Entity.
 *****************************************************************************/

using Azure;
using MessengerCloud;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace MessengerCloud
{
    /// <summary>
    /// Custom Azure Table Entity.
    /// </summary>
    public class Entity : ITableEntity
    {
        public const string PartitionKeyName = "EntityPartitionKey";

        public Entity(EntityInfoWrapper info)
        {
            if(info == null){return; }
            PartitionKey = PartitionKeyName;
            RowKey = info.SessionId;
            Id = RowKey;
            SessionId = info.SessionId;
            Timestamp = DateTime.Now;
            PositiveChatCount = info.PositiveChatCount;
            NegativeChatCount = info.NegativeChatCount;
            IsOverallSentimentPositive = info.IsOverallSentimentPositive;
            Sentences = info.Sentences;
            Analysis = info.Analysis;
        }

        public Entity()   { }

        [JsonInclude]
        [JsonPropertyName("SessionId")] //Unique id for the session conducted
        public string SessionId { get; set; }


        [JsonInclude]
        [JsonPropertyName("Sentences")]
        public List<string> Sentences;

        [JsonInclude]
        [JsonPropertyName("PositiveChatCount")]
        public int PositiveChatCount { get; set; }
        [JsonInclude]
        [JsonPropertyName("NegativeChatCount")]
        public int NegativeChatCount { get; set; }
        [JsonInclude]
        [JsonPropertyName("IsOverallSentinmentPositive")]
        public bool IsOverallSentimentPositive { get; set; }

        [JsonInclude]
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonInclude]
        [JsonPropertyName("PartitionKey")]
        public string PartitionKey { get; set; }

        [JsonInclude]
        [JsonPropertyName("RowKey")]
        public string RowKey { get; set; }

        [JsonInclude]
        [JsonPropertyName("Timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonInclude]
        [JsonPropertyName("Analysis")]
        public AnalysisCloud Analysis { get; set; }

        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
