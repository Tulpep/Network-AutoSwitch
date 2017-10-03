using System;
using System.Diagnostics;

namespace Tulpep.Network.NetworkStateService
{
    public static class Logging
    {
        public static void WriteMessage(string text, params object[] args)
        {
            Console.WriteLine(DateTime.Now + "\t" + string.Format(text, args));
        }

        public static void WriteMessageEventViewer(string text, params object[] args)
        {
            WriteMessage(string.Format(text, args));

            string sSource = "ProxyAutoSwitch";
            string sEvent = "ProxyAutoSwitch Log";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sEvent);

            EventLog.WriteEntry(sSource, string.Format(text, args), EventLogEntryType.Information);

        }
    }
}
