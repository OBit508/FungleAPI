using AmongUs.Data.Player;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(PlayerBanData), "BanPoints", MethodType.Setter)]
    internal static class PlayerBanDataPatch
    {
        public static bool Prefix(PlayerBanData __instance)
        {
            if (AmongUsClient.Instance && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame)
            {
                float ban = 0;
                __instance.SetValue<float>(ref ban, Mathf.Max(0f, 0f), new Action(__instance.OnBanPointsChanged.Invoke));
                __instance.banPoints = ban;
                return false;
            }
            return true;
        }
    }
}
