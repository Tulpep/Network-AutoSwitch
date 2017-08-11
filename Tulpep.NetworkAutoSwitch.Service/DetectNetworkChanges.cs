﻿using System;
using System.Net.NetworkInformation;

namespace Tulpep.NetworkAutoSwitch.Service
{
    public class DetectNetworkChanges
    {
        public DetectNetworkChanges(Priority priority)
        {
            ManageNetworkState.AnalyzeNow(priority);
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler((sender, e) => NetworkAddressChanged(sender, e, priority));

        }

        private void NetworkAddressChanged(object sender, EventArgs e, Priority priority)
        {
            ManageNetworkState.AnalyzeNow(priority);
        }

        public void StopNow()
        {
            Logging.WriteMessage("Exiting now");
            ManageNetworkState.EnableAllNics();
        }

    }

}
