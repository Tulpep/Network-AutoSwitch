using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Text;

namespace Tulpep.NetworkAutoSwitch.Service
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

            //Add desired parameters
            sbPathWIthParams.Append(" test");

            Context.Parameters["assemblypath"] = sbPathWIthParams.ToString();
            File.WriteAllText(@"C:\log.txt", sbPathWIthParams.ToString());
            base.OnBeforeInstall(savedState);

        }

    }


}
