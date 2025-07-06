using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using FungleAPI;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(VersionShower), "Start")]
    class VersionShowerPatch
    {
        public static void Postfix(VersionShower __instance)
        {
            __instance.text.text = "AmongUs " + __instance.text.text + " - FungleAPI " + FungleAPIPlugin.ModV;
        }
    }
}
