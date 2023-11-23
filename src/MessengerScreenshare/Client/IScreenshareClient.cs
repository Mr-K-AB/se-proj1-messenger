/******************************************************************
* Filename    = IScreenshareClient.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = Interface to set the user details by dashboard.
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerScreenshare.Client
{
    /// <summary>
    /// Interface to obtain user id and name from the dashboard
    /// </summary>
    public interface IScreenshareClient
    {
        /// <summary>
        /// funcion called to set the user details by dashboard when clint instance is created
        /// </summary>
        /// <param name="id">Id of the client</param>
        /// <param name="name">Name of the client</param>
        public void SetUser(int id, string name);
    }
}
