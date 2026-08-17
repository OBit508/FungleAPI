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
        private bool __loop;
        public LateGIF(string Resource, float PixelPerUnit, bool Loop = true, Assembly assembly = null)
        {
            __assembly = assembly;
            if (__assembly == null)
            {
                __assembly = Assembly.GetCallingAssembly();
            }
            __resource = Resource;
            __pixelPerUnit = PixelPerUnit;
            __loop = Loop;
        }
        protected override GIF LoadAsset()
        {
            GIF gifFile = null;
            try
            {
                FungleApiPlugin.Instance.Log.LogInfo($"Created {__resource}");
                gifFile = AssetLoader.LoadGIF(__assembly, __resource, __pixelPerUnit, __loop);
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return gifFile;
        }
    }
}
