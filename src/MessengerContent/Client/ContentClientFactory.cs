﻿/******************************************************************************
 * Filename    = ContentClientFactory.cs
 *
 * Author      = Rapeti Siddhu Neehal
 *
 * Product     = Messenger
 * 
 * Project     = MessengerContent
 *
 * Description = factory for getting the instance of ContentClient
 *****************************************************************************/
using System;

namespace MessengerContent.Client
{
    public class ContentClientFactory
    {
        // initializing in a thread safe way using Lazy<>
        private static readonly Lazy<ContentClient> s_contentClient = new(() => new ContentClient());

        /// <summary>
        /// Creates a client side content manager that will live until the end of the program
        /// </summary>
        /// <returns>ContentClient object which implements IContentClient interface</returns>
        public static IContentClient GetInstance()
        {
            return s_contentClient.Value;
        }
    }
}
