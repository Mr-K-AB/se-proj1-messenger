using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerScreenshare.Client
{
    public interface IScreenshareClient
    {
        public void SetUser(string id, string naame, int serverPortNumber, string serverIP);
        public bool IsExam { get; set; }
    }
}
