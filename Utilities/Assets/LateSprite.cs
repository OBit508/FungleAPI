using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.Glyphs.UnityUI.UnityUITextMeshProGlyphHelper;

namespace FungleAPI.Utilities.Assets
{
    /// <summary>
    /// An Sprite that is created later during the loadAssets event
    /// </summary>
    public class LateSprite
    {
        public LateSprite(string Resource, float PixelPerUnit, ModPlugin modPlugin = null)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ResourceHelper.loadAssets += delegate
            {
                try
                {
                    if (modPlugin == null)
                    {
                        modPlugin = ModPluginManager.GetModPlugin(assembly);
                        if (modPlugin == null)
                        {
                            FungleAPIPlugin.Instance.Log.LogError($"Failed to find the ModPlugin from the \"{Resource}\" asset.");
                            return;
                        }
                    }
                    Sprite = ResourceHelper.LoadSprite(modPlugin, Resource, PixelPerUnit);
                }
                catch (Exception ex)
                {
                    FungleAPIPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
                }
            };
        }
        /// <summary>
        /// The created asset instance
        /// </summary>
        public Sprite Sprite { get; private set; }
        public static implicit operator Sprite(LateSprite lateSprite)
        {
            return lateSprite.Sprite;
        }
    }
}
