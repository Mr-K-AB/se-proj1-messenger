using MessengerNetworking.Communicator;

namespace MessengerWhiteboard
{
    public class ServerCommunicator : IServerCommunicator
    {
        private static ServerCommunicator s_instance;
        private static Serializer s_serializer;
        //private static ICommunicator s_communicator;

        public static ServerCommunicator Instance
        {
            get
            {
                if (s_instance == null)
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
                string serializedShape = s_serializer.SerializeWBShape(wBShape);
                //s_communicator.Send(serializedShape);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Broadcast(List<ShapeItem> shapes, Operation op)
        {
            try
            {
                List<SerializableShapeItem> serializedShapes = s_serializer.SerializeShapes(shapes);
                WBShape wBShape = new(serializedShapes, op);
                Broadcast(wBShape);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Broadcast(ShapeItem shape, Operation op)
        {
            try
            {
                List<ShapeItem> shapes = new()
                {
                    shape
                };
                Broadcast(shapes, op);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
