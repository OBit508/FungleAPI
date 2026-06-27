using FungleAPI.Api;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal))]
    internal static class InnerNetClientPatch
    {
        public static void Prefix(InnerNetClient __instance, ref DisconnectReasons reason)
        {
            if (reason == DisconnectReasons.Kicked && HandShakeManager.DisconnectData != null)
            {
                reason = DisconnectReasons.Custom;
                StringBuilder stringBuilder = new StringBuilder();

                ModsDisconnectData modsDisconnectData = HandShakeManager.DisconnectData.Value;

                if (modsDisconnectData.MissingMods != null)
                {
                    stringBuilder.Append(string.Format(FungleTranslation.HandShakeFail_MissingMods.GetString(), modsDisconnectData.MissingMods));
                    if (modsDisconnectData.ExtraMods != null)
                    {
                        stringBuilder.AppendLine();
                    }
                }
                if (modsDisconnectData.ExtraMods != null)
                {
                    stringBuilder.Append(string.Format(FungleTranslation.HandShakeFail_ExtraMods.GetString(), modsDisconnectData.ExtraMods));
                }

                __instance.LastCustomDisconnect = stringBuilder.ToString();

                HandShakeManager.DisconnectData = null;
            }
        }
    }
}
