﻿using CommandLine;
using System;
using System.Configuration.Install;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Tulpep.Network.NetworkStateService;
using Tulpep.NetworkAutoSwitch.NetworkStateLibrary;

namespace Tulpep.NetworkAutoSwitch.ProxyService
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

            ExtractProxyEnabler();

            if (Environment.UserInteractive)
            {                
                const string exeFileName = Constants.SERVICE_NAME + ".exe";
                const string installStateFileName = Constants.SERVICE_NAME + ".InstallState";

                string currentPath = Assembly.GetExecutingAssembly().Location;
                string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string serviceInSystem32Path = Path.Combine(system32Path, exeFileName);
                bool runningFromSystem32 = string.Equals(currentPath, serviceInSystem32Path, StringComparison.OrdinalIgnoreCase);
                Logging.WriteMessage("Starting from " + currentPath);
                if (Options.Install)
                {
                    if (Options.Priority == Priority.None)
                    {
                        Logging.WriteMessage("You must select a priority");
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
                    StartService(Constants.SERVICE_NAME, 500, Options.Priority);
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
                ServiceBase[] ServicesToRun = new ServiceBase[]
                {
                    new ProxyAutoSwitch()
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
            Logging.WriteMessage("Starting Windows Service {0} with timeout of {1} ms", serviceName, timeoutMilliseconds);
            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
            service.Start(new string[] { "-p", priority.ToString() });
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            Logging.WriteMessage("Service running");

            string currentPath = Assembly.GetExecutingAssembly().Location;
            string system32Path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            string configInSystem32Path = Path.Combine(system32Path, Constants.TXT_CONFIG_NAME);
            Byte[] info = null;

            using (FileStream fs = File.Create(configInSystem32Path))
            {
                info = new UTF8Encoding(true).GetBytes(priority == Priority.Wired ? "1" : "0");
                fs.Write(info, 0, info.Length);

            }
        }

        private static void ExtractProxyEnabler()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("costura.proxyenabler.exe.compressed"))
            using (var decompressStream = new DeflateStream(stream, CompressionMode.Decompress))
            using (var fileStream = new FileStream(Constants.PROXY_ENABLER_EXE_NAME, FileMode.Create))
            {
                decompressStream.CopyTo(fileStream);
            }
        }


    }
}

