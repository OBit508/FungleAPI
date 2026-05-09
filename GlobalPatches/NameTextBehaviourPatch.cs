using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(NameTextBehaviour), "Start")]
    internal static class NameTextBehaviourPatch
    {
        public static bool Prefix(NameTextBehaviour __instance)
        {
            return __instance.name != "ModdedEditNameTab";
        }
    }
}
