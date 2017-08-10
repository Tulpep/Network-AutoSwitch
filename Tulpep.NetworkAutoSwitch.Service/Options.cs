using CommandLine;
using CommandLine.Text;

namespace Tulpep.NetworkAutoSwitch.Service
{
    class Options
    {

        [Option('i', "install", HelpText = "Install Service.", MutuallyExclusiveSet = "service")]
        public bool Install { get; set; }
        [Option('u', "uninstall",  HelpText = "Uninstall Service.", MutuallyExclusiveSet = "unservice")]
        public bool Uninstall { get; set; }
        [Option('p', "priority", HelpText = "Wireless or Wired Priority.", MutuallyExclusiveSet = "service")]
        public Priority Priority { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}