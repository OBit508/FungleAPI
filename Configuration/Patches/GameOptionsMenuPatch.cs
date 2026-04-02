using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Networking;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Configuration.Patches
{
    [HarmonyPatch(typeof(GameOptionsMenu))]
    internal static class GameOptionsMenuPatch
    {
        public static TabType Type;
        public static bool Update;
        [HarmonyPatch("RefreshChildren")]
        [HarmonyPrefix]
        public static bool RefreshChildrenPrefix(GameOptionsMenu __instance)
        {
            AmongUs.GameOptions.GameModes gameModes = GameOptionsManager.Instance.CurrentGameOptions.GameMode;
            if (gameModes == AmongUs.GameOptions.GameModes.HideNSeek && gameModes == AmongUs.GameOptions.GameModes.SeekFools)
            {
                return true;
            }
            Update = true;
            __instance.Initialize();
            return false;
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(GameOptionsMenu __instance)
        {
            AmongUs.GameOptions.GameModes gameModes = GameOptionsManager.Instance.CurrentGameOptions.GameMode;
            if (gameModes != AmongUs.GameOptions.GameModes.HideNSeek && gameModes != AmongUs.GameOptions.GameModes.SeekFools)
            {
                try
                {
                    if (__instance.gameObject.active && __instance.Children == null || __instance.Children.Count == 0)
                    {
                        __instance.MapPicker.Initialize(20);
                        BaseGameSetting mapNameSetting = GameManager.Instance.GameSettingsList.MapNameSetting;
                        __instance.MapPicker.SetUpFromData(mapNameSetting, 20);
                        __instance.Children = new Il2CppSystem.Collections.Generic.List<OptionBehaviour>();
                        __instance.Children.Add(__instance.MapPicker);
                        TabBuilder.BuildTab(__instance, Type);
                        __instance.cachedData = GameOptionsManager.Instance.CurrentGameOptions;
                        for (int i = 0; i < __instance.Children.Count; i++)
                        {
                            OptionBehaviour optionBehaviour = __instance.Children[i];
                            if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
                            {
                                optionBehaviour.SetAsPlayer();
                            }
                        }
                        __instance.InitializeControllerNavigation();
                    }
                    else if (Update)
                    {
                        foreach (CategoryHeaderMasked categoryHeaderMasked in __instance.settingsContainer.GetComponentsInChildren<CategoryHeaderMasked>())
                        {
                            UnityEngine.Object.Destroy(categoryHeaderMasked.gameObject);
                        }
                        foreach (OptionBehaviour op in __instance.Children)
                        {
                            if (op != __instance.MapPicker)
                            {
                                UnityEngine.Object.Destroy(op.gameObject);
                            }
                        }
                        __instance.Children.Clear();
                        __instance.Children.Add(__instance.MapPicker);
                        TabBuilder.BuildTab(__instance, Type);
                        Update = false;
                    }
                    __instance.MapPicker.gameObject.SetActive(Type == TabType.VanillaSettingsTab);
                }
                catch { }
                return false;
            }
            return true;
        }
    }
}
