using CommandLine;
using System.ServiceProcess;

namespace Tulpep.NetworkAutoSwitch.Service
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
                _detectNetworkChanges = new DetectNetworkChanges(Options.Priority);
        }

        protected override void OnStop()
        {
            _detectNetworkChanges.StopNow();
        }
    }
}
