using FungleAPI.AntiCheat;
using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncRole : AdvancedRpc<ICustomRole, NetworkedPlayerInfo>
    {
        public override void Write(MessageWriter messageWriter, ICustomRole data)
        {
            messageWriter.WriteRole(data as RoleBehaviour);
            messageWriter.WritePacked(data.RoleOptions.LocalRoleCount.Value);
            messageWriter.WritePacked(data.RoleOptions.LocalRoleChance.Value);

            if (!RpcSyncGRnT.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{data.RoleColor.ToTextColor()}{data.RoleName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{data.RoleOptions.LocalRoleCount.Value}</font>, " +
                $"{FungleTranslation.ChanceText.GetString()}: {SyncManager.MainFont}{data.RoleOptions.LocalRoleChance.Value}%</font>.", false);

                if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == data.RoleOptions.Plugin.ModAssembly)
                {
                    LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                }
            }
        }
        public override void Handle(NetworkedPlayerInfo innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject == null) return;

            if (AntiCheatManager.Active && !innerNetObject.IsHost())
            {
                AntiCheatManager.CheaterFinded(innerNetObject.ClientId);

                return;
            }

            ICustomRole customRole = messageReader.ReadRole().CustomRole();
            customRole.RoleOptions.NonHostRoleCount = messageReader.ReadPackedInt32();
            customRole.RoleOptions.NonHostRoleChance = messageReader.ReadPackedInt32();

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{customRole.RoleColor.ToTextColor()}{customRole.RoleName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{customRole.RoleOptions.NonHostRoleCount}</font>, " +
                $"{FungleTranslation.ChanceText.GetString()}: {SyncManager.MainFont}{customRole.RoleOptions.NonHostRoleChance}%</font>.", !RpcSyncGRnT.UnSynced);

            if (!RpcSyncGRnT.UnSynced && LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == customRole.RoleOptions.Plugin.ModAssembly)
            {
                LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
            }
        }
    }
}
