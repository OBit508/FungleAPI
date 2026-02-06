using AmongUs.GameOptions;
using Epic.OnlineServices;
using FungleAPI.Base.Rpc;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
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
    /// <summary>
    /// 
    /// </summary>
    public class RpcSyncSettings : AdvancedRpc<(SyncTextType type, ModdedOption option, RoleBehaviour role, ModdedTeam team)>
    {
        private static void Notify(string text1, string text2, bool playSound)
        {
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(
                StringNames.None,
                StringNames.LobbyChangeSettingNotification.GetString()
                    .Replace("{0}", $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{text1}</font>")
                    .Replace("{1}", $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">{text2}</font>"),
                playSound
            );
        }
        private static string FormatRoleText(RoleBehaviour role, ModdedOption option)
        {
            return $"{role.TeamColor.ToTextColor()}({role.NiceName}) {option.ConfigName.GetString()}</color>";
        }
        private static string FormatTeamText(ModdedTeam team, StringNames name)
        {
            return $"{team.TeamColor.ToTextColor()}({team.TeamName.GetString()}) {name.GetString()}</color>";
        }
        private static (StringNames name, string value) GetTeamValue(SyncTextType type, ModdedTeam team, ModdedOption option)
        {
            return type switch
            {
                SyncTextType.TeamOption => (option.ConfigName.StringName, option.GetValue()),
                SyncTextType.TeamCount => (FungleTranslation.CountText, team.CountAndPriority.GetCount().ToString()),
                SyncTextType.TeamPriority => (FungleTranslation.PriorityText, team.CountAndPriority.GetPriority().ToString()),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
        public override void Write(MessageWriter writer, (SyncTextType type, ModdedOption option, RoleBehaviour role, ModdedTeam team) value)
        {
            writer.Write(ConfigurationManager.Options.Count);
            foreach (var opt in ConfigurationManager.Options)
            {
                writer.WriteOption(opt);
                writer.Write(opt.GetValue());
            }
            writer.Write(ConfigurationManager.TeamCountAndPriorities.Count);
            foreach (var t in ConfigurationManager.TeamCountAndPriorities)
            {
                writer.WriteCountAndPriority(t);
                writer.Write(t.GetCount());
                writer.Write(t.GetPriority());
            }
            writer.Write((int)value.type);
            switch (value.type)
            {
                case SyncTextType.Option:
                    writer.WriteOption(value.option);
                    Notify(value.option.ConfigName.GetString(), value.option.GetValue(), false);
                    break;

                case SyncTextType.RoleOption:
                    writer.WriteOption(value.option);
                    writer.WriteRole(value.role);
                    Notify(FormatRoleText(value.role, value.option), value.option.GetValue(), false);
                    break;
                case SyncTextType.TeamOption:
                case SyncTextType.TeamCount:
                case SyncTextType.TeamPriority:
                    writer.WriteCountAndPriority(value.team.CountAndPriority);
                    if (value.type == SyncTextType.TeamOption)
                    {
                        writer.WriteOption(value.option);
                    }
                    (StringNames name, string value) teamValue = GetTeamValue(value.type, value.team, value.option);
                    Notify(FormatTeamText(value.team, teamValue.name), teamValue.value, false);
                    break;
            }
        }
        public override void Handle(MessageReader reader)
        {
            for (int i = reader.ReadInt32(); i > 0; i--)
            {
                reader.ReadOption().SetValue(reader.ReadString());
            }
            for (int i = reader.ReadInt32(); i > 0; i--)
            {
                var c = reader.ReadCountAndPriority();
                c.SetCount(reader.ReadInt32());
                c.SetPriority(reader.ReadInt32());
            }
            SyncTextType type = (SyncTextType)reader.ReadInt32();
            switch (type)
            {
                case SyncTextType.Option:
                    {
                        ModdedOption opt = reader.ReadOption();
                        Notify(opt.ConfigName.GetString(), opt.GetValue(), true);
                        break;
                    }
                case SyncTextType.RoleOption:
                    {
                        ModdedOption opt = reader.ReadOption();
                        RoleBehaviour role = reader.ReadRole();
                        Notify(FormatRoleText(role, opt), opt.GetValue(), true);
                        break;
                    }
                case SyncTextType.TeamOption:
                case SyncTextType.TeamCount:
                case SyncTextType.TeamPriority:
                    {
                        ModdedTeam team = reader.ReadCountAndPriority().Team;
                        ModdedOption opt = type == SyncTextType.TeamOption ? reader.ReadOption() : null;
                        (StringNames name, string value) teamValue = GetTeamValue(type, team, opt);
                        Notify(FormatTeamText(team, teamValue.name), teamValue.value, true);
                        break;
                    }
            }
        }
    }
}
