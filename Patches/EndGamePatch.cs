using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.Roles;
using HarmonyLib;
using Rewired.Utils.Classes.Data;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), "OnGameEnd")]
    internal class EndGamePatch
    {
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            Winners.Clear();
        }
        public static Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> Winners = new Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo>();
    }
}
