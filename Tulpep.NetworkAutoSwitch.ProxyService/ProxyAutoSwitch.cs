using CommandLine;
using System.ServiceProcess;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;
using System;
using System.IO;
using System.Reflection;

namespace Tulpep.NetworkAutoSwitch.ProxyService
{
    public partial class ProxyAutoSwitch : ServiceBase
    {
        DetectNetworkChanges _detectNetworkChanges;
        static Options Options { get; set; }

        public ProxyAutoSwitch()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Options = new Options();
            if (Parser.Default.ParseArguments(args, Options))
            {
                Priority priority = Options.Priority == Priority.None ? ManageProxyState.GetPriorityConfig() : Options.Priority;
                _detectNetworkChanges = new DetectNetworkChanges(priority);
            }
        }
         
        protected override void OnSessionChange(SessionChangeDescription cd)
        {
            ManageProxyState.AnalyzeNow(ManageProxyState.GetPriorityConfig());
        }

        protected override void OnStop()
        {
            _detectNetworkChanges.StopNow();
        }

    }
}
