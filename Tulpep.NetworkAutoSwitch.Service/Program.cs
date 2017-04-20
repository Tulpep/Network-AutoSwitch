using System;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace Tulpep.NetworkAutoSwitch.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static int Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            if (Environment.UserInteractive)
            {
                switch (string.Concat(args))
                {
                    case "--install":
                        string currentPath = Assembly.GetExecutingAssembly().Location;
                        string pathOfService = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "NetworkAutoSwitch.exe");
                        if(currentPath == pathOfService) Logging.WriteMessage("The file is located in {0}, you can not delete it from there after the service installation", currentPath);
                        else
                        {
                            Logging.WriteMessage("Copying file to " + pathOfService);
                            File.Copy(currentPath, pathOfService, true);
                        }
                        ManagedInstallerClass.InstallHelper(new string[] { "/LogFile=", "/LogToConsole=true", pathOfService });
                        Logging.WriteMessage("Service Installed");
                        const string serviceName = "NetworkAutoSwitch";
                        int timeout = 5000;
                        Logging.WriteMessage("Starting Windows Service {0} with timeout of {1} ms", serviceName, timeout);
                        StartService(serviceName, timeout);
                        Logging.WriteMessage("Service running");
                        return 0;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/uninstall", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location });
                        Logging.WriteMessage("Service Uninstalled");
                        return 0;
                    default:
                        Console.WriteLine("Use parameters --install or --uninstall to use as Windows Service");
                        var exitEvent = new ManualResetEvent(false);
                        Console.CancelKeyPress += (sender, eventArgs) => {
                            eventArgs.Cancel = true;
                            exitEvent.Set();
                        };

                        DetectNetworkChanges detectNetworkChanges = new DetectNetworkChanges();
                        Console.WriteLine("Press CTRL + C to exit...");
                        exitEvent.WaitOne();
                        detectNetworkChanges.StopNow();
                        return 0;

                }
            }
            else
            {
                ServiceBase[]  ServicesToRun = new ServiceBase[]
                {
                    new NetworkAutoSwitch()
                };
                ServiceBase.Run(ServicesToRun);
                return 0;
            }

        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logging.WriteMessage(e.ExceptionObject.ToString());
            Environment.Exit(1);
        }


        private static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

    }
}
