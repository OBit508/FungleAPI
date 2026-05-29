using BepInEx.Core.Logging.Interpolation;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._CoHandleSpawn_d__166), "MoveNext")]
    internal static class CoHandleSpawnPatch
    {
        public static void Postfix(InnerNetClient._CoHandleSpawn_d__166 __instance, bool __result)
        {
            if (ReactorCompatibility.Instance != null || __result) return;

            if (!AmongUsClient.Instance.AmHost && __instance._ownerId_5__2 == AmongUsClient.Instance.ClientId)
            {
                MessageReader reader = __instance.reader;
                if (reader.BytesRemaining > 0)
                {
                    try
                    {
                        string str = reader.ReadString();
                        if (str != "FClient")
                        {
                            HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail.HostNotModded.GetString());
                        }
                    }
                    catch { HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail.HostNotModded.GetString()); }
                    return;
                }
                HandShakeManager.DisconnectWithReason(FungleTranslation.HandShakeFail.HostNotModded.GetString());
            }
        }
    }
}
