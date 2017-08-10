using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

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
            Context.Parameters["assemblypath"] += "\" /service";
            base.OnBeforeInstall(savedState);
        }
    }


}
