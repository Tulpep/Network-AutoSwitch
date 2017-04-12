using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Tulpep.NetworkAutoSwitch.Service
{
    class NetworkState
    {
        public OperationalStatus WirelessStatus { get; set; }
        public HashSet<string> WirelessAdapters { get; set; }

        public HashSet<string> WiredAdapters { get; set; }
        public OperationalStatus WiredStatus { get; set; }


        public NetworkState()
        {
            WiredAdapters = new HashSet<string>();
            WirelessAdapters = new HashSet<string>();
        }
    }
}
