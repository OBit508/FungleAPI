using AsmResolver.PE.DotNet.StrongName;
using FungleAPI.Extensions;
using FungleAPI.ModCompatibility;
using FungleAPI.Ship;
using Il2CppInterop.Runtime;
using Il2CppSystem.Runtime.Remoting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FungleAPI.Utilities
{
    /// <summary>
    /// A prefab utility class
    /// </summary>
    public static class PrefabUtils
    {
        /// <summary>
        /// Finds a prefab of the specified type using an optional predicate
        /// </summary>
        public static T FindPrefab<T>(Predicate<T> predicate = null) where T : UnityEngine.Object
        {
            if (predicate == null)
            {
                predicate = new Predicate<T>(x => x);
            }
            return Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(T))).FirstOrDefault(x => predicate(x.SafeCast<T>())).SafeCast<T>();
        }
        /// <summary>
        /// Finds a prefabs of the specified type
        /// </summary>
        public static IEnumerable<T> FindPrefabs<T>() where T : UnityEngine.Object
        {
            List<T> list = new List<T>();
            foreach (UnityEngine.Object @object in Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(T))))
            {
                list.Add(@object.SafeCast<T>());
            }
            return list;
        }
    }
}