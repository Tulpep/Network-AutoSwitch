using System;
using System.Net.NetworkInformation;

namespace Tulpep.NetworkAutoSwitch.Service
{
    public class DetectNetworkChanges
    {
        public DetectNetworkChanges()
        {
            ManageNetworkState.AnalyzeNow();
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkAddressChanged);
        }

        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            ManageNetworkState.AnalyzeNow();
        }

        public void StopNow()
        {
            ManageNetworkState.EnableAllNics();
        }

    }

}
