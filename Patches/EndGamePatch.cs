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
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            if (Winners.Count > 0)
            {
                EndGameResult.CachedWinners = Winners;
                Winners.Clear();
            }
            else
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.Role.GetTeam().WinReason == endGameResult.GameOverReason)
                    {
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
                        FungleAPIPlugin.Instance.Log.LogError(player.name + " Winned");
                    }
                }
            }
        }
        public static Il2CppSystem.Collections.Generic.List<CachedPlayerData> Winners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
    }
}
