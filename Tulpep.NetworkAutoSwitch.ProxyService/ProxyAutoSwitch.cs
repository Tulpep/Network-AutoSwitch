using CommandLine;
using System.ServiceProcess;

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
                _detectNetworkChanges = new DetectNetworkChanges(Options.Priority);
        }

        protected override void OnStop()
        {
            _detectNetworkChanges.StopNow();
        }
    }
}
