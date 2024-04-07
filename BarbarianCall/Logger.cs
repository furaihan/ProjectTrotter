using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;

namespace BarbarianCall
{
    internal static class Logger
    {
        internal static void Print(this string msg) => Game.Console.Print(msg);
        private static readonly Stopwatch LogStopwatch = new();
        private static readonly StringBuilder LogBuilder = new();
        internal static void ToLogDebug(this string micin)
        {
#if DEBUG
            ToLog(micin);
#endif
        }
        internal static void ToLogDebug(this string micin, bool makeUppercase)
        {
#if DEBUG
            ToLog(micin, makeUppercase);
#endif
        }
        internal static void ToLog(this string micin) => ToLog(micin, false);
        internal static void ToLog(this string micin, bool makeUppercase)
        {
            if (!LogStopwatch.IsRunning) LogStopwatch.Start();
            string text = makeUppercase ? micin.ToUpper() : micin;
            Game.LogTrivial(makeUppercase ? "[BARBARIAN-CALL]: " + text : "[BarbarianCall]: " + text);
            LogBuilder.AppendLine(string.Format("[{0}]: {1}", DateTime.Now.ToString("d MMM yyyy - HH:mm:ss:FFFFF"), text));
            if (LogStopwatch.ElapsedMilliseconds > 20000)
            {
                LogStopwatch.Restart();
                string path = Path.Combine("Plugins", "LSPDFR", "BarbarianCall", "BarbarianCall.log");
                if (File.Exists(path)) File.AppendAllText(path, LogBuilder.ToString());
                else Game.LogTrivial("Your log file doesnt exist");
                LogBuilder.Clear();
            }
        }
    }
}
