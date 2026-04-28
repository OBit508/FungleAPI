using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(GamePresetsTab), nameof(GamePresetsTab.OnEnable))]
    internal static class GamePresetsTabPatch
    {
        public static bool Prefix(GamePresetsTab __instance)
        {
            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin) return true;

            RulesPresets rulesPresets = (RulesPresets)GameSettingMenuPatch.pluginChanger.CurrentPlugin.RulePreset.Value;

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
