using CommandLine;
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
        static Options Options { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static int Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            Options = new Options();
            if (!Parser.Default.ParseArguments(args, Options))
                return 1;


            if (Environment.UserInteractive)
            {
                const string serviceName = "NetworkAutoSwitch";
                const string exeFileName = "NetworkAutoSwitch.exe";
                const string installStateFileName = "NetworkAutoSwitch.InstallState";

                string currentPath = Assembly.GetExecutingAssembly().Location;
                string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string serviceInSystem32Path = Path.Combine(system32Path, exeFileName);
                bool runningFromSystem32 = string.Equals(currentPath, serviceInSystem32Path, StringComparison.OrdinalIgnoreCase);
                Logging.WriteMessage("Starting from " + currentPath);
                if (Options.Install)
                {
                    if (Options.Priority == Priority.None)
                    {
                        Logging.WriteMessage("No priority is selected.");
                        Console.WriteLine(Options.GetUsage());
                        return 1;
                    }
                    if (runningFromSystem32) Logging.WriteMessage("The file is located in {0}, you can not delete it from there after the service installation", currentPath);
                    else
                    {
                        Logging.WriteMessage("Copying file to " + serviceInSystem32Path);
                        File.Copy(currentPath, serviceInSystem32Path, true);
                    }
                    ManagedInstallerClass.InstallHelper(new string[] { "/LogFile=", "/LogToConsole=true", serviceInSystem32Path });
                    Logging.WriteMessage("Service Installed");
                    int timeout = 5000;
                    Logging.WriteMessage("Starting Windows Service {0} with timeout of {1} ms", serviceName, timeout);
                    StartService(serviceName, timeout);
                    Logging.WriteMessage("Service running");
                    return 0;
                }
                if (Options.Uninstall)
                {
                    ManagedInstallerClass.InstallHelper(new string[] { "/uninstall", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location });
                    Logging.WriteMessage("Service Uninstalled");
                    if (!runningFromSystem32)
                    {
                        if (File.Exists(serviceInSystem32Path))
                        {
                            Logging.WriteMessage("Removing file from " + serviceInSystem32Path);
                            File.Delete(serviceInSystem32Path);
                        }

                        string installStatePath = Path.Combine(system32Path, installStateFileName);
                        if (File.Exists(installStatePath))
                        {
                            Logging.WriteMessage("Removing file from " + installStatePath);
                            File.Delete(installStatePath);
                        }
                    }
                    return 0;
                }

                if (Options.Priority == Priority.None)
                {
                    Logging.WriteMessage("No priority is selected.");
                    Console.WriteLine(Options.GetUsage());
                    return 1;
                }
                var exitEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, eventArgs) => {
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                };
                DetectNetworkChanges detectNetworkChanges = new DetectNetworkChanges(Options.Priority);
                Console.WriteLine("Use parameters --install or --uninstall to use as Windows Service");
                Console.WriteLine("Press CTRL + C to exit...");
                exitEvent.WaitOne();
                detectNetworkChanges.StopNow();
                return 0;
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
