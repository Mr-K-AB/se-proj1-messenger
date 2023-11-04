using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace MessengerScreenshare.Server
{
    public class ScreenshareServer: ITimer,
       //INotificationHandler, // To receive chat messages from clients.
             // Handles client timeouts.
        IDisposable
    {
        private  static ScreenshareServer? s_instance;
        private bool _disposedValue;
        private readonly IDataReceiver _receiver;   // To notify changes to the UI.
        private static readonly object s_lockObject = new ();
        private readonly Dictionary<string, SharedClientScreen> _subscribers;
        private readonly bool _disposed;
        protected ScreenshareServer(IDataReceiver listener, bool isDebugging)
        {
            if (!isDebugging)
            {
                // Get an instance of a communicator object.
               

                // Subscribe to the networking module for packets.
               
            }

            // Initialize the rest of the fields.
            _subscribers = new Dictionary<string, SharedClientScreen>();
            _receiver = listener;
            _disposedValue = false;

            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenshareServer", withTimeStamp: true));
        }
        ~ScreenshareServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }
        public static ScreenshareServer GetInstance(IDataReceiver listener, bool isDebugging = false)
        {
            Debug.Assert(listener != null, Utils.GetDebugMessage("listener is found null"));

            // Create a new instance if it was null before.
            if (s_instance == null)
            {
                lock (s_lockObject) // Use the defined lockObject for synchronization.
                {
                    s_instance ??= new ScreenshareServer(listener, isDebugging );
                }
            }

            return s_instance;
        }
        public void OnDataReceived(string packetData)
        {
            try
            {
                DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(packetData);

                if (packet == null)
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Not able to deserialize data packet: {packetData}", withTimeStamp: true));
                    return;
                }

                string clientId = packet.Id;
                string clientName = packet.Name;
                ClientDataHeader header = Enum.Parse<ClientDataHeader>(packet.Header);
                string clientData = packet.Data;

                // Create a dictionary to map packet headers to actions.
                Dictionary<ClientDataHeader, Action> headerActions = new()
                {       
                    { ClientDataHeader.Register, () => RegisterClient(clientId, clientName) },
                    { ClientDataHeader.Deregister, () => DeregisterClient(clientId) },
                    { ClientDataHeader.Image, () => PutImage(clientId, clientData) },
                    { ClientDataHeader.Confirmation, () => UpdateTimer(clientId) },
                };

                if (headerActions.TryGetValue(header, out Action? action))
                {
                    action.Invoke();
                }
                else
                {
                    throw new Exception($"Unknown header {packet.Header}");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while processing the packet: {e.Message}", withTimeStamp: true));
            }
        }
        private void RegisterClient(string clientId, string clientName)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            lock (_subscribers)
            {
                if (!_subscribers.TryAdd(clientId, new (clientId, clientName, this)))
                {
                    // If TryAdd fails, the client is already registered.
                    Trace.WriteLine(Utils.GetDebugMessage($"Trying to register an already registered client with id {clientId}", withTimeStamp: true));
                    return; // Early exit.
                }
            }

            //NotifyUX();
            //NotifyUX(clientId, clientName, hasStarted: true);

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully registered the client - Id: {clientId}, Name: {clientName}", withTimeStamp: true));
        }

        private void DeregisterClient(string clientId)
        {

            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen client))
                {
                    _subscribers.Remove(clientId);

                    //NotifyUX();
                    //NotifyUX(clientId, client.Name, hasStarted: false);

                    try
                    {
                        client.StopProcessing();
                    }
                    catch (OperationCanceledException e)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage($"Task canceled for the client with id {clientId}: {e.Message}", withTimeStamp: true));
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage($"Failed to stop the task for the removed client with id {clientId}: {e.Message}", withTimeStamp: true));
                    }
                    finally
                    {
                        //client.Dispose(); // Dispose of the client's resources.
                    }

                    Trace.WriteLine(Utils.GetDebugMessage($"Successfully removed the client with Id {clientId}", withTimeStamp: true));
                }
                else
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Trying to deregister a client with id {clientId} which is not present in subscribers list", withTimeStamp: true));
                }
            }
        }
        private void PutImage(string clientId, string image)
        {
            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen client))
                {
                    try
                    {
                        client.PutImage(image, client.TaskId);
                        Trace.WriteLine(Utils.GetDebugMessage($"Successfully received image of the client with Id: {clientId}", withTimeStamp: true));
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage($"Exception while processing the received image: {e.Message}", withTimeStamp: true));
                    }
                }
                else
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Client with id {clientId} is not present in subscribers list", withTimeStamp: true));
                }
            }
        }
        public void BroadcastClients(List<string> clientIds, string headerVal, (int Rows, int Cols) numRowsColumns)
        {
            //if (_communicator == null)
            //{
              //  Trace.WriteLine(Utils.GetDebugMessage("_communicator is found null", withTimeStamp: true));
                //return;
            //}

            if (clientIds == null)
            {
                Trace.WriteLine(Utils.GetDebugMessage("List of client Ids is found null", withTimeStamp: true));
                return;
            }

            // If there are no clients to broadcast to, return early.
            if (clientIds.Count == 0)
            {
                return;
            }

            if (!Enum.TryParse(headerVal, out ServerDataHeader serverDataHeader))
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to parse the header {headerVal}", withTimeStamp: true));
                return;
            }

            try
            {
                int product = numRowsColumns.Rows * numRowsColumns.Cols;

                var packet = new DataPacket("1", "Server", serverDataHeader.ToString(), JsonSerializer.Serialize(product));
                string packedData = JsonSerializer.Serialize(packet);

                //foreach (string clientId in clientIds)
               // {
                   // _communicator.Send(packedData, Utils.ModuleIdentifier, clientId);
                //}
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while sending the packet to the client: {e.Message}", withTimeStamp: true));
            }
        }

        private void UpdateTimer(string clientId)
        {
            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen client))
                {
                    try
                    {
                        client.UpdateTimer();
                        BroadcastClients(new List<string> { clientId }, nameof(ServerDataHeader.Confirmation), (0, 0));

                        Trace.WriteLine(Utils.GetDebugMessage($"Timer updated for the client with Id: {clientId}", withTimeStamp: true));
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage($"Failed to update the timer for the client with id {clientId}: {e.Message}", withTimeStamp: true));
                    }
                }
                else
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Client with id {clientId} is not present in subscribers list", withTimeStamp: true));
                }
            }
        }


        public void OnTimeOut(object? source, string clientId, ElapsedEventArgs e)
        {

            DeregisterClient(clientId);
            Trace.WriteLine(Utils.GetDebugMessage($"Timeout occurred for the client with id: {clientId}", withTimeStamp: true));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    List<SharedClientScreen> sharedClientScreens;

                    // Acquire lock because timer threads could also execute simultaneously.
                    lock (_subscribers)
                    {
                        sharedClientScreens = _subscribers.Values.ToList();
                        _subscribers.Clear();
                    }

                    // Deregister all the clients.
                    foreach (SharedClientScreen client in sharedClientScreens)
                    {
                        DeregisterClient(client.Id);
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ScreenshareServer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
