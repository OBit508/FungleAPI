using AmongUs.GameOptions;
using FungleAPI.Player.Networking;
using FungleAPI.Role;
using TMPro;
using UnityEngine;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using System.Collections.Generic;

namespace FungleAPI.Components
{
    /// <summary>
    ///  A component designed to help the API work
    /// </summary>
    public class PlayerHelper : PlayerComponent
    {
        public static List<PlayerHelper> AllPlayers = new List<PlayerHelper>();
        /// <summary>
        /// Returns to the last role the player had when alive
        /// </summary>
        public RoleTypes LastAliveRole = RoleTypes.Crewmate;
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
            TextMeshPro original = player.cosmetics.nameText;

            RoleText = GameObject.Instantiate(original, original.transform);
            RoleText.transform.localScale = Vector3.one * 0.6f;

            RoleText.transform.localPosition = new Vector3(0, -0.2f, 0);

            SetRoleText(player.Data.RoleType);

            AllPlayers.Add(this);
        }
        public void OnDestroy()
        {
            AllPlayers.Remove(this);
        }
        public void SetRoleText(RoleTypes roleTypes)
        {
            RoleBehaviour roleBehaviour = RoleManager.Instance.GetRole(roleTypes);

            if (roleBehaviour != null)
            {
                ModdedTeam localTeam = PlayerControl.LocalPlayer.Data.Role.GetTeam();
                ModdedTeam team = roleBehaviour.GetTeam();

                if (roleBehaviour.ShowRoleText() && (localTeam == team && localTeam.KnowMembers || player.AmOwner))
                {
                    RoleText.gameObject.SetActive(true);
                    RoleText.text = roleBehaviour.NiceName;
                    RoleText.color = roleBehaviour.TeamColor;
                    return;
                }
            }

            RoleText.gameObject.SetActive(false);
        }
        public void Update()
        {
            player.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, RoleText.gameObject.activeSelf ? -0.4f : -0.2f, 0);
            RoleConfigManager.LightConfig?.Update?.Invoke();
        }

        [EventRegister]
        private static void SetRoleText(AfterSetRoleEvent afterSetRoleEvent)
        {
            afterSetRoleEvent.TargetPlayer?.GetComponent<PlayerHelper>().SetRoleText(afterSetRoleEvent.RoleType);

            if (afterSetRoleEvent.TargetPlayer.AmOwner)
            {
                foreach (PlayerHelper playerHelper in AllPlayers)
                {
                    playerHelper.SetRoleText(playerHelper.player.Data.RoleType);
                }
            }
        }
    }
}
