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
            Rpc<RpcSyncEverything>.Instance.Send(PlayerControl.LocalPlayer.Data, SendOption.Reliable, targetId);
        }
        public static void RpcSyncTeam(ModdedTeam moddedTeam)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncTeam>.Instance.SendLate(moddedTeam, PlayerControl.LocalPlayer.Data);
        }
        public static void RpcSyncRole(ICustomRole customRole)
        {
            if (!AmongUsClient.Instance.AmHost || customRole == null)
            {
                return;
            }
            Rpc<RpcSyncRole>.Instance.SendLate(customRole, PlayerControl.LocalPlayer.Data);
        }
        public static void RpcSyncGamemode()
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncGamemode>.Instance.SendLate(PlayerControl.LocalPlayer.Data);
        }
        public static void RpcUpdatePreset(RulesPresets rulesPresets, ModPlugin modPlugin)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcUpdatePreset>.Instance.SendLate((rulesPresets, modPlugin), PlayerControl.LocalPlayer.Data);
        }
        public static void RpcSyncOption(IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.None, moddedOption, null), PlayerControl.LocalPlayer.Data);
        }
        public static void RpcSyncRoleOption(ICustomRole customRole, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.Role, moddedOption, customRole), PlayerControl.LocalPlayer.Data);
        }
        public static void RpcSyncTeamOption(ModdedTeam moddedTeam, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.Team, moddedOption, moddedTeam), PlayerControl.LocalPlayer.Data);
        }
        public static void RpcSyncGameOption(IModdedOption moddedOption)
        {
            RpcSyncOption(moddedOption);
        }
    }
}
