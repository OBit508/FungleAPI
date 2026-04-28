using AsmResolver.PE.DotNet.ReadyToRun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities
{
    /// <summary>
    /// Extensions for the enumerables
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Get a random index from a item
        /// </summary>
        public static int RandomIdx<T>(this IEnumerable<T> self)
        {
            return UnityEngine.Random.Range(0, self.Count<T>());
        }
        /// <summary>
        /// Create a new IEnumerable with all the given IEnumerable values but shuffled
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> values)
        {
            List<T> list = values.ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.RandomRangeInt(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
            return list;
        }
        /// <summary>
        /// Shuffle every List item
        /// </summary>
        public static void Shuffle<T>(this List<T> values)
        {
            IEnumerable<T> enumerable = values.SimpleCast<IEnumerable<T>>().Shuffle();
            values.Clear();
            values.AddRange(enumerable);
        }
        /// <summary>
        /// Get a random item from a Enumerable
        /// </summary>
        public static T Random<T>(this IEnumerable<T> values)
        {
            if (values.Count() == 0)
            {
                return default;
            }
            return values.ToArray()[UnityEngine.Random.RandomRangeInt(0, values.Count() - 1)];
        }
        /// <summary>
        /// Get the index of the given item
        /// </summary>
        public static int GetIndex<T>(this IEnumerable<T> values, T thing)
        {
            T[] list = values.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Equals(thing))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
