using System.Net.NetworkInformation;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.Service
{
    public class ManageNetworkState
    {
        private static NetworkState _networkState = new NetworkState();


        public static void AnalyzeNow(Priority priority)
        {
            NetworkStateService.RefreshNetworkState(priority);
            Logging.WriteMessage("Wireless: {0} | Wired: {1}", _networkState.WirelessStatus, _networkState.WiredStatus);
            if (_networkState.WirelessStatus == OperationalStatus.Up && _networkState.WiredStatus == OperationalStatus.Up)
            {
                NetworkStateService.ChangeNicState(_networkState.WirelessAdapters, priority == Priority.Wireless ? true : false);
                NetworkStateService.LogChangeStateAdapters(_networkState.WirelessAdapters, priority == Priority.Wireless ? true : false);
                NetworkStateService.ChangeNicState(_networkState.WiredAdapters, priority == Priority.Wireless ? false : true);
                NetworkStateService.LogChangeStateAdapters(_networkState.WirelessAdapters, priority == Priority.Wireless ? false : true);
            }
            else if (_networkState.WirelessStatus == OperationalStatus.Down && _networkState.WiredStatus == OperationalStatus.Down)
            {
                NetworkStateService.ChangeNicState(_networkState.WiredAdapters, true);
                NetworkStateService.LogChangeStateAdapters(_networkState.WirelessAdapters, true);
                NetworkStateService.ChangeNicState(_networkState.WirelessAdapters, true);
                NetworkStateService.LogChangeStateAdapters(_networkState.WirelessAdapters, true);
            }
        }
    }

}
