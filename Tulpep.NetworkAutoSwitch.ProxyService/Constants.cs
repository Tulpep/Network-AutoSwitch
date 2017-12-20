namespace Tulpep.NetworkAutoSwitch.ProxyService
{
    public static class Constants
    {
        public const string SERVICE_NAME = "ProxyAutoSwitch";
        public const string SERVICE_DISPLAY_NAME = "Tulpep Proxy AutoSwitch Service";
        public const string SERVICE_DESCRIPTION = "For detailed information visit https://github.com/Tulpep/Network-AutoSwitch";

        public const string PROXY_ENABLER_EXE_NAME = "ProxyEnabler.exe";
        public const string KEY_CONFIG_PATH = @"Software\Tulpep\" + SERVICE_NAME;
        public const string KEY_CONFIG_NAME = SERVICE_NAME + "Key";

        public const string EXE_FILE_NAME = SERVICE_NAME + ".exe";
        public const string INSTALL_STATE_FILE_NAME = SERVICE_NAME + ".InstallState";
    }
}
