using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tulpep.NetworkAutoSwitch.NetworkService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            StringBuilder sbPathWIthParams = new StringBuilder(Context.Parameters["assemblypath"]);

            //Wrap the existing path in quotes if it isn't already
            if (!sbPathWIthParams[0].Equals("\""))
            {
                sbPathWIthParams.Insert(0, "\"");
                sbPathWIthParams.Append("\"");
            }


            //Add original parameters
            foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
            {
                sbPathWIthParams.Append(" " + arg);
            }

            Context.Parameters["assemblypath"] = sbPathWIthParams.ToString();
            base.OnBeforeInstall(savedState);

        }

    }


}
