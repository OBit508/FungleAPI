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
        public static Dictionary<string, BepInMod> RequiredMods = new Dictionary<string, BepInMod>();
        public static void GetMods((string, string, string)[] mods, out List<BepInMod> sameMods, out Dictionary<string, KeyValuePair<string, string>> missingMods, out List<KeyValuePair<string, string>> extraMods)
        {
            sameMods = new List<BepInMod>();
            missingMods = new Dictionary<string, KeyValuePair<string, string>>();
            extraMods = new List<KeyValuePair<string, string>>();

            foreach (BepInMod bepInMod in RequiredMods.Values)
            {
                missingMods.Add(bepInMod.GUID, new KeyValuePair<string, string>(bepInMod.Version, bepInMod.Name));
            }
            
            foreach ((string, string, string) mod in mods)
            {
                if (RequiredMods.TryGetValue(mod.Item1, out BepInMod bepInMod))
                {
                    sameMods.Add(bepInMod);
                    missingMods.Remove(mod.Item1);
                    continue;
                }
                extraMods.Add(new KeyValuePair<string, string>(mod.Item3, mod.Item2));
            }
        }
        public static void KickWithReason(int targetClientId, string reason)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            Rpc<RpcKickWithReason>.Instance.Send(reason, SendOption.Reliable, targetClientId);
        }
        public static void DisconnectWithReason(string reason)
        {
            AmongUsClient.Instance.ExitGame(DisconnectReasons.Custom);
            AmongUsClient.Instance.LastCustomDisconnect = reason;
        }
        public static System.Collections.IEnumerator CoKick(ClientData clientData, Func<string, string> message)
        {
            if (!AmongUsClient.Instance.AmHost) yield break;

            DateTime startTime = DateTime.UtcNow;
            string playerName = null;

            while (playerName == null)
            {
                if (DateTime.UtcNow - startTime > TimeSpan.FromSeconds(2))
                {
                    playerName = "(unknown)";
                    break;
                }

                NetworkedPlayerInfo networkedPlayerInfo = GameData.Instance.GetPlayerByClient(clientData);

                if (networkedPlayerInfo != null)
                {
                    playerName = networkedPlayerInfo.PlayerName;
                }

                yield return null;
            }

            AmongUsClient.Instance?.KickPlayer(clientData.Id, false);
            HudManager.Instance?.Notifier.AddDisconnectMessage(message(playerName));
        }
    }
}
