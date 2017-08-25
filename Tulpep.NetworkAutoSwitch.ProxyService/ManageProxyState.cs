using Microsoft.Win32;
using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
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

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public const string REGISTRY_KEY_IS = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

        public static void SetProxyState(bool proxyEnabled)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_IS, true);

            registry.SetValue("ProxyEnable", proxyEnabled ? 1 : 0);

            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }


        public static void ModifyProxyState(bool EnableProxy)
        {
            SetProxyState(EnableProxy);
            Logging.WriteMessage((EnableProxy ? "Enable" : "Disable") + " Proxy Server");
        }
    }

}
