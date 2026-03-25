using AmongUs.GameOptions;
using FungleAPI.Base.Rpc;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Configuration.Networking
{
    /// <summary>
    /// Rpc that sync a role count and chance
    /// </summary>
    public class RpcSyncRoleCountAndChance : AdvancedRpc<RoleBehaviour>
    {
        public override void Handle(MessageReader reader)
        {
            RoleBehaviour role = reader.ReadRole();
            if (role.CustomRole() != null)
            {
                RoleOptions countAndChance = role.CustomRole().RoleOptions;
                countAndChance.SetCount(reader.ReadInt32());
                countAndChance.SetChance(reader.ReadInt32());
            }
            IRoleOptionsCollection roleOptionsCollection = GameOptionsManager.Instance.currentGameOptions.RoleOptions;
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, StringNames.LobbyChangeSettingNotificationRole.GetString().Replace("{0}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + role.TeamColor.ToTextColor() + role.NiceName + "</color></font>").Replace("{1}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + roleOptionsCollection.GetNumPerGame(role.Role).ToString() + "</font>").Replace("{2}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + roleOptionsCollection.GetChancePerGame(role.Role).ToString() + "</font>"), false);
        }
        public override void Write(MessageWriter writer, RoleBehaviour value)
        {
            writer.WriteRole(value);
            if (value.CustomRole() != null)
            {
                writer.Write(value.CustomRole().RoleOptions.GetCount());
                writer.Write(value.CustomRole().RoleOptions.GetChance());
            }
            IRoleOptionsCollection roleOptionsCollection = GameOptionsManager.Instance.currentGameOptions.RoleOptions;
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, StringNames.LobbyChangeSettingNotificationRole.GetString().Replace("{0}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + value.TeamColor.ToTextColor() + value.NiceName + "</color></font>").Replace("{1}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + roleOptionsCollection.GetNumPerGame(value.Role).ToString() + "</font>").Replace("{2}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + roleOptionsCollection.GetChancePerGame(value.Role).ToString() + "</font>"), false);
        }
    }
}
