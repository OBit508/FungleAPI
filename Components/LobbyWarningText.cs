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

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class LobbyWarningText : MonoBehaviour
    {
        public static Dictionary<ClientData, (ChangeableValue<float>, ChangeableValue<float>)> nonModdedPlayers = new Dictionary<ClientData, (ChangeableValue<float>, ChangeableValue<float>)>();
        public TextMeshPro Text;
        private static Translator nonModdedText;
        private static Translator kickingText;
        public static StringNames NonModdedText
        {
            get
            {
                if (nonModdedText == null)
                {
                    nonModdedText = new Translator("Non Modded Clients:");
                    nonModdedText.AddTranslation(SupportedLangs.Latam, "Clientes sin mods:");
                    nonModdedText.AddTranslation(SupportedLangs.Brazilian, "Clientes não modificados:");
                    nonModdedText.AddTranslation(SupportedLangs.Portuguese, "Clientes não modificados:");
                    nonModdedText.AddTranslation(SupportedLangs.Korean, "모드가 없는 클라이언트:");
                    nonModdedText.AddTranslation(SupportedLangs.Russian, "Клиенты без модов:");
                    nonModdedText.AddTranslation(SupportedLangs.Dutch, "Niet-gemodde clients:");
                    nonModdedText.AddTranslation(SupportedLangs.Filipino, "Mga kliyenteng walang mods:");
                    nonModdedText.AddTranslation(SupportedLangs.French, "Clients sans mods :");
                    nonModdedText.AddTranslation(SupportedLangs.German, "Nicht-modifizierte Clients:");
                    nonModdedText.AddTranslation(SupportedLangs.Italian, "Client non moddati:");
                    nonModdedText.AddTranslation(SupportedLangs.Japanese, "非Modクライアント:");
                    nonModdedText.AddTranslation(SupportedLangs.Spanish, "Clientes sin mods:");
                    nonModdedText.AddTranslation(SupportedLangs.SChinese, "未安装模组的客户端:");
                    nonModdedText.AddTranslation(SupportedLangs.TChinese, "未安裝模組的客戶端:");
                    nonModdedText.AddTranslation(SupportedLangs.Irish, "Cliaint gan mods:");
                }
                return nonModdedText.StringName;
            }
        }
        public static StringNames KickingText
        {
            get
            {
                if (kickingText == null)
                {
                    kickingText = new Translator("Kicking in:");
                    kickingText.AddTranslation(SupportedLangs.Brazilian, "Expulsando em:");
                    kickingText.AddTranslation(SupportedLangs.Portuguese, "Expulsando em:");
                    kickingText.AddTranslation(SupportedLangs.Korean, "추방까지:");
                    kickingText.AddTranslation(SupportedLangs.Russian, "Исключение через:");
                    kickingText.AddTranslation(SupportedLangs.Dutch, "Uitzetten over:");
                    kickingText.AddTranslation(SupportedLangs.Filipino, "Pagtatanggal sa loob ng:");
                    kickingText.AddTranslation(SupportedLangs.French, "Expulsion dans :");
                    kickingText.AddTranslation(SupportedLangs.German, "Rauswurf in:");
                    kickingText.AddTranslation(SupportedLangs.Italian, "Espulsione tra:");
                    kickingText.AddTranslation(SupportedLangs.Japanese, "追放まで:");
                    kickingText.AddTranslation(SupportedLangs.Spanish, "Expulsando en:");
                    kickingText.AddTranslation(SupportedLangs.SChinese, "踢出倒计时:");
                    kickingText.AddTranslation(SupportedLangs.TChinese, "踢出倒數:");
                    kickingText.AddTranslation(SupportedLangs.Irish, "Ag ciceáil i:");

                }
                return kickingText.StringName;
            }
        }
        public void Update()
        {
            string clientText = "<size=2>" + NonModdedText.GetString() + "</size>";
            foreach (KeyValuePair<ClientData, (ChangeableValue<float>, ChangeableValue<float>)> client in nonModdedPlayers)
            {
                if (!AmongUsClient.Instance.allClients.Contains(client.Key))
                {
                    nonModdedPlayers.Remove(client.Key);
                    return;
                }
                clientText += "\n" + client.Key.PlayerName + " - " + KickingText.GetString() + " " + client.Value.Item1.Value.ToString();
                client.Value.Item1.Value -= Time.deltaTime;
                client.Value.Item2.Value -= Time.deltaTime;
                if (client.Value.Item1.Value <= 0)
                {
                    AmongUsClient.Instance.KickPlayer(client.Key.Id, false);
                    nonModdedPlayers.Remove(client.Key);
                }
                else if (client.Value.Item2.Value <= 0 && AmongUsClient.Instance.AmHost)
                {
                    CustomRpcManager.Instance<RpcRequestForAmModded>().Send(PlayerControl.LocalPlayer.NetId, Hazel.SendOption.Reliable, client.Key.Id);
                    client.Value.Item2.Value = 1.5f;
                }
            }
            Text.text = clientText;
            Text.enabled = nonModdedPlayers.Count > 0;
        }
    }
}
