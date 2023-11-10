/******************************************************************************
* Filename    = ContentServerFactory.cs
*  
* Author      = Manikanta Gudipudi
* 
* Product     = Messenger
* 
* Project     = MessengerContent
* 
* Description = Factory design Pattern implementation over ContentServer.
*****************************************************************************/

using System;

namespace MessengerContent.Server
{
    public class ContentServerFactory
    {
        // initializing in a thread safe way using Lazy<>
        private static readonly Lazy<ContentServer> s_contentServer = new(() => new ContentServer());

        /// <summary>
        /// Creates a Server side content manager that will live until the end of the program
        /// </summary>
        /// <returns>ContentServer object which implements IContentServer interface</returns>
        public static IContentServer GetInstance()
        {
            return s_contentServer.Value;
        }
    }
}
