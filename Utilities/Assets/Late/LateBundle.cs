using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Assets.Late
{
    public class LateBundle : LateAsset<AssetBundle>
    {
        private Assembly __assembly;
        private string __resource;
        public LateBundle(string Resource, Assembly assembly = null)
        {
            __assembly = assembly;
            if (__assembly == null)
            {
                __assembly = Assembly.GetCallingAssembly();
            }
            __resource = Resource;
        }
        protected override AssetBundle LoadAsset()
        {
            AssetBundle assetBundle = null;
            try
            {
                FungleAPIPlugin.Instance.Log.LogInfo($"Created {__resource}");
                assetBundle = AssetLoader.LoadBundle(__assembly, __resource);
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return assetBundle;
        }
    }
}
