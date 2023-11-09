///<author>Alugonda Sathvik </author>
///<summary>
/// This file has ScreenshareClient class's implementation
/// In this file functions related to starting and stopping Screen Capturing 
/// are implemented
///</summary>
///
//using MessengerNetworking.Communicator;
//using MessengerNetworking.NotificationHandler;
using MessengerScreenshare.Client;
using MessengerScreenshare;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MessengerNetworking.Communicator;
using MessengerNetworking.Factory;
using MessengerNetworking.NotificationHandler;

namespace MessengerScreenshare.Client
{
    public class ScreenshareClient : IScreenshareClient, INotificationHandler
    {

        // ScreenshareClient Object
        private static ScreenshareClient? s_screenShareClient;

        // Object of networking module through which
        // we will be communicating to the server
        private readonly ICommunicator _communicator;

        // Threads for sending images and the confirmation packet
        private Task? _sendImageTask, _sendConfirmationTask;

        // Capturer and Processor Class Objects
        private readonly ScreenCapturer _capturer;
        private readonly ScreenProcessor _processor;

        // Name and Id of the current client user
        private string? _name;
        private string? _id;
        private bool _isExam;
        private readonly string _myIP;
        private readonly int _myPort;
        // Tokens added to be able to stop the thread execution
        private bool _confirmationCancellationToken;
        private readonly CancellationTokenSource? _imageCancellation;

        public bool IsExam { get => _isExam;
            set {
                if(_isExam != value)
                {
                    _isExam = value;
                }
            } }

        /// <summary>
        /// Setting up the ScreenCapturer and ScreenProcessor Class
        /// Taking instance of communicator from communicator factory
        /// and subscribing to it.
        /// </summary>
        public ScreenshareClient(bool isDebugging)
        {
            _capturer = new ScreenCapturer();
            _processor = new ScreenProcessor(_capturer);
            if (!isDebugging)
            {
                _communicator = Factory.GetInstance();
                _communicator.AddSubscriber(Utils.ModuleIdentifier, this);
                _myPort = _communicator.ListenPort;
                _myIP = _communicator.IpAddress;
            }
            Trace.WriteLine(Utils.GetDebugMessage("Successfully stopped image processing", withTimeStamp: true));
        }

        /// <summary>
        /// Gives an instance of ScreenshareClient class and that instance is always 
        /// the same i.e. singleton pattern.
        /// </summary>
        public static ScreenshareClient GetInstance(bool isDebugging = false)
        {
            s_screenShareClient ??= new ScreenshareClient(isDebugging);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully created an instance of ScreenshareClient", withTimeStamp: true));
            return s_screenShareClient;
        }

        /// <summary>
        /// When client clicks the screensharing button, this function gets executed
        /// It will send a register packet to the server and it will even start sending the
        /// confirmation packets to the sever
        /// </summary>
        public async Task StartScreensharingAsync()
        {
            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null"));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null"));

            DataPacket dataPacket = new(_id, _name, ClientDataHeader.Register.ToString(), "", _myPort, _myIP);
            string serializedData = JsonSerializer.Serialize(dataPacket);
            _communicator.Broadcast(Utils.ModuleIdentifier, serializedData);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully sent REGISTER packet to server", withTimeStamp: true));

            await SendConfirmationPacketAsync();
            Trace.WriteLine(Utils.GetDebugMessage("Started sending confirmation packet", withTimeStamp: true));

