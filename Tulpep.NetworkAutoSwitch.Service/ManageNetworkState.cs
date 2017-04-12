using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

namespace Tulpep.NetworkAutoSwitch.Service
{
    static class ManageNetworkState
    {

        private static NetworkState _networkState = new NetworkState();

        public static void AnalyzeNow()
        {
            RefreshNetworkState();
            Logging.WriteMessage("Wireless: {0} | Wired: {1}", _networkState.WirelessStatus, _networkState.WiredStatus);
            if (_networkState.WirelessStatus == OperationalStatus.Up && _networkState.WiredStatus == OperationalStatus.Up)
            {
                ChangeNicState(_networkState.WirelessAdapters, true);
                ChangeNicState(_networkState.WiredAdapters, false);
            }
            else if (_networkState.WirelessStatus == OperationalStatus.Down && _networkState.WiredStatus == OperationalStatus.Down)
            {
                ChangeNicState(_networkState.WiredAdapters, true);
                ChangeNicState(_networkState.WiredAdapters, true);
            }
        }


        public static void EnableAllNics()
        {
            ChangeNicState(_networkState.WirelessAdapters, true);
            ChangeNicState(_networkState.WiredAdapters, true);
        }

        private static void RefreshNetworkState()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            IEnumerable<NetworkInterface> wirelessAdapters = nics.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            if (wirelessAdapters.Any(x => x.OperationalStatus == OperationalStatus.Up)) _networkState.WirelessStatus = OperationalStatus.Up;
            else _networkState.WirelessStatus = OperationalStatus.Down;
            foreach (NetworkInterface nic in wirelessAdapters) _networkState.WirelessAdapters.Add(nic.Name);


            IEnumerable<NetworkInterface> wiredAdapters = nics.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
            if (wiredAdapters.Any(x => x.OperationalStatus == OperationalStatus.Up)) _networkState.WiredStatus = OperationalStatus.Up;
            else _networkState.WiredStatus = OperationalStatus.Down;
            foreach (NetworkInterface nic in wiredAdapters) _networkState.WiredAdapters.Add(nic.Name);
        }


        private static void ChangeNicState(IEnumerable<string> interfacesNames, bool enable)
        {
            if (enable) foreach (string nic in interfacesNames) ChangeNicState(nic, true);
            else foreach (string nic in interfacesNames) ChangeNicState(nic, false);
        }

        private static void ChangeNicState(string interfaceName, bool enable)
        {
            string arguments;
            if (enable)
            {
                arguments = "interface set interface \"" + interfaceName + "\" enable";
                Logging.WriteMessage("Enabling " + interfaceName);
            }
            else
            {
                arguments = "interface set interface \"" + interfaceName + "\" disable";
                Logging.WriteMessage("Disabling " + interfaceName);
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = new Process { StartInfo = processStartInfo };
            process.Start();
            process.WaitForExit();
        }
    }

}
