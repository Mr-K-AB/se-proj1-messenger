using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerDashboard.Dashboard
{
    public interface IDashboardSerializer
    {
        string Serialize<S>(S objectToSerialize) where S : new();
        S Deserialize<S>(string serializedString);

    }
}
