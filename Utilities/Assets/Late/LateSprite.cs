using FungleAPI.PluginLoading;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.Glyphs.UnityUI.UnityUITextMeshProGlyphHelper;

namespace FungleAPI.Utilities.Assets.Late
{
    /// <summary>
    /// An Sprite that is created when called
    /// </summary>
    public class LateSprite : LateAsset<Sprite>
    {
        private Assembly __assembly;
        private string __resource;
        private float __pixelPerUnit;
        public LateSprite(string Resource, float PixelPerUnit, Assembly assembly = null)
        {
            __assembly = assembly;
            if (__assembly == null)
            {
                __assembly = Assembly.GetCallingAssembly();
            }
            __resource = Resource;
            __pixelPerUnit = PixelPerUnit;
        }
        protected override Sprite LoadAsset()
        {
            Sprite sprite = null;
            try
            {
                FungleAPIPlugin.Instance.Log.LogInfo($"Created {__resource}");
                sprite = AssetLoader.LoadSprite(__assembly, __resource, __pixelPerUnit);
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return sprite;
        }
    }
}
