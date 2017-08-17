using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Tulpep.NetworkAutoSwitch.NetworkStateLibrary
{
    public static class NetworkStateService
    {
        private static NetworkState _networkState = new NetworkState();


        public static void RefreshNetworkState(Priority priority)
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

        public static void EnableAllNics()
        {
            ChangeNicState(_networkState.WirelessAdapters, true);
            ChangeNicState(_networkState.WiredAdapters, true);
        }

        public static void ChangeNicState(IEnumerable<string> interfacesNames, bool enable)
        {
            if (enable) foreach (string nic in interfacesNames) ChangeNicState(nic, true);
            else foreach (string nic in interfacesNames) ChangeNicState(nic, false);
        }

        public static void LogChangeStateAdapters(IEnumerable<string> interfacesNames, bool enable)
        {
            foreach (string nic in interfacesNames) LogChangeStateAdapter(nic, true);
        }

        public static void LogChangeStateAdapter(string nicName, bool enable)
        {
            string action = enable ? "Enabling " : "Disabling ";
            LoggingNetworkState.WriteMessage(action + nicName);
        }

        public static void ChangeNicState(string interfaceName, bool enable)
        {
            string arguments;
            if (enable)
            {
                arguments = "interface set interface \"" + interfaceName + "\" enable";
            }
            else
            {
                arguments = "interface set interface \"" + interfaceName + "\" disable";
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
