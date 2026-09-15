using AmongUs.Data;
using FungleAPI.Player.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    public static class HandShakeManager
    {
        internal static string MissingMods = null;
        internal static string ExtraMods = null;
        public static Dictionary<string, BepInMod> RequiredMods = new Dictionary<string, BepInMod>();

        public static bool? ModdedServerHandshakeActive;

        public static void GetMods(BepInMod[] mods, out List<BepInMod> missingMods, out List<BepInMod> missingOnClientMods)
        {
            Dictionary<string, BepInMod> msMods = new Dictionary<string, BepInMod>();

            missingOnClientMods = new List<BepInMod>();

            foreach (BepInMod bepInMod in RequiredMods.Values)
            {
                msMods.Add(bepInMod.GUID, bepInMod);
            }
            
            foreach (BepInMod mod in mods)
            {
                if (RequiredMods.TryGetValue(mod.GUID, out BepInMod bepInMod) && bepInMod.Version == mod.Version)
                {
                    msMods.Remove(mod.GUID);
                    continue;
                }
                missingOnClientMods.Add(mod);
            }

            missingMods = msMods.Values.ToList();
        }
        public static void DisconnectWithReason(string reason)
        {
            AmongUsClient.Instance.ExitGame(DisconnectReasons.Custom);
            AmongUsClient.Instance.LastCustomDisconnect = reason;
        }
    }
}
