﻿///<author>Alugonda Sathvik </author>
///<summary>
/// This file has ScreenshareClient class's implementation
/// In this file functions related to starting and stopping Screen Capturing 
/// are implemented
///</summary>
///
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
using System.Timers;

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
        public string? _name = "SuryaBhai";
        public int _id = 10;
        // Tokens added to be able to stop the thread execution
        private bool _confirmationCancellationToken;
        private readonly CancellationTokenSource? _imageCancellation;

        // View model for screenshare client
        private ScreenshareClientViewModel? _viewModel;

        private readonly System.Timers.Timer? _timer;
        public static double Timeout { get; } = 20 * 1000;


        /// <summary>
        /// Setting up the ScreenCapturer and ScreenProcessor Class
        /// Taking instance of communicator from communicator factory
        /// and subscribing to it.
        /// </summary>
        public ScreenshareClient(bool isDebugging)
        {
            _capturer = new ScreenCapturer();
            _processor = new ScreenProcessor(_capturer);
            _imageCancellation = new CancellationTokenSource();
            if (!isDebugging)
            {
                _communicator = Factory.GetInstance();
                _communicator.AddSubscriber(Utils.ClientIdentifier, this);
            }

            try
            {
                // Create the timer for this client.
                _timer = new System.Timers.Timer();
                _timer.Elapsed += new((sender, e) => OnTimeOut());

                // The timer should be invoked only once.
                _timer.AutoReset = false;

                // Set the time interval for the timer.
                UpdateTimer();
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to create the timer: {e.Message}", withTimeStamp: true));
            }

            Trace.WriteLine(Utils.GetDebugMessage("Successfully stopped image processing", withTimeStamp: true));
        }

        /// <summary>
        /// On timeout stop screensharing and make the viewmodel's sharingscreen boolean
        /// value as false for letting viewmodel know that screenshare stopped
        /// </summary>
        public void OnTimeOut()
        {
            StopScreensharing();
            _viewModel.SharingScreen = false;
            Trace.WriteLine(Utils.GetDebugMessage($"Timeout occurred", withTimeStamp: true));
        }

        /// <summary>
        /// Gives an instance of ScreenshareClient class and that instance is always 
        /// the same i.e. singleton pattern.
        /// </summary>
        public static ScreenshareClient GetInstance(ScreenshareClientViewModel? viewModel = null, bool isDebugging = false)
        {
            s_screenShareClient ??= new ScreenshareClient(isDebugging);
            s_screenShareClient._viewModel = viewModel;
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
            // Start the timer.
            _timer.Enabled = true;

            Debug.Assert(_id != 0, Utils.GetDebugMessage("_id property found null"));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null"));

            DataPacket dataPacket = new(_id, _name, ClientDataHeader.Register.ToString(), 0, 0, "");
            string serializedData = JsonSerializer.Serialize(dataPacket);
            _communicator.Broadcast(Utils.ServerIdentifier, serializedData);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully sent REGISTER packet to server", withTimeStamp: true));
            //Task.Run(async () => await StartImageSendingAsync());
            Trace.WriteLine(Utils.GetDebugMessage("Started sending confirmation packet", withTimeStamp: true));
            await SendConfirmationPacketAsync();
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
                UpdateTimer();
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
        /// Resets the time of the timer object.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateTimer()
        {
            Debug.Assert(_timer != null, Utils.GetDebugMessage("_timer is found null"));

            try
            {
                // It will reset the timer to start again.
                _timer.Interval = Timeout;
            }
            catch (Exception e)
            {
                Trace.WriteLine(Utils.GetDebugMessage($"Failed to reset the timer: {e.Message}", withTimeStamp: true));
                throw new Exception("Failed to reset the timer", e);
            }
        }

        /// <summary>
        /// Image sending function which will take image pixel diffs from processor and 
        /// send it to the server via the networking module. Images are sent only if there
        /// are any changes in pixels as compared to previous image.
        /// </summary>
        private async Task ImageSendingAsync(CancellationToken cancellationToken)
        {
            int cnt = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                string serializedImg = await _processor.GetFrameAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                cnt++;
                //DataPacket dataPacket = new(_id, _name, ClientDataHeader.Image.ToString(), serializedImg);
                //string serializedData = JsonSerializer.Serialize(dataPacket);

                // Split the data into 500 fragments
                /*List<string> dataFragments = SplitDataIntoFragments(serializedImg, 500);
                int fragmentOffset = 1;
                foreach (string fragment in dataFragments)
                {
                    DataPacket dataPacket = new(_id, _name, ClientDataHeader.Image.ToString(), cnt, fragmentOffset, fragment);
                    string serializedData = JsonSerializer.Serialize(dataPacket);
                    Trace.WriteLine(Utils.GetDebugMessage($"Sent frame {cnt} fragment of size {fragment.Length}", withTimeStamp: true));
                    _communicator.Broadcast(Utils.ServerIdentifier, serializedData);
                    fragmentOffset++;
                }*/
                DataPacket dataPacket = new(_id, _name, ClientDataHeader.Image.ToString(), cnt, 0, serializedImg);
                string serializedData = JsonSerializer.Serialize(dataPacket);
                _communicator.Broadcast(Utils.ServerIdentifier, serializedData);
                //await Task.Delay(1); // Introduce a small delay for asynchronous behavior
            }
        }

        private List<string> SplitDataIntoFragments(string data, int fragmentSize)
        {
            List<string> fragments = new();
            for (int i = 0; i < data.Length; i += fragmentSize)
            {
                int length = Math.Min(fragmentSize, data.Length - i);
                fragments.Add(data.Substring(i, length));
            }
            return fragments;
        }


        /// <summary>
        /// Starting the image sending function on a thread.
        /// </summary>
        private async Task StartImageSendingAsync()
        {
            CancellationToken cancellationToken = _imageCancellation!.Token;
            _imageCancellation?.Dispose();
            _capturer.StartCapture();
            _processor?.StartProcessingAsync(1);
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started capturer and processor", withTimeStamp: true));
            
            _sendImageTask = Task.Run(async () => await ImageSendingAsync(cancellationToken));
            Trace.WriteLine(Utils.GetDebugMessage("Successfully started image sending", withTimeStamp: true));
            await _sendImageTask;
        }

        /// <summary>
        /// Method to stop screensharing. Calling this will stop sending both the image sending
        /// task and confirmation sending task. It will also call stop on the processor and capturer.
        /// </summary>
        public void StopScreensharing()
        {
            Debug.Assert(_id != 0, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));
            DataPacket deregisterPacket = new(_id, _name, ClientDataHeader.Deregister.ToString(), 0, 0, "");
            string serializedDeregisterPacket = JsonSerializer.Serialize(deregisterPacket);

            StopImageSending();
            StopConfirmationSendingAsync().Wait(); // Synchronously wait here for demonstration purposes
            _communicator.Broadcast(Utils.ServerIdentifier, serializedDeregisterPacket);
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
            Debug.Assert(_id != 0, Utils.GetDebugMessage("_id property found null", withTimeStamp: true));
            Debug.Assert(_name != null, Utils.GetDebugMessage("_name property found null", withTimeStamp: true));
            DataPacket confirmationPacket = new(_id, _name, ClientDataHeader.Confirmation.ToString(), 0, 0, "");
            string serializedConfirmationPacket = JsonSerializer.Serialize(confirmationPacket);

            _sendConfirmationTask = Task.Run(async () =>
            {
                while (!_confirmationCancellationToken)
                {
                    _communicator.Broadcast(Utils.ServerIdentifier, serializedConfirmationPacket);
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
        public void SetUser(int id, string name)
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

