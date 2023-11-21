/******************************************************************************
* Filename    = EntityInfoWrapper.cs
*
* Author      = Shubh Pareek
*
* Roll Number = 112001039
*
* Product     = Messenger 
* 
* Project     = MessengerCloud
*
* Description = A class for Azure functions.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerCloud
{
    public class EntityInfoWrapper
    {
        public EntityInfoWrapper(List<string> sentences, int positiveChatCount, int negativeChatCount, int neutralChatCount, string overallSentiment, string sessionId, Analysis obj)
        {

            Sentences = sentences;
            PositiveChatCount = positiveChatCount;
            NegativeChatCount = negativeChatCount;
            NeutralChatCount = neutralChatCount;
            OverallSentiment = overallSentiment;
            SessionId = sessionId;
            Analysis = obj;
            Trace.WriteLine("[EntityInfoWrapper]: wrapper created");
        }
        public EntityInfoWrapper() { }

        [JsonInclude]
        [JsonPropertyName(nameof(SessionId))] //Unique id for the session conducted
        public string SessionId { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(Sentences))]
        public List<string> Sentences;

        [JsonInclude]
        [JsonPropertyName(nameof(PositiveChatCount))]
        public int PositiveChatCount { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(NegativeChatCount))]
        public int NegativeChatCount { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(NeutralChatCount))]
        public int NeutralChatCount{ get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(OverallSentiment))]
        public string OverallSentiment { get; set; }

        [JsonInclude]
        [JsonPropertyName(nameof(Analysis))]
        public Analysis Analysis { get; set; }

    }
}
