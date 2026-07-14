using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Extensions;
using FungleAPI.ModCompatibility.MiraSupport;
using FungleAPI.PluginLoading;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(GamePresetsTab), nameof(GamePresetsTab.OnEnable))]
    [HarmonyPriority(Priority.Last)]
    internal static class GamePresetsTabPatch
    {
        public static bool Prefix(GamePresetsTab __instance)
        {
            if (!GameSettingMenu.Instance) return true;

            if (GameManager.Instance.IsHideAndSeek() || GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleApiPlugin.Plugin.ModAssembly) return true;

            if (MiraCompatibility.Instance != null && MiraCompatibility.Instance.IsMiraAssembly(GameSettingMenuPatch.pluginChanger.CurrentPlugin)) return true;

            ModPlugin modPlugin = ModPluginManager.GetModPlugin(GameSettingMenuPatch.pluginChanger.CurrentPlugin);

            RulesPresets rulesPresets = (RulesPresets)modPlugin.RulePreset.Value;

            if (rulesPresets == RulesPresets.Standard)
            {
                __instance.SecondPresetButton.SelectButton(false);
                __instance.StandardPresetButton.SelectButton(true);
                __instance.StandardPresetButton.ReceiveMouseOut();
                __instance.SecondPresetButton.ReceiveMouseOut();
            }
            else if (rulesPresets != RulesPresets.Custom)
            {
                __instance.SecondPresetButton.SelectButton(true);
                __instance.StandardPresetButton.SelectButton(false);
                __instance.StandardPresetButton.ReceiveMouseOut();
                __instance.SecondPresetButton.ReceiveMouseOut();
            }
            else
            {
                __instance.SecondPresetButton.SelectButton(false);
                __instance.StandardPresetButton.SelectButton(false);
            }
            __instance.SetSelectedText();
            return false;
        }
    }
}
