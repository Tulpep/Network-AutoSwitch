using CommandLine;
using System.ServiceProcess;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.NetworkService
{
    public partial class NetworkAutoSwitch : ServiceBase
    {
        DetectNetworkChanges _detectNetworkChanges;
        static Options Options { get; set; }

        public NetworkAutoSwitch()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Options = new Options();
            if (Parser.Default.ParseArguments(args, Options))
            {
                Priority priority = Options.Priority == Priority.None ? ManageNetworkState.GetPriorityConfig() : Options.Priority;
                _detectNetworkChanges = new DetectNetworkChanges(priority);
            }
        }

        protected override void OnStop()
        {
            _detectNetworkChanges.StopNow();
        }
    }
}
