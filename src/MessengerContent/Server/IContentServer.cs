/******************************************************************************
 * Filename    = IContentServer.cs
 *
 * Author      = Manikanta Gudipudi
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = Interface for ContentServer
 *****************************************************************************/

using Messenger.Client;
using MessengerContent.Client;
using MessengerContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerContent.Server
{
    public interface IContentServer
    {
        /// <summary>
        /// Add a new subscriber to the list of subscribers
        /// </summary>
        /// <param name="subscriber">IContentListener implementation provided by the subscriber</param>
        //void ServerSubscribe(IMessageListener subscriber);

        /// <summary>
        /// Get all the messages sent
        /// </summary>
        /// <returns>List of Thread objects</returns>
        List<ChatThread> GetAllMessages();

        /// <summary>
        /// Sends all the messages to the client of the user with user id = userId
        /// </summary>
        /// <param name="userId">user id of the user to which messages needs to be sent</param>
        //void SendAllMessagesToClient(int userId);
    }
}
