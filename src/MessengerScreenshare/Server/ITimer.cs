using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MessengerScreenshare.Server
{
    public interface ITimer
    {
        public void OnTimeOut(object? source, int id, ElapsedEventArgs e);
    }
}
