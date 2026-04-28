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
        public static void RpcSyncEverything(int targetId)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
        }
        public static void RpcSyncGameOpt(ModPlugin modPlugin)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
        }
        public static void RpcSyncTeam(ModdedTeam moddedTeam)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                return;
            }
        }
        public static void RpcSyncRole(RoleBehaviour roleBehaviour)
        {
            ICustomRole customRole = roleBehaviour.CustomRole();
            if (!AmongUsClient.Instance.AmHost || customRole == null)
            {
                return;
            }
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
