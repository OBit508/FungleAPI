using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Teams;
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
    internal class RpcSyncTeam : AdvancedRpc<ModdedTeam, PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, ModdedTeam data)
        {
            messageWriter.WriteTeam(data);
            messageWriter.Write(data.TeamOptions.LocalTeamCount.Value);
            messageWriter.Write(data.TeamOptions.LocalTeamPriority.Value);

            if (!RpcSyncGRnT.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{data.TeamColor.ToTextColor()}{data.TeamName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{data.TeamOptions.LocalTeamCount.Value}</font>, " +
                $"{FungleTranslation.PriorityText.GetString()}: {SyncManager.MainFont}{data.TeamOptions.LocalTeamPriority.Value}</font>.", false);

                if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == data.TeamOptions.Plugin.ModAssembly)
                {
                    LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                }
            }
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject.OwnerId != AmongUsClient.Instance.HostId) return;

            ModdedTeam moddedTeam = messageReader.ReadTeam();
            moddedTeam.TeamOptions.NonHostTeamCount = messageReader.ReadByte();
            moddedTeam.TeamOptions.NonHostTeamPriority = messageReader.ReadByte();

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{moddedTeam.TeamColor.ToTextColor()}{moddedTeam.TeamName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{moddedTeam.TeamOptions.NonHostTeamCount}</font>, " +
                $"{FungleTranslation.PriorityText.GetString()}: {SyncManager.MainFont}{moddedTeam.TeamOptions.NonHostTeamPriority}</font>.", !RpcSyncGRnT.UnSynced);

            if (!RpcSyncGRnT.UnSynced && LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == moddedTeam.TeamOptions.Plugin.ModAssembly)
            {
                LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
            }
        }
    }
}
