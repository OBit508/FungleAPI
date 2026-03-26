using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Networking;
using FungleAPI.Configuration.Presets;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
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

namespace FungleAPI.Configuration
{
    public static class SyncManager
    {
        public static void RpcSyncEverything(int targetId)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncEverything>.Instance.Send(PlayerControl.LocalPlayer, SendOption.Reliable, targetId);
        }
        public static void RpcSyncGameOpt(ModPlugin modPlugin)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncGameOpt>.Instance.Send(modPlugin.AsArray(), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncTeam(ModdedTeam moddedTeam)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncTeam>.Instance.Send(moddedTeam.AsArray(), PlayerControl.LocalPlayer);
        }
        public static void RpcSyncRole(RoleBehaviour roleBehaviour)
        {
            ICustomRole customRole = roleBehaviour.CustomRole();
            if (!AmongUsClient.Instance.AmHost || customRole == null)
            {
                return;
            }
            Rpc<RpcSyncRole>.Instance.Send(customRole.AsArray(), PlayerControl.LocalPlayer);
        }
        public static void RpcUpdatePreset(PresetV2 preset)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcUpdatePreset>.Instance.Send(preset, PlayerControl.LocalPlayer);
        }
        public static void RpcSyncOption(ModdedOption moddedOption)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
            Rpc<RpcSyncOption>.Instance.Send(moddedOption, PlayerControl.LocalPlayer);
        }
        public static void AddSettingsChangeMessage(this NotificationPopper notificationPopper, StringNames key, int roleCount, int roleChance, Color color, bool playSound = true, bool usePriority = false)
        {
            string item = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{color.ToTextColor()}{key.GetString()}</color></font>:" +
                $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{roleCount}</font>, " +
                $"{(usePriority ? FungleTranslation.PriorityText.GetString() : FungleTranslation.ChanceText.GetString())}: <font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{roleChance}{(usePriority ? "" : "%")}</font>";
            notificationPopper.SettingsChangeMessageLogic(key, item, playSound);
        }
    }
}
