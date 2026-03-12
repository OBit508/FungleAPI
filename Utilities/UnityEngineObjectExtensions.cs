using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities
{
    /// <summary>
    /// Extensions for the Unity Objects
    /// </summary>
    public static class UnityEngineObjectExtensions
    {
        /// <summary>
        /// Add HideAndDontSave flag
        /// </summary>
        public static T DontDestroy<T>(this T obj) where T : UnityEngine.Object
        {
            obj.hideFlags |= HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(obj);
            return obj;
        }
        /// <summary>
        /// Add DontUnloadUnusedAsset flag
        /// </summary>
        public static T DontUnload<T>(this T obj) where T : UnityEngine.Object
        {
            obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return obj;
        }
        /// <summary>
        /// Get or Add a component to a GameObject
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T result = obj.GetComponent<T>();
            if (result == null)
            {
                result = obj.AddComponent<T>();
            }
            return result;
        }
    }
}
