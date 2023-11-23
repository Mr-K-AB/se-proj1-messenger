/******************************************************************************
* Filename    = ScreenshareServer.cs
*
* Author      = M Anish Goud
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description =  Represents the ScreenshareServer responsible for managing shared client screens and handling communication with clients.
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using MessengerNetworking.NotificationHandler;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;
using System.Drawing.Printing;
using System.Security.Cryptography;

namespace MessengerScreenshare.Server
{
    public class ScreenshareServer: ITimer,
       INotificationHandler,
        IDisposable
    {
        private bool _disposedValue;
        private static readonly object s_lockObject = new ();
        private readonly Dictionary<int, SharedClientScreen> _subscribers;
        
        private readonly IDataReceiver _receiver;
        private readonly ICommunicator? _communicator;
        private static ScreenshareServer? s_instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenshareServer"/> class.
        /// </summary>
        /// <param name="receiver">The data receiver implementation.</param>
        /// <param name="isDebugging">A flag indicating whether the application is in debugging mode.</param>
        protected ScreenshareServer(IDataReceiver receiver,bool isDebugging)
        {
            if (!isDebugging)
            {
                
                _communicator = CommunicationFactory.GetCommunicator(false);
                _communicator.Subscribe(Utils.ServerIdentifier, this);
            }

            // Initialize the rest of the fields.
            _subscribers = new Dictionary<int, SharedClientScreen>();
            _disposedValue = false;
            _receiver = receiver;
            Trace.WriteLine(Utils.GetDebugMessage("created the instance for ScreenshareServer", withTimeStamp: true));
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="ScreenshareServer"/> class.
        /// </summary>
        ~ScreenshareServer()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// Gets the singleton instance of the ScreenshareServer.
        /// </summary>
        /// <param name="receiver">The data receiver implementation.</param>
        /// <param name="isDebugging">A flag indicating whether the application is in debugging mode.</param>
        /// <returns>The singleton instance of the ScreenshareServer.</returns>
        public static ScreenshareServer GetInstance(IDataReceiver receiver, bool isDebugging = false)
        {
            Debug.Assert(receiver != null, Utils.GetDebugMessage("receiver is found null"));

            // Create a new instance if it was null before.
            if (s_instance == null)
            {
                lock (s_lockObject) // Use the defined lockObject for synchronization.
                {
                    s_instance ??= new ScreenshareServer(receiver,isDebugging);
                }
            }

            return s_instance;
        }

        /// <summary>
        /// Handles the data received event.
        /// </summary>
        /// <param name="packetData">The data received in the packet.</param>
        public void OnDataReceived(string packetData)
        {
            try
            {
                DataPacket? packet = JsonSerializer.Deserialize<DataPacket>(packetData);

                if (packet != null)
                {
                    int clientId = packet.Id;
                    string clientName = packet.Name;
                    ClientDataHeader header = Enum.Parse<ClientDataHeader>(packet.Header);
                    string clientData = packet.Data;


                        if (header == ClientDataHeader.Register)
                        {
                            RegisterClient(clientId, clientName);
                        }
                        else if (header == ClientDataHeader.Deregister)
                        {
                            DeregisterClient(clientId);
                        }
                        else if (header == ClientDataHeader.Image)
                        {
                            PutImage(clientId, clientData);
                        }
                        else if (header == ClientDataHeader.Confirmation)
                        {
                            UpdateTimer(clientId);
                        }
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while processing the packet: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Registers a client with the specified ID and name.
        /// </summary>
        /// <param name="clientId">The ID of the client to register.</param>
        /// <param name="clientName">The name of the client to register.</param>
        private void RegisterClient(int clientId, string clientName)
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));

            lock (_subscribers)
            {
                if (!_subscribers.TryAdd(clientId, new (clientId, clientName, this)))
                {
                    // If TryAdd fails, the client is already registered.
                    Trace.WriteLine(Utils.GetDebugMessage($"Trying to register an already registered client with id {clientId}", withTimeStamp: true));
                    return; 
                }
            }
            NotifyUX();
            NotifyUX(clientId, clientName, start: true);

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully registered the client - Id: {clientId}, Name: {clientName}", withTimeStamp: true));
        }

        /// <summary>
        /// Deregisters a client with the specified ID.
        /// </summary>
        /// <param name="clientId">The ID of the client to deregister.</param>
        private void DeregisterClient(int clientId)
        {

            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen? client))
                {
                    _subscribers.Remove(clientId);
                    NotifyUX();
                    NotifyUX(clientId, client.Name, start: false);

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
                        client.Dispose(); // Dispose of the client's resources.
                    }

                    Trace.WriteLine(Utils.GetDebugMessage($"Successfully removed the client with Id {clientId}", withTimeStamp: true));
                }
                else
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"This client with id {clientId} which is not present in subscribers list", withTimeStamp: true));
                }
            }
        }

        /// <summary>
        /// checks if  client is present or not.if present sends the image data to SharedClientScreen.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="data">The image data received from the client.</param>
        private void PutImage(int clientId, string data)
        {
            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen? client))
                {
                    try
                    {
                        client.PutImage(data, client.TaskId);
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

        /// <summary>
        /// Broadcasts a message to multiple clients present in clientIds list.
        /// </summary>
        /// <param name="clientIds">The IDs of the clients to broadcast to.</param>
        /// <param name="headerVal">The header value for the broadcast.</param>
        /// <param name="numRowsColumns">The number of rows and columns in the broadcast.</param>
        public void BroadcastClients(List<int> clientIds, string headerVal, (int Rows, int Cols) numRowsColumns)
        {
            if (_communicator == null)
            {
                Trace.WriteLine(Utils.GetDebugMessage("_communicator is found null", withTimeStamp: true));
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

                var packet = new DataPacket(1, "Server", serverDataHeader.ToString(), JsonSerializer.Serialize(product));
                string packedData = JsonSerializer.Serialize(packet);

                foreach (int clientId in clientIds)
                {
                    _communicator.Send(packedData, Utils.ClientIdentifier, clientId.ToString());
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while sending the packet to the client: {e.Message}", withTimeStamp: true));
            }
        }

        /// <summary>
        /// Updates the timer for a specific client and broadcasts the confirmation packet to client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        private void UpdateTimer(int clientId)
        {
            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen? client))
                {
                    try
                    {
                        client.UpdateTimer();
                        BroadcastClients(new List<int> { clientId }, nameof(ServerDataHeader.Confirmation), (0, 0));

                        Trace.WriteLine(Utils.GetDebugMessage($"Timer updated for the client with Id: {clientId}", withTimeStamp: true));
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(Utils.GetDebugMessage($"Failed to update the timer for the client with id {clientId}: {e.Message}", withTimeStamp: true));
                    }
                }
                else
                {
                    Trace.WriteLine(Utils.GetDebugMessage($"Client with id {clientId} is not there in the subscribers list", withTimeStamp: true));
                }
            }
        }

        /// <summary>
        /// Handles the timeout event for a client, If timeout occurs it deregister the client.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="clientId">The ID of the client.</param>
        /// <param name="e">The ElapsedEventArgs.</param>
        public void OnTimeOut(object? source, int clientId, ElapsedEventArgs e)
        {

            DeregisterClient(clientId);
            Trace.WriteLine(Utils.GetDebugMessage($"Timeout occurred for the client with id: {clientId} and deregistered", withTimeStamp: true));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    List<SharedClientScreen> sharedClientScreens;
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
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private void NotifyUX()
        {
            Debug.Assert(_subscribers != null, Utils.GetDebugMessage("_subscribers is found null"));
            Debug.Assert(_receiver != null, Utils.GetDebugMessage("_receiver is found null"));

            List<SharedClientScreen> sharedClientScreens;

          
            lock (_subscribers)
            {
                sharedClientScreens = _subscribers.Values.ToList();
            }

            _receiver.OnSubscribersUpdated(sharedClientScreens);
        }
        private void NotifyUX(int clientId, string clientName, bool start)
        {
            if (start)
            {
                _receiver.OnScreenshareStart(clientId, clientName);
            }
            else
            {
                _receiver.OnScreenshareStop(clientId, clientName);
            }
        }
        public void OnClientJoined(string ipAddress, int port) {}

        public void OnClientLeft(string ipAddress, int port) {}
    }
}
