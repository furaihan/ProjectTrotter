using System;
using Rage.Native;

namespace ProjectTrotter
{
    internal static class MyRandom
    {
        private static int Sample() => NativeFunction.Natives.GET_RANDOM_MWC_INT_IN_RANGE<int>(0, int.MaxValue);
        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        internal static int Next() => Sample();
        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="max">The exclusive upper bound of the random number to be generated. 
        /// max must be greater than or equal to 0. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Exception thrown when max is less than 0. </exception>
        internal static int Next(int max)
        {
            if (max < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(max), $"{nameof(max)} must be positive");
            }
            return Sample() % max;
        }
        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="min"> The inclusive lower bound of the random number returned. </param>
        /// <param name="max"> The exclusive upper bound of the random number returned. </param>
        /// <exception cref="ArgumentOutOfRangeException"> 
        /// Exception thrown when min is less than 0, max is less than 0, or max is less than min. 
        /// </exception>
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
        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        internal static double NextDouble() => Sample() / int.MaxValue;
    }
}
