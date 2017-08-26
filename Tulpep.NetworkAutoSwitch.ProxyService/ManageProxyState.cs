using murrayju.ProcessExtensions;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.ProxyService
{
    public class ManageProxyState
    {
        public static void AnalyzeNow(Priority priority)
        {
         
            NetworkState networkState = NetworkStateService.RefreshNetworkState(priority);
            Logging.WriteMessage("Wireless: {0} | Wired: {1}" , networkState.WirelessStatus, networkState.WiredStatus);

            if(networkState.WiredStatus == OperationalStatus.Up && networkState.WirelessStatus == OperationalStatus.Down)
            {
                    ModifyProxyState(priority == Priority.Wired ? 1 : 0);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Down)
            {
                    ModifyProxyState(priority == Priority.Wireless ? 1 : 0);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Up)
            {
                    ModifyProxyState(1);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Down && networkState.WiredStatus == OperationalStatus.Down)
            {
                    ModifyProxyState(0);
            }
        }


        public static void ModifyProxyState(int enableProxy)
        {
            string parameters = "-e " + enableProxy;
            if (Environment.UserInteractive)
            {
                Process.Start(Constants.PROXY_ENABLER_EXE_NAME, parameters);
            }
            else
            {
                ProcessExtensions.StartProcessAsCurrentUser(Constants.PROXY_ENABLER_EXE_NAME, parameters);
            }

            Logging.WriteMessage((enableProxy == 1 ? "Enable" : "Disable") + " Proxy Server");
        }
    }

}
