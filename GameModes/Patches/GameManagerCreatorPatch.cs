using FungleAPI.Api;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xCloud;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(GameManagerCreator), nameof(GameManagerCreator.Awake))]
    internal static class GameManagerCreatorPatch
    {
        public static void Postfix(GameManagerCreator __instance)
        {
            HideNSeekMode hideNSeekMode = GameMode<HideNSeekMode>.Instance;

            foreach (RulesCategory rulesCategory in __instance.HideAndSeekManagerPrefab.gameSettingsList.AllCategories)
            {
                hideNSeekMode.ModeOptions.Groups.Add(new HideNSeekMode.HNSGroup(rulesCategory, hideNSeekMode));
            }

            hideNSeekMode.ModeOptions.OptionCollection = new OptionCollection("GameModes", typeof(HideNSeekMode));
            hideNSeekMode.ModeOptions.OptionCollection.Initialize(FungleApiPlugin.Plugin, hideNSeekMode.Settings.Values.ToList());

        }
    }
}
