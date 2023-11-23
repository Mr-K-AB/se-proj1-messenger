/******************************************************************************
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
    public class ReceivingQueueTests
    {
        readonly ReceivingQueue _receivingQueue = new();

        /// <summary>
        /// Testing whether a packet is getting inserted into the receiving queue when enqueued
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void EnqueueOnePacketTest()
        {
            string serializedString = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = NetworkTestGlobals.DashboardName;

            Packet packet = new(serializedString, destinationModule, moduleName);

            // Enqueueing a single random packet
            _receivingQueue.Enqueue(packet);

            Assert.AreEqual(_receivingQueue.Size(), 1);
        }

        /// <summary>
        /// Testing whether multiple packets are getting inserted into the receiving queue when enqueued
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void EnqueueMultiplePacketsTest()
        {
            string moduleName = NetworkTestGlobals.DashboardName;
            string destinationModule = NetworkTestGlobals.RandomString(5);

            int number_of_packets = 100;

            // Enqueueing multiple packets
            for (int i = 0; i < number_of_packets; ++i)
            {
                _receivingQueue.Enqueue(new(NetworkTestGlobals.RandomString(10), destinationModule, moduleName));
            }

            Assert.AreEqual(_receivingQueue.Size(), number_of_packets);
        }

        /// <summary>
        /// Testing whether packets are getting inserted into the receiving queue in the order in which they are enqueued
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void EnqueueMultiplePacketsOrderTest()
        {
            string moduleName = NetworkTestGlobals.DashboardName;
            string destinationModule = NetworkTestGlobals.RandomString(5);

            int number_of_packets = 100;

            // For comparison of each packet
            Stack stack = new();

            // Enqueueing multiple packets
            for (int i = 0; i < number_of_packets; ++i)
            {
                Packet packet = new(NetworkTestGlobals.RandomString(10), destinationModule, moduleName);
                _receivingQueue.Enqueue(packet);

                stack.Push(packet);
            }

            // Reversing the stack's elements for comparison
            Stack reverseStack = new();
            while (stack.Count != 0)
            {
                reverseStack.Push(stack.Pop());
            }

            // Checking each element
            while (reverseStack.Count != 0 && !_receivingQueue.IsEmpty())
            {
                Assert.AreEqual(_receivingQueue.Dequeue(), reverseStack.Pop());
            }

            // The reverse stack and receiving queue need to be empty
            Assert.AreEqual(_receivingQueue.Size(), 0);
            Assert.AreEqual(_receivingQueue.Size(), reverseStack.Count);
        }

        /// <summary>
        /// Testing whether concurrent insertion of packets into the receiving queue works fine
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

                    // Creating a packet of the above module
                    Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                    _receivingQueue.Enqueue(packet);
                }
            });

            // Starting another thread
            var thread2 = Task.Run(() =>
            {
                for (int i = 0; i < number_of_packets; ++i)
                {
                    // Taking a random module name
                    string moduleName = NetworkTestGlobals.RandomString(10);

                    // Creating a packet of the above module
                    Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                    _receivingQueue.Enqueue(packet);
                }
            });

            // Waiting for all tasks to be finished
            Task.WaitAll(thread1, thread2);

            Assert.AreEqual(_receivingQueue.Size(), 2 * number_of_packets);
        }

        /// <summary>
        /// Testing whether Clear() method of the receiving queue removes all entries in the queue
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void ClearReceivingQueueTest()
        {
            int number_of_packets = 100;

            for (int i = 0; i < number_of_packets; ++i)
            {
                // Taking a random module name
                string moduleName = NetworkTestGlobals.RandomString(10);

                // Creating a packet of the above module
                Packet packet = new(NetworkTestGlobals.RandomString(10), NetworkTestGlobals.RandomString(5), moduleName);

                _receivingQueue.Enqueue(packet);
            }

            // Checking if all packets are enqueued
            Assert.AreEqual(_receivingQueue.Size(), number_of_packets);

            _receivingQueue.Clear();

            // Checking if all packets are removed
            Assert.AreEqual(_receivingQueue.Size(), 0);
        }

        /// <summary>
        /// Testing whether concurrent dequeue of a packet in the receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void ConcurrentDequeueTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            // Creating a packet
            Packet packet = new(serializedData, destinationModule, moduleName);

            _receivingQueue.Enqueue(packet);

            Packet dequeueOne = null, dequeueTwo = null;

            // Concurrently dequeueing
            Parallel.Invoke(
                () => { dequeueOne = _receivingQueue.Dequeue(); },
                () => { dequeueTwo = _receivingQueue.Dequeue(); }
                );

            // Exactly one of the dequeue must be fruitful
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
        /// Testing whether concurrent peek of a packet in the receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void ConcurrentPeekTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            // Creating a packet
            Packet packet = new(serializedData, destinationModule, moduleName);

            _receivingQueue.Enqueue(packet);

            Packet peekOne = null, peekTwo = null;

            // Concurrently peeking
            Parallel.Invoke(
                () => { peekOne = _receivingQueue.Peek(); },
                () => { peekTwo = _receivingQueue.Peek(); }
                );

            // Output of both peeks must be same and not null
            Assert.IsTrue(peekOne != null);
            Assert.AreEqual(peekOne, peekTwo);

            // Comparing with the packet enqueued
            Assert.AreEqual(packet, peekOne);
        }

        /// <summary>
        /// Testing whether peek on an empty receiving queue works fine
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void PeekOnEmptyQueueTest()
        {
            Packet packet = _receivingQueue.Peek();
            Assert.AreEqual(packet, null);
        }

        /// <summary>
        /// Testing whether WaitForPacket() method detects a packet in the receiving queue and returns 'bool : true'
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void WaitForPacketTest()
        {
            string serializedData = NetworkTestGlobals.RandomString(10);
            string destinationModule = NetworkTestGlobals.RandomString(5);
            string moduleName = "Demo";

            // Creating a packet
            Packet packet = new(serializedData, destinationModule, moduleName);

            // Thread to enqueue a packet
            Thread enqueueThread = new(() =>
            {
                _receivingQueue.Enqueue(packet);
            });

            bool isEmpty = true;

            // Thread to wait for a packet
            Thread waitThread = new(() =>
            {
                isEmpty = _receivingQueue.WaitForPacket();
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
        /// Testing whether WaitForPacket() method keeps waiting forever when no packet arrives at the receiving queue
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void WaitForeverForPacketTest()
        {
            // Thread to wait for a packet
            Thread waitThread = new(() =>
            {
                _receivingQueue.WaitForPacket();
            });

            // Beginning to wait
            waitThread.Start();

            // Sleeping for some time
            Thread.Sleep(5000);

            // Checking that the thread has not terminated yet
            Assert.IsTrue(waitThread.IsAlive);
        }
    }
}
