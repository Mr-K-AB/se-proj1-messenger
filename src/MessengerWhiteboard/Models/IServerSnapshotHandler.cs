using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWhiteboard.Models
{
    public interface IServerSnapshotHandler
    {
        public List<ShapeItem> LoadSession(string id);
        public string SaveSession(string id, List<ShapeItem> shapes);
    }
}
