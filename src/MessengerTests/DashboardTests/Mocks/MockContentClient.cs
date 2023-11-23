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
        public void ClientDelete(int messageID, string ip, int port)
        {
        }

        public void ClientDownload(int messageID, string savePath, string ip, int port)
        {
        }

        public void ClientEdit(int messageID, string newMessage, string ip, int port)
        {
        }

        public ChatThread ClientGetThread(int threadID)
        {
            return new ChatThread();
        }

        public void ClientSendData(SendChatData chatData, string ip, int port)
        {
        }

        public void ClientStar(int messageID, string ip, int port)
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

        public void SetUser(int id, string name, string ip, int port)
        {
        }
    }
}
