using FungleAPI.Api;
using FungleAPI.Assets;
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

namespace FungleAPI.Assets.Late
{
    /// <summary>
    /// An Sprite that is created when called
    /// </summary>
    public class LateSprite : LateAsset<Sprite>
    {
        private Assembly __assembly;
        private string __resource;
        private float __pixelsPerUnit;
        private bool __readable;
        public LateSprite(string Resource, float PixelsPerUnit, bool readable = false, Assembly assembly = null)
        {
            __assembly = assembly;
            if (__assembly == null)
            {
                __assembly = Assembly.GetCallingAssembly();
            }
            __resource = Resource;
            __pixelsPerUnit = PixelsPerUnit;
            __readable = readable;
        }
        protected override Sprite LoadAsset()
        {
            Sprite sprite = null;
            try
            {
                FungleApiPlugin.Instance.Log.LogInfo($"Created {__resource}");
                sprite = AssetLoader.LoadSprite(__assembly, __resource, __pixelsPerUnit, __readable);
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return sprite;
        }
    }
}
