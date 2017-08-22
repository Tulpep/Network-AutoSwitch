using CommandLine;
using CommandLine.Text;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.NetworkService
{
    class Options
    {
        [Option('p', "priority", HelpText = "'Wireless' or 'Wired' priority.", MutuallyExclusiveSet = "service")]
        public Priority Priority { get; set; }

        [Option('i', "install", HelpText = "Install Service.", MutuallyExclusiveSet = "service")]
        public bool Install { get; set; }
        [Option('u', "uninstall",  HelpText = "Uninstall Service.", MutuallyExclusiveSet = "unservice")]
        public bool Uninstall { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}