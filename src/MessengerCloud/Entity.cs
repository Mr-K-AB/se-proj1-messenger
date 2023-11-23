/******************************************************************************
* Filename    = Entity.cs
*
* Author      = Shubh Pareek
*
* Product     = Messenger 
* 
* Project     = MessengerCloud
*
* Description = A custom Azure Table Entity.
*****************************************************************************/

using Azure;
using MessengerCloud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (info == null) { return; }
            PartitionKey = PartitionKeyName;
            RowKey = info.SessionId;
            Id = RowKey;
            SessionId = info.SessionId;
            Timestamp = DateTime.Now;
            PositiveChatCount = info.PositiveChatCount;
            NegativeChatCount = info.NegativeChatCount;
            NeutralChatCount = info.NeutralChatCount;
            OverallSentiment = info.OverallSentiment;
            Sentences = info.Sentences;
            Analysis = info.Analysis;
            Trace.WriteLine("[Entity]: new entity created ");
        }

        public Entity() { }

        [JsonInclude]
        [JsonPropertyName(nameof(SessionId))] //Unique id for the session conducted
        public string SessionId { get; set; }


        [JsonInclude]
        [JsonPropertyName(nameof(Sentences))]
        public List<string> Sentences { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(PositiveChatCount))]
        public int PositiveChatCount { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(NegativeChatCount))]
        public int NegativeChatCount { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(NeutralChatCount))]
        public int NeutralChatCount { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(OverallSentiment))]
        public string OverallSentiment { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(Id))]
        public string Id { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(PartitionKey))]
        public string PartitionKey { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(RowKey))]
        public string RowKey { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(Timestamp))]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(Analysis))]
        public Analysis Analysis { get; set; }

        [JsonIgnore]
        public ETag ETag { get; set; }
    }
}
