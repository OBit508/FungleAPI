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

        public static void GetMods((string GUID, string version, string name)[] mods, out Dictionary<string, KeyValuePair<string, string>> missingMods, out List<KeyValuePair<string, string>> extraMods)
        {
            missingMods = new Dictionary<string, KeyValuePair<string, string>>();
            extraMods = new List<KeyValuePair<string, string>>();

            foreach (BepInMod bepInMod in RequiredMods.Values)
            {
                missingMods.Add(bepInMod.GUID, new KeyValuePair<string, string>(bepInMod.Name, bepInMod.Version));
            }
            
            foreach ((string GUID, string version, string name) mod in mods)
            {
                if (RequiredMods.TryGetValue(mod.GUID, out BepInMod bepInMod) && bepInMod.Version == mod.version)
                {
                    missingMods.Remove(mod.GUID);
                    continue;
                }
                extraMods.Add(new KeyValuePair<string, string>(mod.name, mod.version));
            }
        }
        public static void DisconnectWithReason(string reason)
        {
            AmongUsClient.Instance.ExitGame(DisconnectReasons.Custom);
            AmongUsClient.Instance.LastCustomDisconnect = reason;
        }
    }
}
