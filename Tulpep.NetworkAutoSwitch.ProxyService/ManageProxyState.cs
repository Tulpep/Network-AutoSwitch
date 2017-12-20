using Microsoft.Win32;
using murrayju.ProcessExtensions;
using System;
using System.Diagnostics;
using System.Management;
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

            Logging.WriteConsoleMessage("Wireless: {0} | Wired: {1} | State: {2}", networkState.WirelessStatus, networkState.WiredStatus, priority);

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

            Logging.WriteConsoleMessage((enableProxy == 1 ? "Enable" : "Disable") + " Proxy Server");
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
            string firstLine = string.Empty;

            using (RegistryKey hkcu = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey proxyAutoSwitch = hkcu.CreateSubKey(Constants.KEY_CONFIG_PATH, RegistryKeyPermissionCheck.Default))
                {
                    firstLine = proxyAutoSwitch.GetValue(Constants.KEY_CONFIG_NAME)?.ToString();
                }
            }
            
            Priority priority = Priority.None;

            if (firstLine == null)
            {
                Logging.WriteMessageEventViewerError(Constants.SERVICE_NAME, "RegistryKey not found " + Constants.KEY_CONFIG_PATH + Constants.KEY_CONFIG_NAME);
            }
            else
            {
                if(firstLine.Equals(Decimal.Zero.ToString()))
                {
                    Logging.WriteMessageEventViewerWarning(Constants.SERVICE_NAME, $"RegistryKey set in zero {Constants.KEY_CONFIG_PATH + Constants.KEY_CONFIG_NAME}, please set a value" );
                }
                else if (firstLine.Equals(Decimal.One.ToString()))
                {
                    priority = Priority.Wired;
                }
                else if(firstLine.Equals(Decimal.Zero.ToString()))
                {
                    priority = Priority.Wireless;
                }
            }

            return priority;
        }
    }

}
