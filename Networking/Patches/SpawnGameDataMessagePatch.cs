using AmongUs.InnerNet.GameDataMessages;
using FungleAPI.ModCompatibility;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(SpawnGameDataMessage), nameof(SpawnGameDataMessage.SerializeValues))]
    internal static class SpawnGameDataMessagePatch
    {
        public static void Postfix(SpawnGameDataMessage __instance, MessageWriter msg)
        {
            if (ReactorCompatibility.Instance != null) return;

            if (AmongUsClient.Instance.ClientId != __instance.ownerId && __instance.NetObjectType == Il2CppType.Of<PlayerControl>())
            {
                msg.Write("FClient");
            }
        }
    }
}
