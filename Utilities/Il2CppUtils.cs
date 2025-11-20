using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.IO;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Steamworks;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities
{
    public static class Il2CppUtils
    {
        public static bool Exists<T>(this List<T> list, Predicate<T> match)
        {
            return list.Exists(ToIl2CppPredicate(match));
        }
        public static T Find<T>(this List<T> list, Predicate<T> match)
        {
            return list.Find(ToIl2CppPredicate(match));
        }
        public static int FindIndex<T>(this List<T> list, Predicate<T> match)
        {
            return list.FindIndex(ToIl2CppPredicate(match));
        }
        public static int FindIndex<T>(this List<T> list, int startIndex, int count, Predicate<T> match)
        {
            return list.FindIndex(startIndex, count, ToIl2CppPredicate(match));
        }
        public static void RemoveAll<T>(this List<T> list, Predicate<T> match)
        {
            list.RemoveAll(ToIl2CppPredicate(match));
        }
        public static bool TrueForAll<T>(this List<T> list, Predicate<T> match)
        {
            return list.TrueForAll(ToIl2CppPredicate(match));
        }
        public static Il2CppStructArray<T> ToIl2CppStructArray<T>(this List<T> list) where T : unmanaged
        {
            return (T[])list.ToArray();
        }
        public static Il2CppSystem.Predicate<T> ToIl2CppPredicate<T>(this Predicate<T> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            return DelegateSupport.ConvertDelegate<Il2CppSystem.Predicate<T>>(predicate);
        }
        public static Predicate<T> ToSystemPredicate<T>(this Il2CppSystem.Predicate<T> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            return new Predicate<T>(obj => predicate.Invoke(obj));
        }
        public static List<T> ToIl2CppList<T>(this System.Collections.Generic.List<T> list)
        {
            List<T> values = new List<T>();
            foreach (T item in list)
            {
                values.Add(item);
            }
            return values;
        }
        public static System.Collections.Generic.List<T> ToSystemList<T>(this List<T> list)
        {
            System.Collections.Generic.List<T> values = new System.Collections.Generic.List<T>();
            foreach (T item in list)
            {
                values.Add(item);
            }
            return values;
        }
        public static System.Collections.Generic.Dictionary<TKey, TValue> ToSystemDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            System.Collections.Generic.Dictionary<TKey, TValue> dic = new System.Collections.Generic.Dictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                dic.Add(pair.key, pair.value);
            }
            return dic;
        }
        public static Dictionary<TKey, TValue> ToIl2CppDictionary<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, TValue> dictionary)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            foreach (System.Collections.Generic.KeyValuePair<TKey, TValue> pair in dictionary)
            {
                dic.Add(pair.Key, pair.Value);
            }
            return dic;
        }
        public static T FirstOrDefault<T>(this List<T> enumerable, Predicate<T> match)
        {
            foreach (T m in enumerable)
            {
                if (match(m))
                {
                    return m;
                }
            }
            return default(T);
        }
        public static bool Any<T>(this List<T> enumerable, Predicate<T> match)
        {
            foreach (T m in enumerable)
            {
                if (match(m))
                {
                    return true;
                }
            }
            return false;
        }
        public static TValue[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue>.ValueCollection valueCollection)
        {
            List<TValue> values = new List<TValue>();
            foreach (TValue item in valueCollection)
            {
                values.Add(item);
            }
            return values.ToArray();
        }
        public static TKey[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection valueCollection)
        {
            List<TKey> values = new List<TKey>();
            foreach (TKey item in valueCollection)
            {
                values.Add(item);
            }
            return values.ToArray();
        }
        public static List<TSource> OrderBy<TSource, TKey>(this List<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ToArray().OrderBy(keySelector).ToList().ToIl2CppList();
        }
    }
}
