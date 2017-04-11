using System.Collections.Generic;

namespace Tulpep.NetworkAutoSwitch.Logic
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
