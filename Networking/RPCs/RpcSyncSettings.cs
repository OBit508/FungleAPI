using AmongUs.GameOptions;
using Epic.OnlineServices;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSyncSettings : CustomRpc<(SyncTextType type, ModdedOption option, RoleBehaviour role, ModdedTeam team)>
    {
        public static void Notify(string text1, string text2, bool playSound)
        {
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, StringNames.LobbyChangeSettingNotification.GetString().Replace("{0}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + text1 + "</font>").Replace("{1}", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + text2 + "</font>"), playSound);
        }

        public override void Write(MessageWriter writer, (SyncTextType type, ModdedOption option, RoleBehaviour role, ModdedTeam team) value)
        {
            ModdedOption[] options = ConfigurationManager.Configs.Values.ToArray();
            writer.Write(options.Count());
            for (int i = 0; i < options.Count(); i++)
            {
                writer.WriteConfig(options[i]);
                writer.Write(options[i].GetValue());
            }
            writer.Write(ConfigurationManager.TeamCountAndPriorities.Count);
            for (int i = 0; i < ConfigurationManager.TeamCountAndPriorities.Count; i++)
            {
                writer.WriteCountAndPriority(ConfigurationManager.TeamCountAndPriorities[i]);
                writer.Write(ConfigurationManager.TeamCountAndPriorities[i].GetCount());
                writer.Write(ConfigurationManager.TeamCountAndPriorities[i].GetPriority());
            }
            writer.Write((int)value.type);
            if (value.type == SyncTextType.Option || value.type == SyncTextType.RoleOption)
            {
                writer.WriteConfig(value.option);
                if (value.type == SyncTextType.RoleOption)
                {
                    writer.WriteRole(value.role);
                    Notify(value.role.TeamColor.ToTextColor() + "(" + value.role.NiceName + ") " + value.option.ConfigName.GetString() + "</color>", value.option.GetValue(), false);
                    return;
                }
                Notify(value.option.ConfigName.GetString(), value.option.GetValue(), false);
            }
            else if (value.type == SyncTextType.TeamOption || value.type == SyncTextType.TeamCount || value.type == SyncTextType.TeamPriority)
            {
                StringNames name;
                string v;
                writer.WriteCountAndPriority(value.team.CountAndPriority);
                if (value.type == SyncTextType.TeamOption)
                {
                    writer.WriteConfig(value.option);
                    name = value.option.ConfigName.StringName;
                    v = value.option.GetValue();
                }
                else if (value.type == SyncTextType.TeamCount)
                {
                    name = ModdedTeam.CountText;
                    v = value.team.CountAndPriority.GetCount().ToString();
                }
                else
                {
                    name = ModdedTeam.PriorityText;
                    v = value.team.CountAndPriority.GetPriority().ToString();
                }
                Notify(value.team.TeamColor.ToTextColor() + "(" + value.team.TeamName.GetString() + ") " + name.GetString() + "</color>", v, false);
            }
        }
        public override void Handle(MessageReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ModdedOption option = reader.ReadConfig();
                option.SetValue(reader.ReadString());
            }
            int count2 = reader.ReadInt32();
            for (int i = 0; i < count2; i++)
            {
                TeamCountAndPriority c = reader.ReadCountAndPriority();
                c.SetCount(reader.ReadInt32());
                c.SetPriority(reader.ReadInt32());
            }
            SyncTextType type = (SyncTextType)reader.ReadInt32();
            if (type == SyncTextType.Option || type == SyncTextType.RoleOption)
            {
                ModdedOption option = reader.ReadConfig();
                if (type == SyncTextType.RoleOption)
                {
                    RoleBehaviour role = reader.ReadRole();
                    Notify(role.TeamColor.ToTextColor() + "(" + role.NiceName + ") " + option.ConfigName.GetString() + "</color>", option.GetValue(), true);
                    return;
                }
                Notify(option.ConfigName.GetString(), option.GetValue(), true);
            }
            else if (type == SyncTextType.TeamOption ||type == SyncTextType.TeamCount || type == SyncTextType.TeamPriority)
            {
                StringNames name;
                string v;
                ModdedTeam team = reader.ReadCountAndPriority().Team;
                if (type == SyncTextType.TeamOption)
                {
                    ModdedOption option = reader.ReadConfig();
                    name = option.ConfigName.StringName;
                    v = option.GetValue();
                }
                else if (type == SyncTextType.TeamCount)
                {
                    name = ModdedTeam.CountText;
                    v = team.CountAndPriority.GetCount().ToString();
                }
                else
                {
                    name = ModdedTeam.PriorityText;
                    v = team.CountAndPriority.GetPriority().ToString();
                }
                Notify(team.TeamColor.ToTextColor() + "(" + team.TeamName.GetString() + ") " + name.GetString() + "</color>", v, true);
            }
        }
    }
}
