namespace MessengerWhiteboard.Models
{
    public class WBShape
    {
        public WBShape(List<SerializableShapeItem> serializedShapes, Operation operation, string userID = "1", string snapshotID = null)
        {
            ShapeItems = serializedShapes;
            Op = operation;
            UserId = userID;
            SnapshotID = snapshotID;
        }

        public List<SerializableShapeItem> ShapeItems { get; set; }
        public string UserId { get; set; }
        public Operation Op { get; set; }
        public string SnapshotID { get; set; }

    }
}
