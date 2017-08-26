using CommandLine;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using Tulpep.Network.NetworkStateService;

namespace Tulpep.NetworkAutoSwitch.ProxyEnabler
{
    class Program
    {
        static Options Options { get; set; }

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        public const string REGISTRY_KEY_IS = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            Options = new Options();
            if (!Parser.Default.ParseArguments(args, Options))
                return 1;

            RegistryKey registry = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_IS, true);

            registry.SetValue("ProxyEnable", Options.Enable? 1 : 0);

            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

            return 0;
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logging.WriteMessage(e.ExceptionObject.ToString());
            Environment.Exit(1);
        }


    }
}
