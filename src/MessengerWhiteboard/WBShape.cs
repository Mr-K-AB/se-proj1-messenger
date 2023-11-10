namespace MessengerWhiteboard
{
    public class WBShape
    {
        public WBShape(List<SerializableShapeItem> serializedShapes, Operation operation, string userID = "1")
        {
            ShapeItems = serializedShapes;
            Op = operation;
            UserId = userID;
        }

        public List<SerializableShapeItem> ShapeItems { get; set; }
        public string UserId { get; set; }
        public Operation Op { get; set; }

    }
}
