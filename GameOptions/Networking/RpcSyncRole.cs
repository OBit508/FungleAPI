using FungleAPI.Base.Rpc;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Networking;
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

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncRole : AdvancedRpc<ICustomRole>
    {
        public override bool RequiresNetObject => false;
        public override void Write(MessageWriter messageWriter, ICustomRole data)
        {
            messageWriter.WriteRole(data as RoleBehaviour);
            messageWriter.WritePacked(data.RoleOptions.LocalRoleCount);
            messageWriter.WritePacked(data.RoleOptions.LocalRoleChance);

            if (!RpcSyncEverything.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{data.RoleColor.ToTextColor()}{data.RoleName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{data.RoleOptions.LocalRoleCount}</font>, " +
                $"{FungleTranslation.ChanceText.GetString()}: {SyncManager.MainFont}{data.RoleOptions.LocalRoleChance}%</font>.", false);

                if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.Plugin == data.RoleOptions.Plugin)
                {
                    LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                }
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            ICustomRole customRole = messageReader.ReadRole().CustomRole();
            customRole.RoleOptions.NonHostRoleCount = messageReader.ReadPackedInt32();
            customRole.RoleOptions.NonHostRoleChance = messageReader.ReadPackedInt32();

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{customRole.RoleColor.ToTextColor()}{customRole.RoleName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{customRole.RoleOptions.NonHostRoleCount}</font>, " +
                $"{FungleTranslation.ChanceText.GetString()}: {SyncManager.MainFont}{customRole.RoleOptions.NonHostRoleChance}%</font>.", !RpcSyncEverything.UnSynced);

            if (!RpcSyncEverything.UnSynced && LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.Plugin == customRole.RoleOptions.Plugin)
            {
                LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
            }
        }
    }
}
