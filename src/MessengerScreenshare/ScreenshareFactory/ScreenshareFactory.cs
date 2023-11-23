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
    public static class ScreenshareFactory
    {
        private static readonly ScreenshareClient s_clientInstance = new(false);
        public static IScreenshareClient getInstance() { return s_clientInstance; }

        public static ScreenshareClient getClientInstance(ScreenshareClientViewModel? viewModel = null)
        {
            s_clientInstance._viewModel = viewModel;
            return s_clientInstance;
        }
    }
}
