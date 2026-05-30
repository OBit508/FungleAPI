using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
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

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncTeam : AdvancedRpc<ModdedTeam>
    {
        public override bool CanAcceptRPCsWithoutInnerNetObject => true;
        public override void Write(MessageWriter messageWriter, ModdedTeam data)
        {
            messageWriter.WriteTeam(data);
            messageWriter.WritePacked(data.TeamOptions.LocalTeamCount);
            messageWriter.WritePacked(data.TeamOptions.LocalTeamPriority);

            if (!RpcSyncEverything.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{data.TeamColor.ToTextColor()}{data.TeamName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{data.TeamOptions.LocalTeamCount}</font>, " +
                $"{FungleTranslation.PriorityText.GetString()}: {SyncManager.MainFont}{data.TeamOptions.LocalTeamPriority}</font>.", false);
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            ModdedTeam moddedTeam = messageReader.ReadTeam();
            moddedTeam.TeamOptions.NonHostTeamCount = messageReader.ReadPackedInt32();
            moddedTeam.TeamOptions.NonHostTeamPriority = messageReader.ReadPackedInt32();

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{moddedTeam.TeamColor.ToTextColor()}{moddedTeam.TeamName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{moddedTeam.TeamOptions.NonHostTeamCount}</font>, " +
                $"{FungleTranslation.PriorityText.GetString()}: {SyncManager.MainFont}{moddedTeam.TeamOptions.NonHostTeamPriority}</font>.", !RpcSyncEverything.UnSynced);
        }
    }
}
