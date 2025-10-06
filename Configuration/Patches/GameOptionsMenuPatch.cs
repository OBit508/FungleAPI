using Epic.OnlineServices;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
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
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(GameOptionsMenu __instance)
        {
            if (__instance.Children == null || __instance.Children.Count == 0)
            {
                __instance.MapPicker.Initialize(20);
                BaseGameSetting mapNameSetting = GameManager.Instance.GameSettingsList.MapNameSetting;
                __instance.MapPicker.SetUpFromData(mapNameSetting, 20);
                __instance.Children = new Il2CppSystem.Collections.Generic.List<OptionBehaviour>();
                __instance.Children.Add(__instance.MapPicker);
                if (GameSettingMenuPatch.currentPlugin == FungleAPIPlugin.Plugin)
                {
                    __instance.CreateSettings();
                }
                else
                {
                    CreateSettings(__instance);
                }
                __instance.cachedData = GameOptionsManager.Instance.CurrentGameOptions;
                for (int i = 0; i < __instance.Children.Count; i++)
                {
                    OptionBehaviour optionBehaviour = __instance.Children[i];
                    optionBehaviour.OnValueChanged = new Action<OptionBehaviour>(__instance.ValueChanged);
                    if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
                    {
                        optionBehaviour.SetAsPlayer();
                    }
                }
                __instance.InitializeControllerNavigation();
            }
            else
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
                if (GameSettingMenuPatch.currentPlugin == FungleAPIPlugin.Plugin)
                {
                    __instance.CreateSettings();
                }
                else
                {
                    CreateSettings(__instance);
                }
            }
            __instance.MapPicker.gameObject.SetActive(GameSettingMenuPatch.currentPlugin == FungleAPIPlugin.Plugin);
            return false;
        }
        public static void CreateSettings(GameOptionsMenu menu)
        {
            float num = 2;
            foreach (SettingsGroup group in GameSettingMenuPatch.currentPlugin.Settings.Groups)
            {
                CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                categoryHeaderMasked.SetHeader(group.GroupName, 20);
                categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                num -= 0.63f;
                foreach (ModdedOption option in group.Options)
                {
                    OptionBehaviour op = option.CreateOption(menu.settingsContainer);
                    op.LabelBackground.enabled = true;
                    op.transform.localPosition = new Vector3(0.952f, num, -2f);
                    op.SetClickMask(menu.ButtonClickMask);
                    op.OnValueChanged += new Action<OptionBehaviour>(delegate
                    {
                        CustomRpcManager.Instance<RpcSyncSettings>().Send((TranslationController.Instance.GetString(StringNames.LobbyChangeSettingNotification).Replace("{0}", option.ConfigName.GetString()).Replace("{1}", option.GetValue()), false, true), PlayerControl.LocalPlayer.NetId);
                    });
                    menu.Children.Add(op);
                    num -= 0.45f;
                }
            }
            menu.scrollBar.SetYBoundsMax(-num - 1.65f);
        }
    }
}
