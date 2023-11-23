using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Client;
using MessengerContent.DataModels;
using MessengerContent.Server;

namespace MessengerTests.DashboardTests.Mocks
{
    internal class MockContentServer : IContentServer
    {

        List<ChatThread> _chatThreads = new ();

        public List<ChatThread> GetAllMessages()
        {
            return _chatThreads;
        }

        public void ServerSubscribe(IMessageListener subscriber)
        {
        }

        public void SetChats(List<ChatThread> chatThreads)
        {
            _chatThreads = chatThreads;
        }
    }
}
