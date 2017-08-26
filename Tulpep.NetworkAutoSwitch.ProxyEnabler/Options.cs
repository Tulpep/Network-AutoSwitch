using CommandLine;
using CommandLine.Text;

namespace Tulpep.NetworkAutoSwitch.ProxyEnabler
{
    class Options
    {
        [Option('e', "enable", Required = true, HelpText = "'Enable' or 'Disable' proxy.")]
        public int Enable { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}