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

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class LobbyWarningText : MonoBehaviour
    {
        public static Dictionary<ClientData, ChangeableValue<float>> nonModdedPlayers = new Dictionary<ClientData, ChangeableValue<float>>();
        public TextMeshPro Text;
        public void Update()
        {
            string clientText = "<size=2>Non Modded Clients:</size>";
            foreach (KeyValuePair<ClientData, ChangeableValue<float>> client in nonModdedPlayers)
            {
                if (!AmongUsClient.Instance.allClients.Contains(client.Key))
                {
                    nonModdedPlayers.Remove(client.Key);
                    return;
                }
                clientText += "\n" + client.Key.PlayerName + " - Kicking in: " + client.Value.Value.ToString();
                client.Value.Value -= Time.deltaTime;
                if (client.Value.Value <= 0)
                {
                    AmongUsClient.Instance.KickPlayer(client.Key.Id, false);
                    nonModdedPlayers.Remove(client.Key);
                }
            }
            Text.text = clientText;
            Text.enabled = nonModdedPlayers.Count > 0;
        }
    }
}
