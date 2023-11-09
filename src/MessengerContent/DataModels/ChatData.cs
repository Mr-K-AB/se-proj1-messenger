/******************************************************************************
 * Filename    = ChatData.cs
 *
 * Author      = Manikanta
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = 
 *****************************************************************************/

using System.Diagnostics.CodeAnalysis;

namespace MessengerContent.DataModels
{
    [ExcludeFromCodeCoverage]
    public class ChatData : ReceiveChatData
    {
        /// <summary>
        /// file details. 
        /// </summary>
        public SendFileData FileData;

        /// <summary>
        /// Empty constructor to create type without parameters.
        /// </summary>
        public ChatData()
        {
        }

        /// <summary>
        /// Constructor to create type with parameters.
        /// </summary>
        /// <param name="receiveChatData">Instance of the ReceiveChatData class</param>
        public ChatData(ReceiveChatData receiveChatData)
        {
            Type = receiveChatData.Type;
            Data = receiveChatData.Data;
            MessageID = receiveChatData.MessageID;
            //ReceiverIDs = receiveChatData.ReceiverIDs;
            ReplyMessageID = receiveChatData.ReplyMessageID;
            ReplyThreadID = receiveChatData.ReplyThreadID;
            SenderID = receiveChatData.SenderID;
            SentTime = receiveChatData.SentTime;
            Starred = receiveChatData.Starred;
            Event = receiveChatData.Event;
        }

        /// <summary>
        /// gives a copy of ChatData object.
        /// </summary>
        /// <returns>Shallow copy of the object.</returns>
        public ChatData Copy()
        {
            return MemberwiseClone() as ChatData;
        }
    }
}
