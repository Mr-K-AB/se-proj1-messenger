/******************************************************************************
* Filename    = SerializedDataWrapper.cs
*
* Author      = Shailab Chauhan 
*
* Roll number = 112001038
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = Represents a wrapper for serialized data along with it's type information.
*****************************************************************************/
using System;

namespace MessengerDashboard
{
    /// <summary>
    /// Represents a wrapper for serialized data along with its type information.
    /// </summary>
    internal class SerializedDataWrapper
    {
        /// <summary>
        /// Gets or sets the serialized data.
        /// </summary>
        public string SerializedData { get; set; }

        /// <summary>
        /// Gets or sets the type of the serialized data.
        /// </summary>
        public Type DataType { get; set; }
    }
}
