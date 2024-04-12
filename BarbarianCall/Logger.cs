using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Rage;

namespace BarbarianCall
{
    internal static class Logger
    {
        private static readonly object LockObject = new object();
        private static readonly ConcurrentQueue<string> LogQueue = new ConcurrentQueue<string>();
        private static readonly Thread LoggerThread;
        private static readonly AutoResetEvent LogEvent = new AutoResetEvent(false);
        private static readonly StringBuilder LogBuilder = new StringBuilder();
        private static readonly string LogFilePath = Path.Combine("Plugins", "LSPDFR", "BarbarianCall", "BarbarianCall.log");
        private static readonly int MaxLogFileSize = 10 * 1024 * 1024; // 10 MB
        private static long CurrentLogFileSize;

        static Logger()
        {
            LoggerThread = new Thread(LoggerWorker) { IsBackground = true };
            LoggerThread.Start();
        }

        internal static void Print(this string msg) => Game.LogTrivial(msg);
        internal static void ToLog(this string msg) => Log(msg, LogLevel.Info);
        internal static void ToLogDebug(this string msg) => Log(msg, LogLevel.Debug);
        internal static void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            string logConsoleMessage = $"[BCallout]: {message}";
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {message}";
            LogQueue.Enqueue(logMessage);
            LogEvent.Set();

            switch (logLevel)
            {
                case LogLevel.Debug:
                    if (DebugHelper.IsDebugBuild())
                    {
                        Game.LogTrivial(logConsoleMessage);
                    }
                    break;
                case LogLevel.Info:
                    Game.LogTrivial(logConsoleMessage);
                    break;
                case LogLevel.Warning:
                    Game.LogTrivial("WARNING: " + logConsoleMessage);
                    break;
                case LogLevel.Error:
                    Game.LogTrivial("ERROR: " + logConsoleMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        private static void LoggerWorker()
        {
            while (true)
            {
                LogEvent.WaitOne();

                lock (LockObject)
                {
                    while (LogQueue.TryDequeue(out string logMessage))
                    {
                        LogBuilder.AppendLine(logMessage);
                        CurrentLogFileSize += Encoding.UTF8.GetByteCount(logMessage + Environment.NewLine);

                        if (CurrentLogFileSize >= MaxLogFileSize)
                        {
                            RollOverLogFile();
                        }
                    }

                    File.AppendAllText(LogFilePath, LogBuilder.ToString());
                    LogBuilder.Clear();
                }
            }
        }

        private static void RollOverLogFile()
        {
            string backupFilePath = Path.Combine("Plugins", "LSPDFR", "BarbarianCall", $"BarbarianCall_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
            File.Move(LogFilePath, backupFilePath);
            CurrentLogFileSize = 0;
        }
    }

    internal enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}