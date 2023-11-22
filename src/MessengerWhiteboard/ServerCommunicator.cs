using System.Diagnostics;
using System.Windows.Shapes;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;
using MessengerWhiteboard.Interfaces;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public class ServerCommunicator : IServerCommunicator
    {
        private static ServerCommunicator s_instance;
        private static Serializer s_serializer;
        private static readonly string s_moduleID = "whiteboard";
        private static ICommunicator s_communicator;

        public static ServerCommunicator Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ServerCommunicator();
                    s_serializer = new Serializer();
                    s_communicator = Factory.GetInstance();
                    s_communicator.AddSubscriber(s_moduleID, ViewModel.Instance);
                }
                return s_instance;
            }
        }

        public void Broadcast(WBShape wBShape, string? userID = null)
        {
            try
            {
                string serializedShape = s_serializer.SerializeWBShape(wBShape);
                Debug.Print("Broadcast: WBShape {0}", serializedShape);
                s_communicator.Broadcast(s_moduleID, serializedShape);
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
                //Trace.WriteLine("Broadcast List<ShapeItem>: {0}", shapes.Count.ToString());
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
                //Trace.WriteLine("Broadcast ShapeItem: {0}", shape.ShapeType);
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
