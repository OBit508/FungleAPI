using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Core.Logging.Interpolation;
using FungleAPI.Patches;
using HarmonyLib;
using InnerNet;
using static FungleAPI.PluginLoading.ModPlugin;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._CoHandleSpawn_d__166), "MoveNext")]
    internal static class IInnerNetClient_CoHandleSpawn_d__166Patch
    {
        public static void Postfix(InnerNetClient._CoHandleSpawn_d__166 __instance, bool __result)
        {
            if (!__result && !AmongUsClient.Instance.AmHost && __instance._ownerId_5__2 == AmongUsClient.Instance.ClientId)
            {
                Hazel.MessageReader reader = __instance.reader;
                if (reader.BytesRemaining > 0)
                {
                    HandShakeManager.ReadModsAndCheck(reader, __instance._ownerId_5__2);
                    return;
                }
                AmongUsClient.Instance.HandleDisconnect(HandShakeManager.HostIsNotModded);
            }
        }
    }
}
