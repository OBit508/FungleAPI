using FungleAPI.Api;
using FungleAPI.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Assets.Late
{
    /// <summary>
    /// An GIF that is created when called
    /// </summary>
    public class LateGIF : LateAsset<GIF>
    {
        private Assembly __assembly;
        private string __resource;
        private float __pixelPerUnit;
        public LateGIF(string Resource, float PixelPerUnit, Assembly assembly = null)
        {
            __assembly = assembly;
            if (__assembly == null)
            {
                __assembly = Assembly.GetCallingAssembly();
            }
            __resource = Resource;
            __pixelPerUnit = PixelPerUnit;
        }
        protected override GIF LoadAsset()
        {
            GIF gifFile = null;
            try
            {
                FungleApiPlugin.Instance.Log.LogInfo($"Created {__resource}");
                gifFile = AssetLoader.LoadGIF(__assembly, __resource, __pixelPerUnit);
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return gifFile;
        }
    }
}
