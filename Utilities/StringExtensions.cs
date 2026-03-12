using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities
{
    /// <summary>
    /// Extensions for the strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a random char from the string
        /// </summary>
        public static char GetRandomChar(this string str)
        {
            if (str.Length == 0)
            {
                return default;
            }
            return str[UnityEngine.Random.RandomRangeInt(0, str.Length)];
        }
        /// <summary>
        /// Returns this string but shuffled
        /// </summary>
        public static string Shuffle(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            List<char> list = str.ToCharArray().ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return new string(list.ToArray());
        }
    }
}
