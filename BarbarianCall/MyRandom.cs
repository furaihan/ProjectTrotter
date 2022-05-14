using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage.Native;

namespace BarbarianCall
{
    internal static class MyRandom
    {
        private static int Sample() => NativeFunction.Natives.xF2D49816A804D134<int>(0, int.MaxValue);
        internal static int Next() => Sample();
        internal static int Next(int max)
        {
            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), $"{nameof(max)} must be positive");
            }
            return Sample() % max;
        }

        internal static int Next(int min, int max)
        {
            if (min < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(min), $"{nameof(min)} must be positive");
            }
            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), $"{nameof(max)} must be positive");
            }
            if (min > max)
            {
                throw new ArgumentOutOfRangeException($"{nameof(max)} must greater than {nameof(min)}");
            }
            return (Sample() % (max - min)) + min;
        }
        internal static double NextDouble() => Sample() / int.MaxValue;
    }
}
