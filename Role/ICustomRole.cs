using AmongUs.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Il2CppSystem.Net.Http.Headers.Parser;
using static Rewired.Controller;

namespace FungleAPI.Role
{
    /// <summary>
    /// The interface used to create the component of a custom role
    /// </summary>
    public interface ICustomRole
    {
        /// <summary>
        /// Role team
        /// </summary>
        ModdedTeam Team { get; }
        /// <summary>
        /// Role name
        /// </summary>
        StringNames RoleName { get; }
        /// <summary>
        /// Role blur
        /// </summary>
        StringNames RoleBlur { get; }
        /// <summary>
        /// Role blur med
        /// </summary>
        StringNames RoleBlurMed { get; }
        /// <summary>
        /// Role blur long
        /// </summary>
        StringNames RoleBlurLong { get; }
        /// <summary>
        /// Role color
        /// </summary>
        Color RoleColor { get; }
        /// <summary>
        /// Role configuration
        /// </summary>
        RoleConfiguration Configuration { get; }
        /// <summary>
        /// Role options
        /// </summary>
        RoleOptionCollection RoleOptions { get { return Save[Role]; } }
        /// <summary>
        /// Create the Sabotage button config for the role
        /// </summary>
        SabotageButtonConfig CreateSabotageConfig() => null;
        /// <summary>
        /// Create the Report button config for the role
        /// </summary>
        ReportButtonConfig CreateReportConfig() => null;
        /// <summary>
        /// Create the Vent button config for the role
        /// </summary>
        VentButtonConfig CreateVentConfig() => null;
        /// <summary>
        /// Create the Kill button config for the role
        /// </summary>
        KillButtonConfig CreateKillConfig() => null;
        /// <summary>
        /// Create the Light source config for the role
        /// </summary>
        LightSourceConfig CreateLightConfig() => null;
        /// <summary>
        /// Create the Mira role tab config for the role
        /// </summary>
        PlayerTabConfig CreateRoleTabConfig() => null;
        /// <summary>
        /// Called when the role dies
        /// </summary>
        void AssignRoleOnDeath(PlayerControl plr) => plr.RpcSetRole(Configuration.GhostRole);
        /// <summary>
        /// Role type
        /// </summary>
        public RoleTypes Role => (this as RoleBehaviour).Role;
        /// <summary>
        /// Role exile text
        /// </summary>
        string ExileText(ExileController exileController)
        {
            NetworkedPlayerInfo networkedPlayerInfo = exileController.initData.networkedPlayer;
            return string.Format(FungleTranslation.ExileText.GetString(), networkedPlayerInfo.PlayerName, networkedPlayerInfo.Role.NiceName);
        }
        internal static Dictionary<RoleTypes, RoleOptionCollection> Save = new();
    }
}
