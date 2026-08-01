using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.GameOptions;
using FungleAPI.GlobalPatches;
using FungleAPI.ModCompatibility.MiraSupport;
using FungleAPI.Networking;
using FungleAPI.Player.Networking;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using InnerNet;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FungleAPI.Components
{
    /// <summary>
    ///  A component designed to help the API work
    /// </summary>
    public class PlayerHelper : PlayerComponent
    {
        public static List<PlayerHelper> AllPlayers = new List<PlayerHelper>();
        /// <summary>
        /// Returns to the last role the player had when dead
        /// </summary>
        public RoleTypes LastDeadRole = RoleTypes.CrewmateGhost;
        internal Vent __CurrentVent;
        /// <summary>
        /// Returns the player's current vent
        /// </summary>
        public Vent CurrentVent => player.AmOwner ? Vent.currentVent : __CurrentVent;
        public TextMeshPro RoleText;
        public void Start()
        {
            StartCoroutine(CoInitialize().WrapToIl2Cpp());

            if (RoleText != null) return;

            TextMeshPro original = player.cosmetics.nameText;

            RoleText = GameObject.Instantiate(original, original.transform);
            RoleText.transform.localScale = Vector3.one * 0.6f;

            RoleText.transform.localPosition = new Vector3(0, -0.2f, 0);

            RoleText.gameObject.SetActive(false);

            AllPlayers.Add(this);
        }
        public void OnDestroy()
        {
            AllPlayers.Remove(this);
        }
        public void SetRoleText(RoleTypes roleTypes)
        {
            if (RoleText == null) Start();

            RoleBehaviour roleBehaviour = RoleManager.Instance.GetRole(roleTypes);

            if (roleBehaviour != null && roleBehaviour.ShowRoleText())
            {
                ModdedTeam localTeam = PlayerControl.LocalPlayer.Data.Role.GetTeam();
                ModdedTeam team = roleBehaviour.GetTeam();

                if (localTeam == team && localTeam.KnowMembers || player.AmOwner)
                {
                    RoleText.gameObject.SetActive(true);
                    RoleText.text = roleBehaviour.NiceName;
                    RoleText.color = roleBehaviour.TeamColor;
                    return;
                }
            }

            RoleText.gameObject.SetActive(false);
        }
        public System.Collections.IEnumerator CoInitialize()
        {
            if (!AmongUsClient.Instance.AmHost) yield break;

            while (player.Data == null || player.Data.ClientId < 0) yield return null;

            if (AmongUsClient.Instance.HostId == player.Data.ClientId) yield break;

            if (AmongUsClientPatch.WrongModdeds.TryGetValue(player.Data.ClientId, out KeyValuePair<string, string> mods))
            {
                AmongUsClientPatch.WrongModdeds.Remove(player.Data.ClientId);
                Rpc<RpcSendModsDisconnect>.Instance.Send(mods, PlayerControl.LocalPlayer);
                AmongUsClient.Instance.KickPlayer(player.Data.ClientId, false);
                HudManager.Instance?.Notifier.AddDisconnectMessage(FungleTranslation.HandShakeFail_ModdedPlayerDisconnect.GetString());
            }
            else
            {
                SyncManager.RpcSyncEverything(player.Data.ClientId);
            }
        }
        public void Update()
        {
            player.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, RoleText.gameObject.activeSelf ? -0.4f : -0.2f, 0);
        }

        [EventRegister]
        private static void SetRoleText(AfterSetRoleEvent afterSetRoleEvent)
        {
            if (afterSetRoleEvent.TargetPlayer == null) return;

            if (afterSetRoleEvent.TargetPlayer.AmOwner)
            {
                foreach (PlayerHelper playerHelper in AllPlayers)
                {
                    playerHelper.SetRoleText(playerHelper.player.Data.RoleType);
                }
                return;
            }
            afterSetRoleEvent.TargetPlayer.GetComponent<PlayerHelper>().SetRoleText(afterSetRoleEvent.RoleType);
        }
    }
}
