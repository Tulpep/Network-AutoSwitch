using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

namespace Tulpep.NetworkAutoSwitch.Logic
{
    static class ManageNetworkState
    {

        private static NetworkState _networkState = new NetworkState();

        public static void AnalyzeNow()
        {
            RefreshNetworkState();
            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Wireless " + _networkState.WirelessIsUp);
            Console.WriteLine("Wired " + _networkState.WiredIsUp);
            if (_networkState.WirelessIsUp && _networkState.WiredIsUp)
            {
                ChangeNicState(_networkState.WirelessAdapters, true);
                ChangeNicState(_networkState.WiredAdapters, false);
            }
            else if (!_networkState.WirelessIsUp && !_networkState.WiredIsUp)
            {
                ChangeNicState(_networkState.WiredAdapters, true);
                ChangeNicState(_networkState.WiredAdapters, true);
            }
        }


        private static void RefreshNetworkState()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            IEnumerable<NetworkInterface> wirelessAdapters = nics.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            _networkState.WirelessIsUp = wirelessAdapters.Any(x => x.OperationalStatus == OperationalStatus.Up);
            foreach (NetworkInterface nic in wirelessAdapters)
            {
                _networkState.WirelessAdapters.Add(nic.Name);
            }

            IEnumerable<NetworkInterface> wiredAdapters = nics.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
            _networkState.WiredIsUp = wiredAdapters.Any(x => x.OperationalStatus == OperationalStatus.Up);
            foreach (NetworkInterface nic in wiredAdapters)
            {
                _networkState.WiredAdapters.Add(nic.Name);
            }
        }


        private static void ChangeNicState(IEnumerable<string> interfacesNames, bool enable)
        {
            if (enable) foreach (string nic in interfacesNames) ChangeNicState(nic, true);
            else foreach (string nic in interfacesNames) ChangeNicState(nic, false);
        }

        private static void ChangeNicState(string interfaceName, bool enable)
        {
            string arguments = "interface set interface \"" + interfaceName + "\" enable";
            if (!enable) arguments = "interface set interface \"" + interfaceName + "\" disable";
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Console.WriteLine("netsh " + arguments);
            Process process = new Process { StartInfo = processStartInfo };
            process.Start();
            process.WaitForExit();
        }
    }

}
