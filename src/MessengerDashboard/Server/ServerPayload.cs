using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Server
{
    /// <summary>
    /// Represents a server payload.
    /// </summary>
    public class ServerPayload
    {
        /// <summary>
        /// Gets or sets the operation to be performed by the server.
        /// </summary>
        public Operation Operation { get; set; }
    }
}
