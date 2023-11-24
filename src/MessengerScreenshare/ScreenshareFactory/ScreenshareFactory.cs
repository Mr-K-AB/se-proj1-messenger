/******************************************************************
* Filename    = ScreenshareFactory.cs
*
* Author      = Likhith Reddy
*
* Product     = ScreenShare
* 
* Project     = Messenger
*
* Description = FActory to get the client instance
*****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using MessengerScreenshare.Client;
using MessengerScreenshare.Server;

namespace MessengerScreenshare.ScreenshareFactory
{
    /// <summary>
    /// Factory to obtain the client instance
    /// </summary>
    public static class ScreenshareFactory
    {
        private static readonly ScreenshareClient s_clientInstance = new(false);

        /// <summary>
        /// Factory method to get the client instance
        /// To be used by dashboard module
        /// </summary>
        /// <returns>returns client instance of type IScreenshareClient</returns>
        public static IScreenshareClient getInstance() { return s_clientInstance; }

        /// <summary>
        /// Factory method to get the client instance (to be used by rest)
        /// </summary>
        /// <param name="viewModel">instance of clientViewModel</param>
        /// <returns>returns client instance of type ScreenshareClient</returns>
        public static ScreenshareClient getClientInstance(ScreenshareClientViewModel? viewModel = null)
        {
            s_clientInstance._viewModel = viewModel;
            return s_clientInstance;
        }
    }
}
