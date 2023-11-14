using System.Diagnostics;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
using MessengerNetworking.NotificationHandler;

namespace MessengerWhiteboard
{
    public partial class ViewModel : INotificationHandler
    {
        private Dispatcher MainThreadDispatcher => (Application.Current?.Dispatcher != null) ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;
        public void OnDataReceived(string data)
        {
            Trace.WriteLine("OnDataReceived: {0}", data);
            _ = MainThreadDispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action<string>(serializedData =>
                {
                    lock (this)
                    {
                        HandleData(serializedData);
                    }
                })
                , data);

        }

        public void HandleData(string data)
        {
            Serializer serializer = new();
            ServerState serverState = ServerState.Instance;

            if(isServer)
            {
                try
                {
                    WBShape deserializedData = serializer.DeserializeWBShape(data);
                    List<ShapeItem> shapes = serializer.DeserializeShapes(deserializedData.ShapeItems);
                    switch(deserializedData.Op)
                    {
                        case Operation.Creation:
                            //foreach(ShapeItem shape in shapes)
                            //{
                            Trace.WriteLine("HandleData: {0}", shapes[0].Geometry.ToString());
                            CreateIncomingShape(shapes[0]);
                            serverState.OnShapeReceived(shapes[0], Operation.Creation);
                            //}
                            break;
                        case Operation.ModifyShape:
                            foreach(ShapeItem shape in shapes)
                            {
                                ModifyIncomingShape(shape);
                                serverState.OnShapeReceived(shape, Operation.ModifyShape);
                            }
                            break;
                        case Operation.Deletion:
                            foreach(ShapeItem shape in shapes)
                            {
                                DeleteIncomingShape(shape);
                                serverState.OnShapeReceived(shape, Operation.Deletion);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    throw;
                }
            }
            else
            {
                try
                {
                    WBShape deserializedData = serializer.DeserializeWBShape(data);
                    List<ShapeItem> shapes = serializer.DeserializeShapes(deserializedData.ShapeItems);
                    switch (deserializedData.Op)
                    {
                        case Operation.Creation:
                            foreach (ShapeItem shape in shapes)
                            {
                                CreateIncomingShape(shape);
                                serverState.OnShapeReceived(shape, Operation.Creation);
                            }
                            break;
                        case Operation.ModifyShape:
                            foreach (ShapeItem shape in shapes)
                            {
                                ModifyIncomingShape(shape);
                                serverState.OnShapeReceived(shape, Operation.ModifyShape);
                            }
                            break;
                        case Operation.Deletion:
                            foreach (ShapeItem shape in shapes)
                            {
                                DeleteIncomingShape(shape);
                                serverState.OnShapeReceived(shape, Operation.Deletion);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                    throw;
                }

            }
        }

        public void OnClientJoined(string ipAddr, int port)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string ipAddr, int port)
        {
            throw new NotImplementedException();
        }
    }
}
