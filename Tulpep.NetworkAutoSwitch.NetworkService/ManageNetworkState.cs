using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.NetworkService
{
    public class ManageNetworkState
    {
        public static void AnalyzeNow(Priority priority)
        {
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

        public static Priority GetPriorityConfig()
        {
            string currentPath = Assembly.GetExecutingAssembly().Location;
            string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            string configInSystem32Path = Path.Combine(system32Path, "NetworkAutoSwitchConfig.txt");

            string firstLine;

            using (StreamReader reader = new StreamReader(configInSystem32Path))
            {
                firstLine = reader.ReadLine() ?? "";
            }

            Priority priority = Priority.None;

            if (firstLine == "1")
            {
                priority = Priority.Wired;
            }
            else if (firstLine == "0")
            {
                priority = Priority.Wireless;
            }

            return priority;
        }
    }

}
