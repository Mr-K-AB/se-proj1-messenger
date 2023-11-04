using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard
{
    public class DashboardSerializer : IDashboardSerializer
    {
        public DashboardSerializer() { }

        public S Deserialize<S>(string serializedString)
        {
            throw new NotImplementedException();
        }

        public string Serialize<S>(S objectToSerialize) where S : new()
        {
            throw new NotImplementedException();
        }
    }
}
