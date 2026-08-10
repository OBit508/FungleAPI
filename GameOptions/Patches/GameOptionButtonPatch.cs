using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(GameOptionButton), nameof(GameOptionButton.SetInteractable))]
    internal static class GameOptionButtonPatch
    {
        public static bool Prefix(GameOptionButton __instance, bool interactable)
        {
            if (__instance.buttonSprite != null)
            {
                __instance.buttonSprite.color = interactable ? __instance.interactableColor : __instance.uninteractableColor;
            }
            __instance.isInteractable = interactable;
            return false;
        }
    }
}
