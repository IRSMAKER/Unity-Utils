using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    /// <summary>
    /// A static utility class for common randomization tasks.
    /// </summary>
    public static class RandomUtils
    {
        private static readonly Random _random = new Random();

        #region Choose Method

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

        #endregion

        #region Boolean Methods

        /// <summary>
        /// Returns true or false based on a given probability (0.0 to 1.0).
        /// </summary>
        public static bool Chance(float probability)
        {
            if (probability <= 0) return false;
            if (probability >= 1) return true;
            return _random.NextDouble() < probability;
        }

        #endregion

        #region Vector Methods (Unity Specific)

        /// <summary> Returns a random point on the circumference of a circle with a radius of 1. </summary>
        public static Vector2 OnUnitCircle()
        {
            float angle = (float)(_random.NextDouble() * 2 * Math.PI);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        /// <summary> Returns a random point inside a circle with a radius of 1 (uniform distribution). </summary>
        public static Vector2 InUnitCircle()
        {
            return OnUnitCircle() * Mathf.Sqrt((float)_random.NextDouble());
        }

        /// <summary> Returns a random point on the surface of a sphere with a radius of 1. </summary>
        public static Vector3 OnUnitSphere()
        {
            // Using Unity's built-in is often best, but for a pure C# implementation:
            float u = (float)(_random.NextDouble() * 2.0 - 1.0);
            float t = (float)(_random.NextDouble() * 2.0 * Math.PI);
            float f = Mathf.Sqrt(1.0f - u * u);
            return new Vector3(f * Mathf.Cos(t), f * Mathf.Sin(t), u);
        }

        /// <summary> Returns a random point inside a sphere with a radius of 1 (uniform distribution). </summary>
        public static Vector3 InUnitSphere()
        {
            return OnUnitSphere() * Mathf.Pow((float)_random.NextDouble(), 1f / 3f);
        }

        #endregion

        #region Quaternion Methods

        public static Quaternion Rotation2D()
        {
            return Quaternion.Euler(0, 0, (float)(_random.NextDouble() * 360.0));
        }

        #endregion

        #region Color Methods

        /// <summary>
        /// Returns a random, vibrant color by randomizing the Hue.
        /// Saturation and Value can be optionally constrained for better results.
        /// </summary>
        public static Color ColorHSV(float saturationMin = 0.5f, float saturationMax = 1.0f, float valueMin = 0.8f, float valueMax = 1.0f)
        {
            float hue = (float)_random.NextDouble();
            float saturation = (float)(_random.NextDouble() * (saturationMax - saturationMin) + saturationMin);
            float value = (float)(_random.NextDouble() * (valueMax - valueMin) + valueMin);
            return Color.HSVToRGB(hue, saturation, value);
        }

        #endregion
    }

    /// <summary>
    /// Contains extension methods for randomization.
    /// </summary>
    public static class RandomExtensions
    {
        private static readonly System.Random _random = new System.Random();

        #region Choose Extentions

        /// <summary>
        /// Randomly selects multiple unique elements from a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> ChooseMultiple<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            // Shuffle the entire sequence and take the first 'count' items.
            // This is efficient and guarantees uniqueness.
            return source.OrderBy(x => Guid.NewGuid()).Take(count);
        }

        /// <summary>
        /// Filters a collection based on a condition and then randomly chooses one element from the result,
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="weightSelector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T Choose<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<T, float> weightSelector)
        {
            if (source == null || predicate == null || weightSelector == null)
                throw new ArgumentNullException();

            // 1. Filter the collection first
            var filteredItems = source.Where(predicate).ToList();

            if (filteredItems.Count == 0)
                throw new InvalidOperationException("No items matched the predicate.");

            // 2. Perform a weighted choice on the *filtered* list
            float totalWeight = filteredItems.Sum(item => weightSelector(item));

            if (totalWeight <= 0)
                throw new InvalidOperationException("Total weight of filtered items must be positive.");

            float randomPoint = (float)(new Random().NextDouble() * totalWeight);

            foreach (var item in filteredItems)
            {
                float weight = weightSelector(item);
                if (randomPoint < weight)
                    return item;
                randomPoint -= weight;
            }

            // Fallback in case of floating point inaccuracies
            return filteredItems.Last();
        }

        /// <summary>
        /// Filters a collection based on a condition and then randomly chooses one element from the result.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="source">The source collection to choose from.</param>
        /// <param name="predicate">The condition (lambda expression) that items must satisfy to be considered.</param>
        /// <returns>A randomly chosen item that matches the predicate.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no items in the collection match the predicate.</exception>
        public static T Choose<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            // --- The Core Logic ---
            // 1. Filter the collection using the user's condition.
            // 2. Convert the result to a list to work with indices.
            var filteredList = source.Where(predicate).ToList();

            // --- Robustness Check ---
            if (filteredList.Count == 0)
            {
                throw new InvalidOperationException("No items in the collection matched the predicate.");
            }

            // --- Random Selection ---
            // 3. Choose a random item from the *filtered* list.
            return filteredList[_random.Next(0, filteredList.Count)];
        }

        /// <summary>
        /// A safe version of Choose that returns the default value (e.g., null) if no items match the condition,
        /// instead of throwing an exception.
        /// </summary>
        public static T ChooseOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null || predicate == null)
            {
                return default(T); // Returns null for reference types, 0 for int, etc.
            }

            var filteredList = source.Where(predicate).ToList();

            if (filteredList.Count == 0)
            {
                return default(T);
            }

            return filteredList[_random.Next(0, filteredList.Count)];
        }

        /// <summary>
        /// Randomly selects one item from the entire collection.
        /// This is a more convenient way to call the functionality in our other RandomUtils class.
        /// </summary>
        public static T Choose<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = source as IList<T> ?? source.ToList();

            if (list.Count == 0)
            {
                throw new InvalidOperationException("Cannot choose from an empty collection.");
            }

            return list[_random.Next(0, list.Count)];
        }

        #endregion

        #region Vector Methods

        /// <summary>
        /// Returns a random point within the bounding box.
        /// </summary>
        public static Vector3 GetRandomPoint(this Bounds bounds)
        {
            return new Vector3(
                (float)(_random.NextDouble() * bounds.size.x + bounds.min.x),
                (float)(_random.NextDouble() * bounds.size.y + bounds.min.y),
                (float)(_random.NextDouble() * bounds.size.z + bounds.min.z)
            );
        }

        /// <summary>
        /// Returns a random point within the collider's bounds.
        /// </summary>
        public static Vector3 GetRandomPoint(this Collider collider)
        {
            return collider.bounds.GetRandomPoint();
        }

        #endregion

        #region Collection Extensions

        /// <summary>
        /// Shuffles the elements of a list in place using the Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]); // Tuple swap
            }
        } 

        #endregion
    }
}