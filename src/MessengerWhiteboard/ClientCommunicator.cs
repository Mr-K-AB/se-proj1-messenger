using System.Diagnostics;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;

namespace MessengerWhiteboard
{
    public class ClientCommunicator : IClientCommunicator
    {
        private static ClientCommunicator s_instance;
        private static Serializer s_serializer;
        private static ICommunicator s_communicator;
        private static readonly string s_moduleID = "whiteboard";
        private static Tuple<string, int> s_serverInfo;

        public static ClientCommunicator Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ClientCommunicator();
                    s_serializer = new Serializer();
                    s_communicator = Factory.GetInstance();
                    s_communicator.AddSubscriber(s_moduleID, ViewModel.Instance);
                }
                return s_instance;
            }
        }

        public void InitializeServerInfo(string ipAddr, int port)
        {
            s_serverInfo = new Tuple<string, int>(ipAddr, port);
        }

        public void SendToServer(WBShape shape)
        {
            try
            {
                string serializedShape = s_serializer.SerializeWBShape(shape);
                Debug.Print("ClientCommunicator.SendToServer: {0}", serializedShape);
                s_communicator.Broadcast(s_moduleID, serializedShape);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
        }


    }
}
