using FungleAPI.AntiCheat;
using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using FungleAPI.GameOptions.Patches;
using FungleAPI.GameModes;
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
    internal class RpcSyncGamemode : SimpleRpc<NetworkedPlayerInfo>
    {
        public override void Write(MessageWriter messageWriter)
        {
            BaseGameMode baseGameMode = GameModeManager.GameModes[GameModeManager.HostValue.Value];

            messageWriter.WritePacked(GameModeManager.HostValue.Value);

            if (!RpcSyncEverything.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{FungleTranslation.GameModeText.GetString()}</font>: " +
                $"{SyncManager.MainFont}{baseGameMode.GameModeName.GetString()}</font>", false);

                if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == FungleApiPlugin.Plugin.ModAssembly)
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

            uint gameModeId = messageReader.ReadPackedUInt32();

            BaseGameMode baseGameMode = GameModeManager.GameModes[gameModeId];

            GameModeManager.NonHostValue = gameModeId;

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{FungleTranslation.GameModeText.GetString()}</font>: " +
                $"{SyncManager.MainFont}{baseGameMode.GameModeName.GetString()}</font>", !RpcSyncEverything.UnSynced);

            if (!RpcSyncEverything.UnSynced && LobbyViewSettingsPanePatch.Tab != null)
            {
                if (LobbyViewSettingsPanePatch.Tab.TabAssembly == FungleApiPlugin.Plugin.ModAssembly)
                {
                    LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                }
            }
        }
    }
}
