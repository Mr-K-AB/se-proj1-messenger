using System.IO;
using MessengerWhiteboard.Models;

namespace MessengerWhiteboard
{
    public class ServerSnapshotHandler : IServerSnapshotHandler
    {
        private readonly Serializer _serializer;
        public string SnapshotId { get; set; }

        public ServerSnapshotHandler()
        {
            _serializer = new Serializer();
        }
        public List<ShapeItem> LoadSession(string id)
        {
            try
            {
                string fileName = id + ".json";
                string jsonString = File.ReadAllText(fileName);
                List<ShapeItem> shapeItems = _serializer.UnMarshalShapes(jsonString);
                return shapeItems;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public string SaveSession(string id, List<ShapeItem> shapes)
        {
            try
            {
                string fileName = id + ".json";
                string jsonString = _serializer.MarhsalShapes(shapes);

                File.WriteAllText(fileName, jsonString);
                return id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}
