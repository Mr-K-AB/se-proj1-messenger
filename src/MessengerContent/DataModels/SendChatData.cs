/******************************************************************************
 * Filename    = SendChatData.cs
 *
 * Author      = Manikanta
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = data model for sending chat data
 *****************************************************************************/

using System.Diagnostics.CodeAnalysis;

namespace MessengerContent.DataModels
{
    [ExcludeFromCodeCoverage]
    public class SendChatData
    {
        /// <summary>
        /// Type of message - chat or file
        /// </summary>
        public MessageType Type;

        /// <summary>
        /// Content of message if chat type message.
        /// File path if file type message.
        /// </summary>
        public string Data;

        /// <summary>
        /// List containing the receiver IDs.
        /// Empty in case of broadcast message.
        /// </summary>
        //public int[]? ReceiverIDs;

        /// <summary>
        /// ID of message being replied to.
        /// </summary>
        public int ReplyMessageID;

        /// <summary>
        /// ID of thread to which the message belongs to.
        /// If the thread does not exist, -1.
        /// </summary>
        public int ReplyThreadID;

        /// <summary>
        /// Constructor to create type without parameters.
        /// </summary>
        public SendChatData()
        {
            Data = "";
            //ReceiverIDs = null;
            ReplyMessageID = -1;
            ReplyThreadID = -1;
        }
    }
}
