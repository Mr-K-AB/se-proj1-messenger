using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
