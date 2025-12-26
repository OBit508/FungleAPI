using FungleAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using FungleAPI.Attributes;
using InnerNet;
using FungleAPI.Utilities;
using FungleAPI.Translation;
using FungleAPI.Networking.RPCs;
using FungleAPI.ModCompatibility;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class LobbyWarningText : MonoBehaviour
    {
        public static Dictionary<ClientData, (ChangeableValue<float>, ChangeableValue<float>)> nonModdedPlayers = new Dictionary<ClientData, (ChangeableValue<float>, ChangeableValue<float>)>();
        public TextMeshPro Text;
        public void Update()
        {
            string clientText = "<size=2>" + FungleTranslation.NonModdedText.GetString() + "</size>";
            foreach (KeyValuePair<ClientData, (ChangeableValue<float>, ChangeableValue<float>)> client in nonModdedPlayers)
            {
                bool mciClient = FungleAPIPlugin.MCIActive && MCIUtils.ContainsClient(client.Key.Id);
                if (!AmongUsClient.Instance.allClients.Contains(client.Key) || mciClient)
                {
                    if (mciClient)
                    {
                        FungleAPIPlugin.Instance.Log.LogInfo("MCI-created player found bypassing verification");
                    }
                    nonModdedPlayers.Remove(client.Key);
                    return;
                }
                clientText += "\n" + client.Key.PlayerName + " - " + FungleTranslation.KickingText.GetString() + " " + client.Value.Item1.Value.ToString();
                client.Value.Item1.Value -= Time.deltaTime;
                client.Value.Item2.Value -= Time.deltaTime;
                if (client.Value.Item1.Value <= 0)
                {
                    AmongUsClient.Instance.KickPlayer(client.Key.Id, false);
                    nonModdedPlayers.Remove(client.Key);
                }
                else if (client.Value.Item2.Value <= 0 && AmongUsClient.Instance.AmHost)
                {
                    CustomRpcManager.Instance<RpcRequestForAmModded>().Send(PlayerControl.LocalPlayer, Hazel.SendOption.Reliable, client.Key.Id);
                    client.Value.Item2.Value = 1.5f;
                }
            }
            Text.text = clientText;
            Text.enabled = nonModdedPlayers.Count > 0;
        }
    }
}
