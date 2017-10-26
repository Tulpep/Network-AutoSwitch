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

        public static void WriteMessageEventViewerInfo(string source, string text, params object[] args)
        {
            WriteMessageEventViewer(source, text, args);
            EventLog.WriteEntry(source, string.Format(text, args), EventLogEntryType.Information);
        }

        public static void WriteMessageEventViewerError(string source, string text, params object[] args)
        {
            WriteMessageEventViewer(source, text, args);
            EventLog.WriteEntry(source, string.Format(text, args), EventLogEntryType.Error);
        }

        private static void WriteMessageEventViewer(string source, string text, params object[] args)
        {
            WriteMessage(string.Format(text, args));

            string sEvent = source + " Log";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, sEvent);
        }
    }
}
