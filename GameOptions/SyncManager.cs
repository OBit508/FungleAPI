using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.GameOptions.Networking;
using FungleAPI.Modifiers;
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

            FunglePlugin<FungleApiPlugin>.Logger.LogWarning("Syncing everything");

            Rpc<RpcSyncGRnT>.Instance.Send(PlayerControl.LocalPlayer, SendOption.Reliable, targetId);
            List<IModdedOption> moddedOptions = OptionManager.AllOptions.Values.ToList();
            while (moddedOptions.Count > 0)
            {
                List<IModdedOption> batch = moddedOptions.Take(45).ToList();
                moddedOptions.RemoveRange(0, batch.Count);

                Rpc<RpcSyncOptions>.Instance.Send(batch, PlayerControl.LocalPlayer, SendOption.Reliable, targetId);
            }
        }
        public static void RpcSyncTeam(ModdedTeam moddedTeam)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncTeam>.Instance.SendLate(moddedTeam, PlayerControl.LocalPlayer);
        }
        public static void RpcSyncModifier(BaseModifier baseModifier)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncModifier>.Instance.SendLate(baseModifier, PlayerControl.LocalPlayer);
        }
        public static void RpcSyncRole(ICustomRole customRole)
        {
            if (!AmongUsClient.Instance.AmHost || customRole == null)
            {
                return;
            }
            Rpc<RpcSyncRole>.Instance.SendLate(customRole, PlayerControl.LocalPlayer);
        }
        public static void RpcSyncGamemode()
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncGamemode>.Instance.SendLate(PlayerControl.LocalPlayer);
        }
        public static void RpcUpdatePreset(RulesPresets rulesPresets, ModPlugin modPlugin)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcUpdatePreset>.Instance.SendLate((rulesPresets, modPlugin), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncOption(IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.None, moddedOption, null), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncRoleOption(ICustomRole customRole, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.Role, moddedOption, customRole), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncTeamOption(ModdedTeam moddedTeam, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.Team, moddedOption, moddedTeam), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncModifierOption(BaseModifier baseModifier, IModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.SendLate((SyncOptionType.Modifier, moddedOption, baseModifier), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncGameOption(IModdedOption moddedOption)
        {
            RpcSyncOption(moddedOption);
        }
    }
}
