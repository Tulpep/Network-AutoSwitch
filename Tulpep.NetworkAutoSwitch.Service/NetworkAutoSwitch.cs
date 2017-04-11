using System.ServiceProcess;

namespace Tulpep.NetworkAutoSwitch.Service
{
    public partial class NetworkAutoSwitch : ServiceBase
    {
        DetectNetworkChanges _detectNetworkChanges;
        public NetworkAutoSwitch()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _detectNetworkChanges = new DetectNetworkChanges();
        }

        protected override void OnStop()
        {
            _detectNetworkChanges.StopNow();
        }
    }
}
