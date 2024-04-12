using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using BarbarianCall.Freemode;
using BarbarianCall.Extensions;

namespace BarbarianCall.AmbientEvent
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
