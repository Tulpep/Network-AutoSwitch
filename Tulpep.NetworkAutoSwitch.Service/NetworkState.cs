using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tulpep.NetworkAutoSwitch.Service
{
    class NetworkState
    {
        public bool WiredIsUp { get; set; }
        public bool WirelessIsUp { get; set; }
        public HashSet<string> WiredAdapters { get; set; }
        public HashSet<string> WirelessAdapters { get; set; }

        public NetworkState()
        {
            WiredAdapters = new HashSet<string>();
            WirelessAdapters = new HashSet<string>();
        }
    }
}
