using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(HostLocalGameButton), nameof(HostLocalGameButton.Start))]
    internal static class HostLocalGameButtonPatch
    {
        public static void Postfix(HostLocalGameButton __instance)
        {
            __instance.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
