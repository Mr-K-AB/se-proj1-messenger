using MessengerWhiteboard.Models;

namespace MessengerWhiteboard.Interfaces
{
    public interface IServerCommunicator
    {
        public void Broadcast(ShapeItem shape, Operation op);
        public void Broadcast(List<ShapeItem> shapes, Operation op);
        public void Broadcast(WBShape WBshape, string? userID);
    }
}
