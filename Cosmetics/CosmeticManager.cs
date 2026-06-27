
using FungleAPI.Event;
using FungleAPI.Event.BelpInEx;
using FungleAPI.PluginLoading;
using FungleAPI.Cosmetics.Colors;
using HarmonyLib;
using Innersloth.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FungleAPI.Event.Api;
using FungleAPI.Api;

namespace FungleAPI.Cosmetics
{
    /// <summary>
    /// A class that helps the cosmetic system to work
    /// </summary>
    public static class CosmeticManager
    {
        internal const float r = 0.618033988749895f;
        internal static float hue = UnityEngine.Random.value;
        internal static Dictionary<string, UnityEngine.Object> Assets = new Dictionary<string, UnityEngine.Object>();
        internal static Dictionary<CosmeticData, StringNames> CosmeticsNames = new Dictionary<CosmeticData, StringNames>();

        public static List<CustomHat> AllHats = new List<CustomHat>();
        public static List<CustomSkin> AllSkins = new List<CustomSkin>();
        public static List<CustomVisor> AllVisors = new List<CustomVisor>();
        public static List<CustomPet> AllPets = new List<CustomPet>();
        public static List<CustomNamePlate> AllNamePlates = new List<CustomNamePlate>();

        public static List<BaseCosmetic> AllCosmetics = new List<BaseCosmetic>();

        public static List<CustomColor> AllColors = new List<CustomColor>();
        public static List<CustomColor> SpecialColors = new List<CustomColor>();

        public static AssetReference Register(UnityEngine.Object @object, string id)
        {
            Assets.Add(id, @object);
            return new AssetReference(id);
        }

        public static bool IsSpecialColor(int colorId, out SpecialColor specialColor)
        {
            specialColor = (SpecialColor)SpecialColors.FirstOrDefault(c => c.ColorId == colorId);
            return specialColor != null;
        }
        public static bool IsCustom(int colorId)
        {
            return !FungleApiPlugin.Plugin.Cosmetics.Colors.Any(c => c.ColorId == colorId);
        }
        public static bool IsInvalid(int colorId)
        {
            if (colorId == 255 && AllColors.Count < 244)
            {
                return true;
            }
            return false;
        }
        public static void Add(ModCosmetics modCosmetics)
        {
            modCosmetics.Initialize();
            if (modCosmetics.Colors != null && modCosmetics.Colors.Count() > 0)
            {
                AllColors.AddRange(modCosmetics.Colors);
                SpecialColors.AddRange(modCosmetics.Colors.Where(c => c is SpecialColor));
            }
            if (modCosmetics.Hats != null)
            {
                AllHats.AddRange(modCosmetics.Hats);
                AllCosmetics.AddRange(modCosmetics.Hats);
            }
            if (modCosmetics.Skins != null)
            {
                AllSkins.AddRange(modCosmetics.Skins);
                AllCosmetics.AddRange(modCosmetics.Skins);
            }
            if (modCosmetics.Visors != null)
            {
                AllVisors.AddRange(modCosmetics.Visors);
                AllCosmetics.AddRange(modCosmetics.Visors);
            }
            if (modCosmetics.Pets != null)
            {
                AllPets.AddRange(modCosmetics.Pets);
                AllCosmetics.AddRange(modCosmetics.Pets);
            }
            if (modCosmetics.NamePlates != null)
            {
                AllNamePlates.AddRange(modCosmetics.NamePlates);
                AllCosmetics.AddRange(modCosmetics.NamePlates);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static Color GetBaseColor(int colorId)
        {
            CustomColor color = AllColors.FirstOrDefault(c => c.ColorId == colorId);
            if (color is SpecialColor specialColor)
            {
                return specialColor.BaseColor;
            }
            return color == null ? Color.black : color.PlayerColor;
        }
        /// <summary>
        /// 
        /// </summary>
        public static Color GetValidProxyColor()
        {
            hue = (hue + r) % 1f;
            return Color.HSVToRGB(hue, 0.85f, 0.95f);
        }
        public static ModCosmetics RegisterCosmetics(Type type, ModPlugin plugin)
        {
            ModCosmetics cosmetics = (ModCosmetics)Activator.CreateInstance(type);
            CosmeticManager.Add(cosmetics);
            plugin.BasePlugin.Log.LogInfo("Registered Cosmetics " + type.Name);
            return cosmetics;
        }

        [EventRegister]
        internal static void SetCosmetics(FirstSceneLoadEvent firstSceneLoadEvent)
        {
            List<Color32> PlayerColors = new List<Color32>();
            List<Color32> ShadowColors = new List<Color32>();
            List<StringNames> ColorsNames = new List<StringNames>();
            for (int i = 0; i < AllColors.Count; i++)
            {
                CustomColor color = AllColors[i];
                PlayerColors.Add(color.PlayerColor);
                ShadowColors.Add(color.BackColor);
                ColorsNames.Add(color.ColorName);
                color.ColorId = i;
            }
            Palette.PlayerColors = PlayerColors.ToArray();
            Palette.ShadowColors = ShadowColors.ToArray();
            Palette.ColorNames = ColorsNames.ToArray();
            Palette.TextColors = PlayerColors.ToArray();
            Palette.TextOutlineColors = ShadowColors.ToArray();

            foreach (BaseCosmetic baseCosmetic in AllCosmetics)
            {
                baseCosmetic.Initialize();
            }
        }
    }
}
