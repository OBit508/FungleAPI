using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.InnerNet.GameDataMessages;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(SpawnGameDataMessage), "SerializeValues")]
    internal static class SpawnGameDataMessagePatch
    {
        public static void Postfix(SpawnGameDataMessage __instance, MessageWriter msg)
        {
            if (AmongUsClient.Instance.ClientId != __instance.ownerId && __instance.NetObjectType == Il2CppType.From(typeof(PlayerControl)))
            {
                FungleAPIPlugin.Instance.Log.LogInfo("Deserializing mods - Client");
                HandShakeManager.SendMods(msg);
            }
        }
    }
}
