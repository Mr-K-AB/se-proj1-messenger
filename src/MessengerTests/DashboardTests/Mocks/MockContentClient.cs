/******************************************************************************
* Filename    = MockContentClient.cs
*
* Author      = Pratham Ravindra Nagpure 
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerTests
*
* Description = A class for mocking the content client.
*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client;
using MessengerContent.Client;
using MessengerContent.DataModels;

namespace MessengerTests.DashboardTests.Mocks
{
    internal class MockContentClient : IContentClient
    {
        public void ClientDelete(int messageID)
        {
        }

        public void ClientDownload(int messageID, string savePath)
        {
        }

        public void ClientEdit(int messageID, string newMessage)
        {
        }

        public ChatThread ClientGetThread(int threadID)
        {
            return new ChatThread();
        }

        public void ClientSendData(SendChatData chatData)
        {
        }

        public void ClientStar(int messageID)
        {
        }

        public void ClientSubscribe(IMessageListener subscriber)
        {
        }

        public string GetIP()
        {
            return "0.0.0.0";
        }

        public int GetPort()
        {
            return 0;
        }

        public int GetUserID()
        {
            return 1;
        }

        public string GetUserName()
        {
            return "Name";
        }

        public void SetUser(int id, string name)
        {
        }
    }
}
