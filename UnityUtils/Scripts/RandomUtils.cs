using System;
using System.Collections.Generic;

namespace UnityUtils
{
    /// <summary>
    /// A static utility class for common randomization tasks.
    /// </summary>
    public static class RandomUtils
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Randomly selects and returns one item from a list of provided arguments of a specific type.
        /// This is the type-safe generic version.
        /// </summary>
        /// <typeparam name="T">The type of items to choose from.</typeparam>
        /// <param name="items">A variable number of arguments of type T to choose from.</param>
        /// <returns>The randomly chosen item of type T.</returns>
        /// <exception cref="ArgumentException">Thrown if no arguments are provided.</exception>
        public static T Choose<T>(params T[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("Cannot choose from an empty list of items.", nameof(items));
            }

            int randomIndex = _random.Next(0, items.Length);
            return items[randomIndex];
        }

        /// <summary>
        /// A generic overload that works with any collection (like Lists or Arrays) that can be enumerated.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to choose from (e.g., a List<GameObject>).</param>
        /// <returns>The randomly chosen item of type T.</returns>
        /// <exception cref="ArgumentException">Thrown if the collection is null or empty.</exception>
        public static T Choose<T>(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentException("Cannot choose from a null collection.", nameof(collection));
            }

            // Use LINQ to count the items. This is a bit less efficient than using an array's Length
            // but works for ANY collection type. We materialize it to a list to avoid multiple enumerations.
            var list = collection as IList<T> ?? new List<T>(collection);

            if (list.Count == 0)
            {
                throw new ArgumentException("Cannot choose from an empty collection.", nameof(collection));
            }

            return list[_random.Next(0, list.Count)];
        }

        /// <summary>
        /// The original non-generic version for choosing from mixed types.
        /// </summary>
        public static object Choose(params object[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("Cannot choose from an empty list of items.", nameof(items));
            }

            int randomIndex = _random.Next(0, items.Length);
            return items[randomIndex];
        }
    } 
}