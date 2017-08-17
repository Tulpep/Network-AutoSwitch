using System;

namespace Tulpep.NetworkAutoSwitch.NetworkStateLibrary
{
    static class LoggingNetworkState
    {
        public static void WriteMessage(string text, params object[] args)
        {
            Console.WriteLine(DateTime.Now + "\t" + string.Format(text, args));
        }
    }
}
