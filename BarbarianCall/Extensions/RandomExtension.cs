using System;
using System.Collections.Generic;
using System.Linq;

namespace BarbarianCall.Extensions
{
    /// <summary>
    /// Extension methods for shuffling and selecting random elements from various collection types.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Shuffles the elements in the specified list using the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                // Pick a random element from the remaining elements
                int k = MyRandom.Next(n--);
                // Swap the randomly picked element with the last element
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Returns a random element from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to select a random element from.</param>
        /// <param name="shuffle">Whether to shuffle the list before selecting the random element.</param>
        /// <returns>A random element from the list.</returns>
        public static T PickRandomItem<T>(this IList<T> list, bool shuffle = false)
        {
            if (!list.Any())
                return default;

            if (shuffle)
                list.Shuffle();

            return list[MyRandom.Next(list.Count)];
        }

        /// <summary>
        /// Returns a random element from the specified enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to select a random element from.</param>
        /// <param name="shuffle">Whether to shuffle the enumerable before selecting the random element.</param>
        /// <returns>A random element from the enumerable.</returns>
        public static T PickRandomItem<T>(this IEnumerable<T> enumerable, bool shuffle = false)
        {
            if (!enumerable.Any())
                return default;

            if (shuffle)
                enumerable = enumerable.ToList().OrderBy(_ => MyRandom.Next());

            return enumerable.ElementAt(MyRandom.Next(enumerable.Count()));
        }

        /// <summary>
        /// Returns a random value from the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enumType">The enum type to select a random value from.</param>
        /// <returns>A random value from the specified enum type.</returns>
        public static T PickRandomValue<T>(this Enum enumType) where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(MyRandom.Next(values.Length));
        }

        /// <summary>
        /// Returns a random number of elements from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to select random elements from.</param>
        /// <param name="numOfElements">The number of random elements to select.</param>
        /// <param name="shuffle">Whether to shuffle the list before selecting the random elements.</param>
        /// <returns>A list containing the specified number of random elements from the original list.</returns>
        public static IList<T> PickRandomItems<T>(this IList<T> list, int numOfElements, bool shuffle = false)
        {
            List<T> givenList = new(list);
            return InternalPickRandomItems(givenList, numOfElements, shuffle);
        }

        /// <summary>
        /// Returns a random number of elements from the specified enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to select random elements from.</param>
        /// <param name="numOfElements">The number of random elements to select.</param>
        /// <param name="shuffle">Whether to shuffle the enumerable before selecting the random elements.</param>
        /// <returns>A list containing the specified number of random elements from the original enumerable.</returns>
        public static IList<T> PickRandomItems<T>(this IEnumerable<T> enumerable, int numOfElements, bool shuffle = false)
        {
            List<T> givenList = enumerable.ToList();
            return InternalPickRandomItems(givenList, numOfElements, shuffle);
        }

        private static IList<T> InternalPickRandomItems<T>(IList<T> list, int numOfElements, bool shuffle)
        {
            if (shuffle)
                list.Shuffle();

            return list.OrderBy(_ => MyRandom.Next()).Take(numOfElements).ToList();
        }

        /// <summary>
        /// Returns a random number of elements from the specified list that satisfy the given predicate.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="items">The list to select random elements from.</param>
        /// <param name="predicate">The predicate to filter elements.</param>
        /// <param name="shuffle">Whether to shuffle the list before selecting the random elements.</param>
        /// <returns>A list containing the specified number of random elements from the original list that satisfy the given predicate.</returns>
        public static IList<T> PickRandomItems<T>(this IEnumerable<T> items, Func<T, bool> predicate, bool shuffle = false)
        {
            List<T> filteredList = items.Where(predicate).ToList();
            return filteredList.PickRandomItems(filteredList.Count, shuffle);
        }
    }
}
