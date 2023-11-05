using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerNetworking.Communicator;

namespace MessengerWhiteboard
{
    public class ServerCommunicator : IServerCommunicator
    {
        private static ServerCommunicator s_instance;
        private static Serializer s_serializer;
        private static ICommunicator s_communicator;

        public static ServerCommunicator Instance
        {
            get
            {
                if(s_instance == null)
                {
                    s_instance = new ServerCommunicator();
                    s_serializer = new Serializer();
                    //s_communicator = new Communicator(); //TODO
                }
                return s_instance;
            }
        }

        public void Broadcast(WBShape wBShape, string? userID = null)
        {
            try
            {
                var serializedShape = s_serializer.SerializeShape(wBShape);
            }
        }

    }
}
