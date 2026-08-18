using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Start))]
    internal static class CreateGameOptionsPatch
    {
        public static void Postfix(CreateGameOptions __instance)
        {
            __instance.SetModeButton(0);
            __instance.modeButtons[1].gameObject.SetActive(false);
        }
    }
}
