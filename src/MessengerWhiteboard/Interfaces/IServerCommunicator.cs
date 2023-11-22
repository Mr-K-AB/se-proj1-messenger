using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
