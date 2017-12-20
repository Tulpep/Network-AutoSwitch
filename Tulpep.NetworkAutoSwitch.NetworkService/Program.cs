﻿using CommandLine;
using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.NetworkService
{
    static class Program
    {
        static Options Options { get; set; }

        private static string currentPath = Assembly.GetExecutingAssembly().Location;
        private static string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        private static string serviceInSystem32Path = Path.Combine(system32Path, Constants.EXE_FILE_NAME);
        private static bool runningFromSystem32 = string.Equals(currentPath, serviceInSystem32Path, StringComparison.OrdinalIgnoreCase);

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
                
                
                Logging.WriteConsoleMessage("Starting from " + currentPath);

                if (Options.Install)
                {
                    if (Options.Priority == Priority.None)
                    {
                        Logging.WriteConsoleMessage("No priority is selected.");
                        Console.WriteLine(Options.GetUsage());
                        return 1;
                    }
                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Constants.SERVICE_NAME);
                    if (ctl != null)
                    {
                        Uninstall();
                        Install();
                    }
                    else
                        Install();
                    return 0;
                }
                if (Options.Uninstall)
                {
                    Uninstall();
                    return 0;
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
            Logging.WriteMessageEventViewerError(Constants.SERVICE_NAME, e.ExceptionObject.ToString());
            Environment.Exit(1);
        }


        private static void StartService(string serviceName, int timeoutMilliseconds, Priority priority)
        {
            Logging.WriteConsoleMessage("Starting Windows Service {0} with timeout of {1} ms", serviceName, timeoutMilliseconds);
            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
            service.Start(new string[] { "-p", priority.ToString() });
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            Logging.WriteConsoleMessage("Service running");

            string currentPath = Assembly.GetExecutingAssembly().Location;
            string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            string configInSystem32Path = Path.Combine(system32Path, Constants.SERVICE_NAME + "Config.txt");
            Byte[] info = null;

            using (FileStream fs = File.Create(configInSystem32Path))
            {
                info = new UTF8Encoding(true).GetBytes(priority == Priority.Wired ? "1" : "0");
                fs.Write(info, 0, info.Length);

            }
        }

        private static void Install()
        {
            if (runningFromSystem32) Logging.WriteConsoleMessage("The file is located in {0}, you can not delete it from there after the service installation", currentPath);
            else
            {
                Logging.WriteConsoleMessage("Copying file to " + serviceInSystem32Path);
                File.Copy(currentPath, serviceInSystem32Path, true);
            }
            ManagedInstallerClass.InstallHelper(new string[] { "/LogFile=", "/LogToConsole=true", serviceInSystem32Path });
            Logging.WriteConsoleMessage("Service Installed");
            StartService(Constants.SERVICE_NAME, 500, Options.Priority);
            SetRecoveryOptions(Constants.SERVICE_NAME);
        }

        private static void Uninstall()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/uninstall", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location });
            Logging.WriteConsoleMessage("Service Uninstalled");
            if (!runningFromSystem32)
            {
                if (File.Exists(serviceInSystem32Path))
                {
                    Logging.WriteConsoleMessage("Removing file from " + serviceInSystem32Path);
                    File.Delete(serviceInSystem32Path);
                }

                string installStatePath = Path.Combine(system32Path, Constants.INSTALL_STATE_FILE_NAME);
                if (File.Exists(installStatePath))
                {
                    Logging.WriteConsoleMessage("Removing file from " + installStatePath);
                    File.Delete(installStatePath);
                }
            }
        }

        private static void SetRecoveryOptions(string SERVICE_NAME)
        {
            int exitCode;
            using (var process = new Process())
            {
                var startInfo = process.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // tell Windows that the service should restart if it fails
                startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/60000", SERVICE_NAME);

                process.Start();
                process.WaitForExit();

                exitCode = process.ExitCode;
            }

            if (exitCode != 0)
                throw new InvalidOperationException();
        }
    }
}
