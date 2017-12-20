using System;
using System.Diagnostics;

namespace Tulpep.Network.NetworkStateService
{
    public static class Logging
    {
        public static void WriteConsoleMessage(string text, params object[] args)
        {
            Console.WriteLine(DateTime.Now + "\t" + string.Format(text, args));
        }

        public static void WriteMessageEventViewerInfo(string source, string text, params object[] args)
        {
            WriteMessageEventViewer(source, EventLogEntryType.Information, text, args);
        }

        public static void WriteMessageEventViewerError(string source, string text, params object[] args)
        {
            WriteMessageEventViewer(source, EventLogEntryType.Error, text, args);
        }

        public static void WriteMessageEventViewerWarning(string source, string text, params object[] args)
        {
            WriteMessageEventViewer(source, EventLogEntryType.Warning, text, args);
        }

        private static void WriteMessageEventViewer(string source, EventLogEntryType type, string text, params object[] args)
        {
            string sName = "Application";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, sName);

            EventLog e = new EventLog(sName)
            {
                Source = source
            };
            e.WriteEntry(string.Format(text, args), type);

            e.Close();
            e.Dispose();

            WriteConsoleMessage(source, string.Format(text, args));
        }

        public static void DeleteSource(string source)
        {
            if (EventLog.SourceExists(source))
                EventLog.DeleteEventSource(source);
            if (EventLog.Exists(source))
                EventLog.Delete(source);
        }
    }
}
