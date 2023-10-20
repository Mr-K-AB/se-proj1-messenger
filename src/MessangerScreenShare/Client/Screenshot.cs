using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessangerScreenShare.Client
{
    public class Screenshot
    {
        private static readonly object s_lock = new();
        private static Screenshot? s_instance;
        public bool CaptureActive { get; private set; }
        protected Screenshot()
        {
            CaptureActive = false;
        }

        public static Screenshot Instance()
        {
            lock (s_lock)
            {
                s_instance ??= new Screenshot();
                return s_instance;
            }
        }

        public byte[] MakeScreenShot()
        {
            throw new NotImplementedException();
        }
    }
}
