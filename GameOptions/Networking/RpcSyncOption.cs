using FungleAPI.Base.Rpc;
using FungleAPI.GameOptions.Options;
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
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncOption : AdvancedRpc<(SyncOptionType, IModdedOption, object)>
    {
        public override bool CanAcceptRPCsWithoutInnerNetObject => true;
        public override void Write(MessageWriter messageWriter, (SyncOptionType, IModdedOption, object) data)
        {
            messageWriter.WriteOption(data.Item2);
            data.Item2.Serialize(messageWriter);
            messageWriter.WritePacked((int)data.Item1);
            string str = "<color=#ffffff>";

            switch (data.Item1)
            {
                case SyncOptionType.Role:
                    if (data.Item3 is ICustomRole customRole)
                    {
                        str = $"{customRole.RoleColor.ToTextColor()}({customRole.RoleName.GetString()}) ";
                        messageWriter.WriteRole(customRole as RoleBehaviour);
                    }
                    break;
                case SyncOptionType.Team:
                    if (data.Item3 is ModdedTeam moddedTeam)
                    {
                        str = $"{moddedTeam.TeamColor.ToTextColor()}({moddedTeam.TeamName.GetString()}) ";
                        messageWriter.WriteTeam(moddedTeam);
                    }
                    break;
                case SyncOptionType.Game:
                    if (data.Item3 is ModPlugin modPlugin)
                    {
                        str += $"({modPlugin.ModName}) ";
                        messageWriter.WritePlugin(modPlugin);
                    }
                    break;
            }

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{str}{data.Item2.Data.Title.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{data.Item2.GetStringValue(true)}</font>", false);
        }
        public override void Handle(MessageReader messageReader)
        {
            IModdedOption moddedOption = messageReader.ReadOption();
            moddedOption.Deserialize(messageReader);
            SyncOptionType syncOptionType = (SyncOptionType)messageReader.ReadPackedInt32();

            string str = "<color=#ffffff>";

            switch (syncOptionType)
            {
                case SyncOptionType.Role:
                    ICustomRole customRole = messageReader.ReadRole().CustomRole();
                    str = $"{customRole.RoleColor.ToTextColor()}({customRole.RoleName.GetString()}) ";
                    break;
                case SyncOptionType.Team:
                    ModdedTeam moddedTeam = messageReader.ReadTeam();
                    str = $"{moddedTeam.TeamColor.ToTextColor()}({moddedTeam.TeamName.GetString()}) ";
                    break;
                case SyncOptionType.Game:
                    ModPlugin modPlugin = messageReader.ReadPlugin();
                    str += $"({modPlugin.ModName}) ";
                    break;
            }

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{str}{moddedOption.Data.Title.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{moddedOption.GetStringValue(false)}</font>", true);
        }
    }
}
