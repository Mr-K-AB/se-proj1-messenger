namespace MessengerWhiteboard.Models
{
    public interface IServerSnapshotHandler
    {
        public List<ShapeItem> LoadSession(string id);
        public string SaveSession(string id, List<ShapeItem> shapes);
    }
}
