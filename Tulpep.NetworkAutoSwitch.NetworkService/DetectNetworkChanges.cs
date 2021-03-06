﻿using System;
using System.Net.NetworkInformation;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.NetworkService
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
            Logging.WriteConsoleMessage("Exiting now");
            NetworkStateService.EnableAllNics();
        }

    }

}
