﻿using System;
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
       INotificationHandler, // To receive chat messages from clients.
            // Handles client timeouts.
        IDisposable
    {
        private bool _disposedValue;
        private static readonly object s_lockObject = new ();
        private readonly Dictionary<int, SharedClientScreen> _subscribers;
        private readonly bool _disposed;
        private readonly IDataReceiver _receiver;
        private readonly ICommunicator _communicator;
        private static ScreenshareServer? s_instance;

        protected ScreenshareServer(IDataReceiver receiver,bool isDebugging)
        {
            if (!isDebugging)
            {
                // Get an instance of a communicator object.
                _communicator = CommunicationFactory.GetCommunicator(false);

                // Subscribe to the networking module for packets.
               _communicator.Subscribe(Utils.ServerIdentifier, this);
            }

            // Initialize the rest of the fields.
            _subscribers = new Dictionary<int, SharedClientScreen>();
            _disposedValue = false;
            _receiver = receiver;
            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenshareServer", withTimeStamp: true));
        }
        ~ScreenshareServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }

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

                int clientId = packet.Id;
                string clientName = packet.Name;
                ClientDataHeader header = Enum.Parse<ClientDataHeader>(packet.Header);
                int imgCount = packet.ImgCount;
                int fragmentOffset = packet.Offset;
                string clientData = packet.Data;
                Trace.WriteLine(Utils.GetDebugMessage(header.ToString()));
                switch (header)
                {
                    case ClientDataHeader.Register:
                        RegisterClient(clientId, clientName);
                        break;
                    case ClientDataHeader.Deregister:
                        DeregisterClient(clientId);
                        break;
                    case ClientDataHeader.Image:
                        PutImage(clientId, clientData);
                        break;
                    case ClientDataHeader.Confirmation:
                        UpdateTimer(clientId);
                        break;
                    default:
                        throw new Exception($"Unknown header {packet.Header}");
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Exception while processing the packet: {e.Message}", withTimeStamp: true));
            }
        }


        /*private StringBuilder _receivedDataBuffer = new StringBuilder();

        // Assuming this method is called when data is received from a client
        private void OnDataReceived(string receivedFragment)
        {
            _receivedDataBuffer.Append(receivedFragment);

            // Check if the complete data has been received
            if (IsCompleteDataReceived(_receivedDataBuffer.ToString()))
            {
                // Process the complete data
                string completeData = _receivedDataBuffer.ToString();
                DataPacket dataPacket = JsonSerializer.Deserialize<DataPacket>(completeData);

                // Your analysis logic here using dataPacket
                // ...

                // Clear the buffer for the next set of fragments
                _receivedDataBuffer.Clear();
            }
        }

        private bool IsCompleteDataReceived(string data)
        {
            // Your logic to determine if the complete data has been received
            // For simplicity, you can check if the data ends with a specific marker or pattern
            return data.EndsWith("EndOfDataMarker");
        }
        */


        private void RegisterClient(int clientId, string clientName)
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
            //DataPacket confirmationPacket = new(clientId, clientName, ServerDataHeader.Send.ToString(), 0, 0, "");
            //string serializedConfirmationPacket = JsonSerializer.Serialize(confirmationPacket);
            //_communicator.Broadcast(Utils.ServerIdentifier, serializedConfirmationPacket);
            NotifyUX();
            NotifyUX(clientId, clientName, start: true);

            Trace.WriteLine(Utils.GetDebugMessage($"Successfully registered the client - Id: {clientId}, Name: {clientName}", withTimeStamp: true));
        }

        private void DeregisterClient(int clientId)
        {

            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen? client))
                {
                    _subscribers.Remove(clientId);
                    //DataPacket confirmationPacket = new(clientId, clientName, ServerDataHeader.Stop.ToString(), 0, 0, "");
                    //string serializedConfirmationPacket = JsonSerializer.Serialize(confirmationPacket);
                    //_communicator.Broadcast(Utils.ServerIdentifier, serializedConfirmationPacket);
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
                    Trace.WriteLine(Utils.GetDebugMessage($"Trying to deregister a client with id {clientId} which is not present in subscribers list", withTimeStamp: true));
                }
            }
        }
        private void PutImage(int clientId, string data)
        {
            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen? client))
                {
                    try
                    {
                        /*// Dictionary to store image fragments for each image
                        client.ImageFragments ??= new Dictionary<int, StringBuilder>();

                        // If the image fragment container doesn't exist, create it
                        if (!client.ImageFragments.TryGetValue(imgCount, out StringBuilder imageFragmentContainer))
                        {
                            imageFragmentContainer = new StringBuilder();
                            client.ImageFragments[imgCount] = imageFragmentContainer;
                        }
                        if (!imageFragmentContainer.ToString().Contains($"|{fragmentOffset}|"))
                        {
                            // Concatenate the received image packet to the StringBuilder for the specific image
                            imageFragmentContainer.Append($"|{fragmentOffset}|{data}");

                            // Check if all fragments for the current image have been received
                            if (client.ImageFragments[imgCount].ToString().Split('|').Length - 1 == 500)
                            {
                                // Now, imageFragmentContainer contains the complete image as a single string
                                // You can send or process the complete image here

                                string completeImage = imageFragmentContainer.ToString().Replace($"|{fragmentOffset}|", "");

                                // Clear the StringBuilder for this image
                                client.PutImage(completeImage, client.TaskId);
                                client.ImageFragments[imgCount].Clear();


                                Trace.WriteLine(Utils.GetDebugMessage($"Successfully received and processed image of the client with Id: {clientId}, Image No: {imgCount}", withTimeStamp: true));
                            }
                            
                            else
                            {
                                Trace.WriteLine(Utils.GetDebugMessage($"Received image packet {fragmentOffset} of {imgCount} from the client with Id: {clientId}", withTimeStamp: true));
                            }


                        }
                        else
                        {
                            // Handle the case where the same fragmentOffset comes twice
                            Trace.WriteLine(Utils.GetDebugMessage($"Duplicate image packet {fragmentOffset} received from the client with Id: {clientId}", withTimeStamp: true));
                        }*/
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
        public void BroadcastClients(List<int> clientIds, string headerVal, (int Rows, int Cols) numRowsColumns)
        {
            if (_communicator == null)
            {
                Trace.WriteLine(Utils.GetDebugMessage("_communicator is found null", withTimeStamp: true));
                return;
            }

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

                var packet = new DataPacket(1, "Server", serverDataHeader.ToString(), 0, 0, JsonSerializer.Serialize(product));
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

        private void UpdateTimer(int clientId)
        {
            lock (_subscribers)
            {
                if (_subscribers.TryGetValue(clientId, out SharedClientScreen? client))
                {
                    try
                    {
                        client._timer!.Interval = 20000;
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
                    Trace.WriteLine(Utils.GetDebugMessage($"Client with id {clientId} is not present in subscribers list", withTimeStamp: true));
                }
            }
        }


        public void OnTimeOut(object? source, int clientId, ElapsedEventArgs e)
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

       

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
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
        public void OnClientJoined(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void OnClientLeft(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }
    }
}
