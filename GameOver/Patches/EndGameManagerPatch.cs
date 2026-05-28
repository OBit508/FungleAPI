using AmongUs.Data;
using Assets.CoreScripts;
using FungleAPI.Utilities.Sound;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using FungleAPI.Extensions;

namespace FungleAPI.GameOver.Patches
{
    [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
    internal static class EndGameManagerPatch
    {
        public static bool Prefix(EndGameManager __instance)
        {
            BaseGameOver.CachedGameOver.OnSetEverythingUp(__instance);
            return false;
        }
    }
}
