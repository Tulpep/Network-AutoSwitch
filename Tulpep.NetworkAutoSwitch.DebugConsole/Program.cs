using System;
using Tulpep.NetworkAutoSwitch.Logic;

namespace Tulpep.NetworkAutoSwitch.DebugConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to exit...");
            new DetectNetworkChanges();
            Console.ReadLine();
        }
    }
}