            await StartImageSendingAsync();
        }

        /// <summary>
        /// This function will be invoked on message from server
        /// If the message is SEND then start capturing, processing and sending functions
        /// Otherwise, if the message was STOP then just stop the image sending part
        /// </summary>
        /// <param name="serializedData"> Serialized data from the network module </param>
        public void OnDataReceived(string serializedData)
        {
            // Deserializing data packet received from server
            Debug.Assert(serializedData != "", Utils.GetDebugMessage("Message from serve found null", withTimeStamp: true));
            DataPacket? dataPacket = JsonSerializer.Deserialize<DataPacket>(serializedData);
            Debug.Assert(dataPacket != null, Utils.GetDebugMessage("Unable to deserialize datapacket from server", withTimeStamp: true));
            Trace.WriteLine(Utils.GetDebugMessage("Successfully received packet from server", withTimeStamp: true));

            if (dataPacket?.Header == ServerDataHeader.Send.ToString())
            {
                // If it is SEND packet then start image sending (if not already started) and 
                // Set the resolution as in the packet
                Trace.WriteLine(Utils.GetDebugMessage("Got SEND packet from server", withTimeStamp: true));

                // Starting capturer, processor and Image Sending
                Task.Run(async () => await StartImageSendingAsync());

                int windowCount = int.Parse(dataPacket.Data);
                _processor.SetNewResolution(windowCount);
                Trace.WriteLine(Utils.GetDebugMessage("Successfully set the new resolution", withTimeStamp: true));
            }
            else if (dataPacket?.Header == ServerDataHeader.Stop.ToString())
            {
                // Else if it was a STOP packet then stop image sending
                Trace.WriteLine(Utils.GetDebugMessage("Got STOP packet from server", withTimeStamp: true));
                StopImageSending();
            }
            else if (dataPacket?.Header == ServerDataHeader.Confirmation.ToString())
            {
                // Else if it was a CONFIRMATION packet then update the timer to the max value
                Trace.WriteLine(Utils.GetDebugMessage("Got CONFIRMATION packet from server", withTimeStamp: true));
            }
            else
            {
                // Else it was some invalid packet so add a debug message
                Debug.Assert(false,
                    Utils.GetDebugMessage("Header from server is neither SEND, STOP nor CONFIRMATION"));
            }
        }

        /// <summary>
        /// Image sending function which will take image pixel diffs from processor and 
        /// send it to the server via the networking module. Images are sent only if there
        /// are any changes in pixels as compared to previous image.
        /// </summary>
        private async Task ImageSendingAsync()
        {
            int cnt = 0;
            CancellationToken _imageCancellationToken = _imageCancellation!.Token;
            while (!_imageCancellationToken.IsCancellationRequested)
            {
                string serializedImg = await _processor.GetFrameAsync(_imageCancellationToken);
                if (_imageCancellationToken.IsCancellationRequested)
                {
                    break;
                }

                DataPacket dataPacket = new(_id!, _name!, ClientDataHeader.Image.ToString(), serializedImg, _myPort, _myIP);
                string serializedData = JsonSerializer.Serialize(dataPacket);

                Trace.WriteLine(Utils.GetDebugMessage($"Sent frame {cnt} of size {serializedData.Length}", withTimeStamp: true));
                _communicator.Broadcast(Utils.ModuleIdentifier, serializedData);
                cnt++;
                await Task.Delay(1); // Introduce a small delay for asynchronous behavior
            }
        }

        /// <summary>
        /// Starting the image sending function on a thread.
        /// </summary>
        private async Task StartImageSendingAsync()
        {
            _capturer.StartCapture();
            _processor?.StartProcessingAsync(1);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started capturer and processor", withTimeStamp: true));

            _imageCancellation?.Dispose();
            _sendImageTask = Task.Run(async () => await ImageSendingAsync());
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started image sending", withTimeStamp: true));
            await _sendImageTask;
        }

        /// <summary>
        /// Method to stop screensharing. Calling this will stop sending both the image sending
        /// task and confirmation sending task. It will also call stop on the processor and capturer.
        /// </summary>
        public void StopScreensharing()
        {
            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));
            DataPacket deregisterPacket = new(_id, _name, ClientDataHeader.Deregister.ToString(), "", _myPort, _myIP);
            string serializedDeregisterPacket = JsonSerializer.Serialize(deregisterPacket);

            StopImageSending();
            StopConfirmationSendingAsync().Wait(); // Synchronously wait here for demonstration purposes
            _communicator.Broadcast(Utils.ModuleIdentifier, serializedDeregisterPacket);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully sent DEREGISTER packet to server", withTimeStamp: true));
        }

        /// <summary>
        /// Method to stop sending confirmation packets. Will be called only when the client
        /// stops screensharing.
        /// </summary>
        private async Task StopConfirmationSendingAsync()
        {
            if (_sendConfirmationTask == null)
            {
                return;
            }

            _confirmationCancellationToken = true;
            await _sendConfirmationTask;
        }

        /// <summary>
        /// Method to stop image sending. Will be called whenever the screenshare is stopped by
        /// the client or the client is not on the displayed screen of the server.
        /// </summary>
        private void StopImageSending()
        {
            if (_sendImageTask == null)
            {
                return;
            }

            _capturer.StopCapture();
            _processor.StopProcessing();
            Trace.WriteLine(Utils.GetDebugMessage("Successfully stopped capturer and processor"));

            Debug.Assert(_sendImageTask != null,
                Utils.GetDebugMessage("_sendImageTask is not null, cannot stop image sending"));

            try
            {
                _imageCancellation?.Cancel();
                _sendImageTask.Wait();
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Unable to cancel image sending task: {e.Message}", withTimeStamp: true));
            }

            _sendImageTask = null;
        }

        /// <summary>
        /// Sends confirmation packet to server once every five seconds. The confirmation packet
        /// does not contain any data. The confirmation packets are always sent once the client
        /// has started screen share. In case the network gets disconnected, these packtes will
        /// stop reaching the server, as a result of which the server will remove the client
        /// as a 'screen sharer'.
        /// </summary>
        private async Task SendConfirmationPacketAsync()
        {
            _confirmationCancellationToken = false;
            Debug.Assert(_id != null, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));
            DataPacket confirmationPacket = new(_id, _name, ClientDataHeader.Confirmation.ToString(), "", _myPort, _myIP);
            string serializedConfirmationPacket = JsonSerializer.Serialize(confirmationPacket);

            _sendConfirmationTask = Task.Run(async () =>
            {
                while (!_confirmationCancellationToken)
                {
                    _communicator.Broadcast(Utils.ModuleIdentifier, serializedConfirmationPacket);
                    await Task.Delay(5000);
                }
            });

            Trace.WriteLine(Utils.GetDebugMessage("Starting Confirmation packet sending", withTimeStamp: true));
            await _sendConfirmationTask;
        }

        /// <summary>
        /// Used by dashboard module to set the id and name for the client
        /// </summary>
        /// <param name="id">ID of the client</param>
        /// <param name="name">Name of the client</param>
        public void SetUser(string id, string name, int serverPortNumber, string serverIP)
        {
            _id = id;
            _name = name;
            Trace.WriteLine(Utils.GetDebugMessage("Successfully set client name and id"));
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

