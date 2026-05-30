using FungleAPI.Base.Rpc;
using FungleAPI.GModes;
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
    internal class RpcSyncGamemode : SimpleRpc
    {
        public override bool CanAcceptRPCsWithoutInnerNetObject => true;
        public override void Write(MessageWriter messageWriter)
        {
            BaseGameMode baseGameMode = GameModeManager.GameModes[GameModeManager.HostValue.Value];

            messageWriter.WritePacked(GameModeManager.HostValue.Value);

            if (!RpcSyncEverything.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{FungleTranslation.GameModeText.GetString()}</font>: " +
                $"{SyncManager.MainFont}{baseGameMode.GameModeName.GetString()}</font>", false);
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            int gameModeId = messageReader.ReadPackedInt32();

            BaseGameMode baseGameMode = GameModeManager.GameModes[gameModeId];

            GameModeManager.NonHostValue = gameModeId;

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{FungleTranslation.GameModeText.GetString()}</font>: " +
                $"{SyncManager.MainFont}{baseGameMode.GameModeName.GetString()}</font>", !RpcSyncEverything.UnSynced);
        }
    }
}
