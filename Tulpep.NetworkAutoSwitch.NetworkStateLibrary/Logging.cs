using System;

namespace Tulpep.Network.NetworkStateService
{
    public static class Logging
    {
        public static void WriteMessage(string text, params object[] args)
        {
            Console.WriteLine(DateTime.Now + "\t" + string.Format(text, args));
        }
    }
}
