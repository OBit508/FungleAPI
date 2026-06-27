using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
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
        /// Sabotage button config for the role
        /// </summary>
        SabotageButtonConfig SabotageConfig => SabotageButtonConfig.Default;
        /// <summary>
        /// Report button config for the role
        /// </summary>
        ReportButtonConfig ReportConfig => ReportButtonConfig.Default;
        /// <summary>
        /// Vent button config for the role
        /// </summary>
        VentButtonConfig VentConfig => VentButtonConfig.Default;
        /// <summary>
        /// Kill button config for the role
        /// </summary>
        KillButtonConfig KillConfig => KillButtonConfig.Default;
        /// <summary>
        /// Light source config for the role
        /// </summary>
        LightSourceConfig LightConfig => LightSourceConfig.Default;
        /// <summary>
        /// Player tab config for the role
        /// </summary>
        PlayerTabConfig PlayerTabConfig => PlayerTabConfig.Default;
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
