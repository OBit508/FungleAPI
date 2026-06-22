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
            Rpc<RpcSyncEverything>.Instance.Send(PlayerControl.LocalPlayer, SendOption.Reliable, targetId);
        }
        public static void RpcSyncTeam(ModdedTeam moddedTeam)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncTeam>.Instance.Send(moddedTeam, PlayerControl.LocalPlayer);
        }
        public static void RpcSyncRole(ICustomRole customRole)
        {
            if (!AmongUsClient.Instance.AmHost || customRole == null)
            {
                return;
            }
            Rpc<RpcSyncRole>.Instance.Send(customRole, PlayerControl.LocalPlayer);
        }
        public static void RpcSyncGamemode()
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncGamemode>.Instance.Send(PlayerControl.LocalPlayer);
        }
        public static void RpcUpdatePreset(RulesPresets rulesPresets, ModPlugin modPlugin)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcUpdatePreset>.Instance.Send((rulesPresets, modPlugin), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncOption(IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.None, moddedOption, null), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncRoleOption(ICustomRole customRole, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.Role, moddedOption, customRole), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncTeamOption(ModdedTeam moddedTeam, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send((SyncOptionType.Team, moddedOption, moddedTeam), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncGameOption(IModdedOption moddedOption)
        {
            RpcSyncOption(moddedOption);
        }
    }
}
