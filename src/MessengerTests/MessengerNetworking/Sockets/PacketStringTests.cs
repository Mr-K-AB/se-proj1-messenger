using MessengerNetworking.Queues;
using MessengerNetworking.Sockets;

namespace MessengerTests.MessengerNetworking.Sockets
{
    [TestClass]
    public class PacketStringTests
    {
        /// <summary>
        /// Tests conversion from packet to packet string and then
        /// back to packet
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void PacketToStringAndStringToPacketTest()
        {
            Packet packet = new("Data", "Destination", "Module");
            string pktStr = 
                PacketString.PacketToPacketString(packet);
            Packet packet1 = PacketString.PacketStringToPacket(pktStr);
            NetworkTestGlobals.AssertPacketEquality(packet, packet1);
        }

        /// <summary>
        /// Tests error catch when converting packet string to packet
        /// </summary>
        /// <returns> void </returns>
        [TestMethod]
        public void PacketStringToPacketErrorCatchTest()
        {
            Packet packet = new("null", "null", "null");
            Packet packet1 = PacketString.PacketStringToPacket(null);
            NetworkTestGlobals.AssertPacketEquality(packet, packet1);
        }
    }
}
