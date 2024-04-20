using System;
using System.Diagnostics;
using Rage;

namespace ProjectTrotter.AmbientEvent
{
    internal static class EventHandler
    {
        public static bool IsAnyEventRunning = false;
        public static void StartEventLoop()
        {
            GameFiber.StartNew(delegate
            {
                Stopwatch sw = new();
                TimeSpan timer = new(0, 0, MyRandom.Next(250, 850));
                $"Starting event loop".ToLog();
                $"First ambient event will start at {(DateTime.Now + timer).ToLongTimeString()}".ToLog();
                sw.Start();
                while (true)
                {
                    GameFiber.Yield();
                    if (sw.Elapsed > timer)
                    {
                        
                    }
                }
            });
        }
        public static void CreateEvent()
        {
            if (IsAnyEventRunning)
            {
                "Another event is already running, aborting this".ToLog();
                return;
            }

        }
    }
}
