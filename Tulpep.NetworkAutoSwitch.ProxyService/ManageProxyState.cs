using murrayju.ProcessExtensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.ProxyService
{
    public class ManageProxyState
    {
        public static void AnalyzeNow(Priority priority)
        {
            NetworkState networkState = NetworkStateService.RefreshNetworkState(priority);

            Logging.WriteMessage("Wireless: {0} | Wired: {1} | State: {2}", networkState.WirelessStatus, networkState.WiredStatus, priority);

            if (networkState.WiredStatus == OperationalStatus.Up && networkState.WirelessStatus == OperationalStatus.Down)
            {
                ModifyProxyState(priority == Priority.Wired ? 1 : 0);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Down)
            {
                ModifyProxyState(priority == Priority.Wireless ? 1 : 0);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Up && networkState.WiredStatus == OperationalStatus.Up)
            {
                ModifyProxyState(priority == Priority.Wired ? 1 : 0);
            }
            else if (networkState.WirelessStatus == OperationalStatus.Down && networkState.WiredStatus == OperationalStatus.Down)
            {
                ModifyProxyState(priority == Priority.Wireless ? 1 : 0);
            }
        }

        public static void ModifyProxyState(int enableProxy)
        {
            string arguments = "-e " + enableProxy;

            if (Environment.UserInteractive)
            {
                ProcessStartInfo psi = new ProcessStartInfo()
                {
                    FileName = Constants.PROXY_ENABLER_EXE_NAME,
                    Arguments = arguments,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                };
                Process.Start(psi);
            }
            else
            {
                if (!CheckActiveSessions()) return;
                ProcessExtensions.StartProcessAsCurrentUser(Constants.PROXY_ENABLER_EXE_NAME, Constants.PROXY_ENABLER_EXE_NAME + " " + arguments, null, false);
            }

            Logging.WriteMessage((enableProxy == 1 ? "Enable" : "Disable") + " Proxy Server");
        }

        private static bool  CheckActiveSessions(){
            ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
            SelectQuery queryRead = new SelectQuery("select * from Win32_ComputerSystem");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, queryRead))
            {
                foreach (var item in searcher.Get())
                {
                    ManagementObjectCollection collection = searcher.Get();
                    foreach (ManagementObject obj in collection)
                    {
                        if(obj["UserName"] != null && !string.IsNullOrEmpty(obj["UserName"].ToString()))
                            return true;
                    }
                    
                }
            }

            return false;
        }

        public static Priority GetPriorityConfig()
        {
            string currentPath = Assembly.GetExecutingAssembly().Location;
            string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            string configInSystem32Path = Path.Combine(system32Path, Constants.TXT_CONFIG_NAME);

            string firstLine;

            using (StreamReader reader = new StreamReader(configInSystem32Path))
            {
                firstLine = reader.ReadLine() ?? "";
            }

            Priority priority = Priority.None;

            if(firstLine == "1")
            {
                priority = Priority.Wired;
            }
            else if(firstLine == "0")
            {
                priority = Priority.Wireless;
            }

            return priority;
        }


    }

}
