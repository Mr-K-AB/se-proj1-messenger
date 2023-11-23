﻿/******************************************************************************
 * 
 * Author      = Priyanshu Gupta
 *
 * Roll no     = 112001033
 *
 *****************************************************************************/

using MessengerNetworking.Queues;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerTests.MessengerNetworking.Queues
{
    [TestClass]
    public class SendingQueuesTests
    {
        private readonly SendingQueue _sendingQueue = new();

        /// <summary>
        /// Testing whether a packet is inserted into the sending queue by calling the method Enqueue()
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void EnqueueOnePacketTest()
        {
            // Registering the dashboard module
            _sendingQueue.RegisterModule(NetworkTestGlobals.DashboardName, true);

            // Creating a packet
            Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.DashboardName,
                NetworkTestGlobals.DashboardName);

            // Enqueueing the packet
            _sendingQueue.Enqueue(packet);

            Assert.IsFalse(_sendingQueue.IsEmpty());
            Assert.AreEqual(_sendingQueue.Size(), 1);
        }

        /// <summary>
        /// Testing whether calling the method Dequeue() on an empty sending queue returns null
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void DequeueOnEmptyQueueTest()
        {
            // Dequeueing on an empty queue
            Packet packet = _sendingQueue.Dequeue();

            Assert.IsTrue(_sendingQueue.IsEmpty());
            Assert.AreEqual(packet, null);
        }

        /// <summary>
        /// Testing whether the packet of an unregistered module returns 'bool : false'
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void EnqueueUnregisteredModulePacketTest()
        {
            // Creating a packet
            Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.DashboardName,
                NetworkTestGlobals.DashboardName);

            bool isSuccessful = _sendingQueue.Enqueue(packet);
            Assert.AreEqual(isSuccessful, false);
        }

        /// <summary>
        /// Testing whether concurrent calls of Enqueue() on the sending queue works fine
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void ConcurrentEnqueuesTest()
        {
            int number_of_packets = 100;

            // Starting a thread
            var thread1 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkTestGlobals.RandomString(10);

                    // Registering the module with a random priority
                    _sendingQueue.RegisterModule(moduleName, i % 2 == 0);

                    // Creating a packet of the above module
                    Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                    _sendingQueue.Enqueue(packet);
                }
            });

            // Starting another thread
            var thread2 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkTestGlobals.RandomString(10);

                    // Registering the module with a random priority
                    _sendingQueue.RegisterModule(moduleName, i % 2 == 0);

                    // Creating a packet of the above module
                    Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                    _sendingQueue.Enqueue(packet);
                }
            });

            // Waiting for all tasks to be finished
            Task.WaitAll(thread1, thread2);

            Assert.IsFalse(_sendingQueue.IsEmpty());
            Assert.AreEqual(_sendingQueue.Size(), 2 * number_of_packets);
        }

        /// <summary>
        /// Testing whether the Clear() method of the sending queue removes all the enqueued elements
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void ClearSendingQueuesTest()
        {
            int number_of_packets = 100;

            for (int i = 0; i < number_of_packets; ++i)
            {
                // Taking a random module name
                string moduleName = NetworkTestGlobals.RandomString(10);

                // Registering the module with a random priority
                _sendingQueue.RegisterModule(moduleName, i % 2 == 0);

                // Creating a packet of the above module
                Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                _sendingQueue.Enqueue(packet);
            }

            // Checking if all packets are enqueued
            Assert.IsFalse(_sendingQueue.IsEmpty());
            Assert.AreEqual(_sendingQueue.Size(), number_of_packets);

            _sendingQueue.Clear();

            // Checking if all packets are removed
            Assert.IsTrue(_sendingQueue.IsEmpty());
            Assert.AreEqual(_sendingQueue.Size(), 0);
        }

        /// <summary>
        /// Testing whether multiple registrations of the same module return 'bool : false'
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void DuplicateModuleNameTest()
        {
            string moduleName = "Demo module";

            bool isSuccessful = _sendingQueue.RegisterModule(moduleName, true);

            Assert.AreEqual(isSuccessful, true);

            // Duplicate registration
            isSuccessful = _sendingQueue.RegisterModule(moduleName, false);

            Assert.AreEqual(isSuccessful, false);
        }

        /// <summary>
        /// Testing whether packets are inserted into the high and low-priority queues in the order in which they are enqueued
        /// </summary>
        /// <returns> void </returns>
        private void CheckEnqueueOrderTest(bool isHighPriority)
        {
            string moduleName = "Demo high";
            _sendingQueue.RegisterModule(moduleName, isHighPriority);

            // To store every packet in the order of enqueueing
            Stack stack = new();

            // Enqueueing packets of high priority
            int number_of_packets = 100;
            for (int i = 0; i < number_of_packets; ++i)
            {
                string serializedData = NetworkTestGlobals.RandomString(10);
                string destinationModule = NetworkTestGlobals.RandomString(5);

                Packet packet = new(serializedData, destinationModule, moduleName);

                // Pushing into a stack
                stack.Push(packet);

                _sendingQueue.Enqueue(packet);
            }

            // Reversing the stack
            Stack reverseStack = new();
            while (stack.Count != 0)
            {
                reverseStack.Push(stack.Pop());
            }

            // Dequeueing each packet
            while (!_sendingQueue.IsEmpty() && (reverseStack.Count != 0))
            {
                Packet packet = _sendingQueue.Dequeue();

                // Checking if each packet is in order of enqueueing
                Assert.AreEqual(packet, reverseStack.Pop());
            }

            // Both queue and stack need to be empty here
            Assert.AreEqual(_sendingQueue.Size(), 0);
            Assert.AreEqual(_sendingQueue.Size(), reverseStack.Count);
        }

        /// <summary>
        /// Calling the test method 'CheckEnqueueOrderTest()' for high and low-priority packets
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void CheckEnqueueOrderForAllQueuesTest()
        {
            // Checking the order of enqueueing in both high and low priority case
            CheckEnqueueOrderTest(true);
            CheckEnqueueOrderTest(false);
        }

        /// <summary>
        /// Checking whether concurrent Dequeue() calls on the sending queue works fine
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void ConcurrentDequeueTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            _sendingQueue.RegisterModule(moduleName, true);

            // Creating a packet
            Packet packet = new(serializedData, destinationModule, moduleName);

            _sendingQueue.Enqueue(packet);

            Packet dequeueOne = null, dequeueTwo = null;

            // Concurrently dequeueing
            Parallel.Invoke(
                () => { dequeueOne = _sendingQueue.Dequeue(); },
                () => { dequeueTwo = _sendingQueue.Dequeue(); }
                );

            // Exactly one of the dequeues must be fruitful
            Assert.IsTrue(dequeueOne == null || dequeueTwo == null);
            Assert.IsTrue(dequeueOne != null || dequeueTwo != null);

            // Comparing with the packet enqueued
            if (dequeueOne != null)
            {
                Assert.AreEqual(packet, dequeueOne);
            }
            else
            {
                Assert.AreEqual(packet, dequeueTwo);
            }
        }

        /// <summary>
        /// Testing whether WaitForPacket() method detects a packet in the sending queue and returns 'bool : true'
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void WaitForPacketTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            _sendingQueue.RegisterModule(moduleName, false);

            // Creating a packet
            Packet packet = new(serializedData, destinationModule, moduleName);

            // Thread to enqueue a packet
            Thread enqueueThread = new(() =>
            {
                _sendingQueue.Enqueue(packet);
            });

            bool isEmpty = true;

            // Thread to wait for a packet
            Thread waitThread = new(() =>
            {
                isEmpty = _sendingQueue.WaitForPacket();
            });

            // Beginning to wait
            waitThread.Start();

            // Sleeping for some time
            Thread.Sleep(3000);

            // Checking that the thread has not terminated yet
            Assert.IsTrue(waitThread.IsAlive);

            // Enqueueing a packet now
            enqueueThread.Start();

            // Sleeping until the thread is terminated
            while (waitThread.IsAlive)
            {
                Thread.Sleep(1000);
            }

            // The waiting thread must have been terminated by now
            Assert.IsFalse(isEmpty);
        }

        /// <summary>
        /// Testing whether WaitForPacket() method keeps waiting forever when no packet arrives at the sending queue
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void WaitForeverForPacketTest()
        {
            // Thread to wait for a packet
            Thread waitThread = new(() =>
            {
                _sendingQueue.WaitForPacket();
            });

            // Beginning to wait
            waitThread.Start();

            // Sleeping for some time
            Thread.Sleep(5000);

            // Checking that the thread has not terminated yet
            Assert.IsTrue(waitThread.IsAlive);
        }

        /// <summary>
        /// Testing whether the ratio of dequeue high and low-priority packets in sending queue in working fine
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void RatioOfDequeueTest()
        {
            // Case 1
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo1";

            // Registering a high-priority module
            _sendingQueue.RegisterModule(moduleName, true);

            // Creating a packet
            Packet packet1 = new(serializedData, destinationModule, moduleName);

            _sendingQueue.Enqueue(packet1);

            // Case 2
            serializedData = NetworkTestGlobals.RandomString(10);
            destinationModule = NetworkTestGlobals.RandomString(5);
            moduleName = "Demo2";

            // Registering a high-priority module
            _sendingQueue.RegisterModule(moduleName, true);

            // Creating a packet
            Packet packet2 = new(serializedData, destinationModule, moduleName);

            _sendingQueue.Enqueue(packet2);

            // Case 3
            serializedData = NetworkTestGlobals.RandomString(10);
            destinationModule = NetworkTestGlobals.RandomString(5);
            moduleName = "Demo3";

            // Registering a low-priority module
            _sendingQueue.RegisterModule(moduleName, false);

            // Creating a packet
            Packet packet3 = new(serializedData, destinationModule, moduleName);

            _sendingQueue.Enqueue(packet3);

            // Case 4
            serializedData = NetworkTestGlobals.RandomString(10);
            destinationModule = NetworkTestGlobals.RandomString(5);
            moduleName = "Demo4";

            // Registering a low-priority module
            _sendingQueue.RegisterModule(moduleName, false);

            // Creating a packet
            Packet packet4 = new(serializedData, destinationModule, moduleName);

            _sendingQueue.Enqueue(packet4);

            // Checking whether all packets have been enqueued
            Assert.AreEqual(_sendingQueue.Size(), 4);

            // Checking whether the packets are dequeued in the order high-high-low-low
            Packet packet = _sendingQueue.Dequeue();
            Assert.AreEqual(packet, packet1);

            packet = _sendingQueue.Dequeue();
            Assert.AreEqual(packet, packet2);

            packet = _sendingQueue.Dequeue();
            Assert.AreEqual(packet, packet3);

            packet = _sendingQueue.Dequeue();
            Assert.AreEqual(packet, packet4);
        }
    }
}
