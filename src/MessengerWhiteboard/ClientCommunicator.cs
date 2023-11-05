using System.Diagnostics;

namespace MessengerWhiteboard
{
    public class ClientCommunicator : IClientCommunicator
    {
        private static ClientCommunicator s_instance;
        private static Serializer s_serializer;
        //private static ICommunicator s_communicator;

        public static ClientCommunicator Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ClientCommunicator();
                    s_serializer = new Serializer();
                    //s_communicator = Communicator.Instance;
                }
                return s_instance;
            }
        }

        public void SendToServer(WBShape shape)
        {
            try
            {
                string serializedShape = s_serializer.SerializeWBShape(shape);
                //s_communicator.Send(serializedShape);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                throw;
            }
        }


    }
}
