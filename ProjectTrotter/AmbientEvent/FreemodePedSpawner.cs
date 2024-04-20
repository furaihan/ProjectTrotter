using System;
using Rage;

namespace ProjectTrotter.AmbientEvent
{
    internal class FreemodePedSpawner
    {
        internal static void LoopMethod()
        {          
            while (true)
            {
                GameFiber.Sleep(MyRandom.Next((int)TimeSpan.FromSeconds(300).TotalMilliseconds, (int)TimeSpan.FromSeconds(800).TotalMilliseconds));
            }
        }
    }
}
