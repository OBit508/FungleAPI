using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Assets.Late
{
    public class LateBundleAsset<T> : LateAsset<T> where T : UnityEngine.Object
    {
        private string __name;
        private LateBundle __bundle;
        public LateBundleAsset(string Name, LateBundle Bundle)
        {
            __name = Name;
            __bundle = Bundle;
        }
        protected override T LoadAsset()
        {
            if (__bundle.Asset == null) return null;
            T asset = null;
            try
            {
                FungleAPIPlugin.Instance.Log.LogInfo($"Loaded bundle asset {__name}");
                asset = AssetLoader.LoadAsset<T>(__bundle, __name);
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to load bundle asset message:\n" + ex.Message);
            }
            return asset;
        }
    }
}
