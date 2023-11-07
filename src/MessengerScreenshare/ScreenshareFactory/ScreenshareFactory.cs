using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessengerScreenshare.Client;

namespace MessengerScreenshare.ScreenshareFactory
{
    public static class ScreenshareFactory
    {
        public static ScreenshareClient clientInstance = new(false);
        public static IScreenshareClient getInstance() { return clientInstance; }
    }
}
