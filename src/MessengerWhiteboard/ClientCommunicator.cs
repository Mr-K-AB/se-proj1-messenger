using System.Diagnostics;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;
using MessengerWhiteboard.Interfaces;
using MessengerWhiteboard.Models;

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
                    s_communicator = CommunicationFactory.GetCommunicator();
                    s_communicator.Subscribe(s_moduleID, ViewModel.Instance);
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
                s_communicator.Send(serializedShape, s_moduleID, null);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
        }


    }
}
