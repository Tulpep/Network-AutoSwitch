﻿using System.Net.NetworkInformation;
using System.Windows.Forms;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;
using Tulpep.NetworkAutoSwitch.UtilityLibrary;

namespace Tulpep.NetworkAutoSwitch.Service
{
    public class ManageNetworkState
    {
        public static void AnalyzeNow(Priority priority)
        {
            MessageBox.Show("Begin.");
            NetworkState networkState = NetworkStateService.RefreshNetworkState(priority);
            Logging.WriteMessage("Wireless: {0} | Wired: {1}", networkState.WirelessStatus, networkState.WiredStatus);
            if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Up)
            {
                NetworkStateService.ChangeNicState(networkState.WirelessAdapters, priority == Priority.Wireless ? true : false);
                NetworkStateService.LogChangeStateAdapters(networkState.WirelessAdapters, priority == Priority.Wireless ? true : false);
                NetworkStateService.ChangeNicState(networkState.WiredAdapters, priority == Priority.Wireless ? false : true);
                NetworkStateService.LogChangeStateAdapters(networkState.WirelessAdapters, priority == Priority.Wireless ? false : true);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Down && networkState.WiredStatus == OperationalStatus.Down)
            {
                NetworkStateService.ChangeNicState(networkState.WiredAdapters, true);
                NetworkStateService.LogChangeStateAdapters(networkState.WirelessAdapters, true);
                NetworkStateService.ChangeNicState(networkState.WirelessAdapters, true);
                NetworkStateService.LogChangeStateAdapters(networkState.WirelessAdapters, true);
            }
        }
    }

}
