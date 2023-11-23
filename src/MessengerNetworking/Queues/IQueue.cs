﻿/******************************************************************************
 * 
 * Author      = Priyanshu Gupta
 *
 * Roll no     = 112001033
 *
 *****************************************************************************/

namespace MessengerNetworking.Queues
{
    public interface IQueue
    {
        /// <summary>
        /// Inserts an element into the queue
        /// </summary>
        /// <param name="packet">
        /// The packet to be inserted into the queue
        /// </param>
        /// <returns> void </returns>
        public void Enqueue(Packet packet);

        /// <summary>
        /// Removes and returns the front-most element in the queue
        /// </summary>
        /// <returns>
        /// The front-most element of the queue
        /// </returns>
        public Packet Dequeue();

        /// <summary>
        /// Returns the front-most element in the queue without popping it
        /// </summary>
        /// <returns>
        /// The front-most element of the queue
        /// </returns>
        public Packet Peek();

        /// <summary>
        /// Removes all elements in the queue
        /// </summary>
        /// <returns> void </returns>
        public void Clear();

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        /// <returns>
        /// Number of elements in the queue
        /// </returns>
        public int Size();

        /// <summary>
        /// Returns the size of the queue
        /// </summary>
        /// <returns>
        /// 'bool : true' if the queue is empty and 'bool : false' if not
        /// </returns>
        public bool IsEmpty();

        /// <summary>
        /// The 'ReceivingQueueListener' needs this function to keep listening for packets on the receiving queue
        /// </summary>
        /// <returns>
        /// 'bool : true' if the queue is not empty, else the function keeps waiting for atleast one packet to appear in the queue
        /// and does not return until then
        /// </returns>
        public bool WaitForPacket();
    }
}
