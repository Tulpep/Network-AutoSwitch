using murrayju.ProcessExtensions;
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
                    ModifyProxyState(priority == Priority.Wired);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Down)
            {
                    ModifyProxyState(priority == Priority.Wireless);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Up)
            {
                    ModifyProxyState(true);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Down && networkState.WiredStatus == OperationalStatus.Down)
            {
                    ModifyProxyState(false);
            }
        }


        public static void ModifyProxyState(bool enableProxy)
        {
          // if(enableProxy) ProcessExtensions.StartProcessAsCurrentUser("calc.exe");
            //else ProcessExtensions.StartProcessAsCurrentUser("calc.exe");
            Logging.WriteMessage((enableProxy ? "Enable" : "Disable") + " Proxy Server");
        }
    }

}
