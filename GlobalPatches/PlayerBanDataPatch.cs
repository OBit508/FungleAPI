using AmongUs.Data.Player;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GlobalPatches
{
    [HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.BanPoints), MethodType.Setter)]
    internal static class PlayerBanDataPatch
    {
        public static bool Prefix(PlayerBanData __instance, ref float value)
        {
            value = 0f;
            return false;
        }
    }
}
