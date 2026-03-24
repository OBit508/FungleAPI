using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Assets.Late
{
    /// <summary>
    /// An GifFile that is created when called
    /// </summary>
    public class LateGIF : LateAsset<GifFile>
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
        protected override GifFile LoadAsset()
        {
            GifFile gifFile = null;
            try
            {
                FungleAPIPlugin.Instance.Log.LogInfo($"Created {__resource}");
                gifFile = ResourceHelper.LoadGIF(__assembly, __resource, __pixelPerUnit);
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return gifFile;
        }
    }
}
