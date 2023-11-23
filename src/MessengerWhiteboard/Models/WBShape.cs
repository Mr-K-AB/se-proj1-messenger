/***************************
* Filename    = WBShape.cs
*
* Author      = Kaustubh Chavan
*
* Product     = Messenger
* 
* Project     = White-Board
*
* Description = This file defines the WBShape class, representing a shape for whiteboard operations.
                It includes properties to store a list of SerializableShapeItem objects, user ID, 
                operation type, and snapshot ID.
* 
***************************/

using System.Collections.Generic;

namespace MessengerWhiteboard.Models
{
    /// <summary>
    /// The WBShape class represents a shape for whiteboard operations.
    /// </summary>
    public class WBShape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WBShape"/> class.
        /// </summary>
        /// <param name="serializedShapes">List of serializable shape items.</param>
        /// <param name="operation">Type of whiteboard operation.</param>
        /// <param name="userID">User ID associated with the shape (default is "1").</param>
        /// <param name="snapshotID">Snapshot ID associated with the shape (default is null).</param>
        public WBShape(List<SerializableShapeItem> serializedShapes, Operation operation, string userID = "1", string snapshotID = null)
        {
            ShapeItems = serializedShapes;
            Op = operation;
            UserId = userID;
            SnapshotID = snapshotID;
        }

        /// <summary>
        /// Gets or sets the list of serializable shape items.
        /// </summary>
        public List<SerializableShapeItem> ShapeItems { get; set; }

        /// <summary>
        /// Gets or sets the user ID associated with the shape.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the type of whiteboard operation.
        /// </summary>
        public Operation Op { get; set; }

        /// <summary>
        /// Gets or sets the snapshot ID associated with the shape.
        /// </summary>
        public string SnapshotID { get; set; }
    }
}
