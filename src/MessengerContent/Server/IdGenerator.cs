/******************************************************************************
* Filename    = IdGenerator.cs
*
* Author      = Manikanta Gudipudi
*
* Product     = Messenger
* 
* Project     = MessengerContent
*
* Description = This Class generates unique Ids for message and chat thread and also allows resetting them.
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerContent.Server
{
    public class IdGenerator
    {
        private static int s_msgId;
        private static int s_chatContentId;

        /// <summary>
        /// Generates message id for the message and increment the id.
        /// </summary>
        /// <returns>Returns the generated unique id.</returns>
        public static int GetMsgId()
        {
            int id = s_msgId;
            s_msgId++;
            return id;
        }

        /// <summary>
        /// Reset the message id to 0.
        /// </summary>
        /// <returns>Returns the generated unique id.</returns>
        public static void ResetMsgId()
        {
            s_msgId = 0;
        }

        /// <summary>
        /// Generates ids for chat threads.
        /// </summary>
        /// <returns>Returns the generated id.</returns>
        public static int GetChatId()
        {
            int id = s_chatContentId;
            s_chatContentId++;
            return id;
        }

        /// <summary>
        /// Reset the chat thread id.
        /// </summary>
        public static void ResetChatId()
        {
            s_chatContentId = 0;
        }
    }
}
