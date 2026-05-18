using AmongUs.GameOptions;
using FungleAPI.GModes.Logics;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes.Patches
{
    [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.InitComponents))]
    internal static class NormalGameManagerPatch
    {
        public static bool Prefix(NormalGameManager __instance)
        {
            __instance.LogicFlow = __instance.AddGameLogic<LogicGameFlowNormal>(new LGameFlow(__instance));
            __instance.LogicMinigame = __instance.AddGameLogic<LogicMinigame>(new LMinigame(__instance));
            __instance.LogicRoleSelection = __instance.AddGameLogic<LogicRoleSelectionNormal>(new LRoleSelection(__instance));
            __instance.LogicUsables = __instance.AddGameLogic<LogicUsablesBasic>(new LUsasbles(__instance));
            __instance.LogicOptions = __instance.AddGameLogic<LogicOptionsNormal>(new LOptions(__instance));
            return false;
        }
    }
}
