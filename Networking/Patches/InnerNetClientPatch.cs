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
            if (reason == DisconnectReasons.Kicked && (!string.IsNullOrEmpty(HandShakeManager.MissingMods) || !string.IsNullOrEmpty(HandShakeManager.ExtraMods)))
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(HandShakeManager.MissingMods))
                {
                    stringBuilder.Append(string.Format(FungleTranslation.HandShakeFail_MissingMods.GetString(), HandShakeManager.MissingMods));

                    if (!string.IsNullOrEmpty(HandShakeManager.ExtraMods))
                    {
                        stringBuilder.AppendLine();
                    }
                }

                if (!string.IsNullOrEmpty(HandShakeManager.ExtraMods))
                {
                    stringBuilder.Append(string.Format(FungleTranslation.HandShakeFail_ExtraMods.GetString(), HandShakeManager.ExtraMods));
                }

                reason = DisconnectReasons.Custom;
                __instance.LastCustomDisconnect = stringBuilder.ToString();

                HandShakeManager.MissingMods = null;
                HandShakeManager.ExtraMods = null;
            }
        }
    }
}