using AmongUs.GameOptions;
using FungleAPI.GameOptions.Networking;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.GameOptions
{
    public static class SyncManager
    {
        public const string MainFont = "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">";
        public static void RpcSyncEverything(int targetId)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncEverything>.Instance.Send(SendOption.Reliable, targetId);
        }
        public static void RpcSyncTeam(ModdedTeam moddedTeam)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncTeam>.Instance.Send(moddedTeam);
        }
        public static void RpcSyncRole(ICustomRole customRole)
        {
            if (!AmongUsClient.Instance.AmHost || customRole == null)
            {
                return;
            }
            Rpc<RpcSyncRole>.Instance.Send(customRole);
        }
        public static void RpcUpdatePreset(RulesPresets rulesPresets, ModPlugin modPlugin)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcUpdatePreset>.Instance.Send((rulesPresets, modPlugin));
        }
        public static void RpcSyncOption(IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.None, moddedOption, null));
        }
        public static void RpcSyncRoleOption(ICustomRole customRole, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.Role, moddedOption, customRole));
        }
        public static void RpcSyncTeamOption(ModdedTeam moddedTeam, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.Team, moddedOption, moddedTeam));
        }
        public static void RpcSyncGameOption(ModPlugin modPlugin, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.Game, moddedOption, modPlugin));
        }
    }
}
