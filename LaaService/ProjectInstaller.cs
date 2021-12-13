using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace LaaService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            this.serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Username = null;
            this.serviceProcessInstaller1.Password = null;

            this.serviceInstaller1.Description = "Laa Service";
            this.serviceInstaller1.DisplayName = "Laa Service";
            this.serviceInstaller1.ServiceName = "LaaService";

            this.serviceInstaller1.StartType = ServiceStartMode.Automatic;

            this.serviceInstaller1.AfterInstall += delegate
            {
                ServiceController sc = new ServiceController("LaaService");
                sc.Start();
            };
        }
    }
}
