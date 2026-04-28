using FungleAPI.Base.Rpc;
using FungleAPI.GameMode;
using FungleAPI.Networking;
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
    internal class RpcSyncCurrentGameMode : SimpleRpc
    {
        public override void Write(MessageWriter messageWriter)
        {
            CustomGameMode customGameMode = GameModeManager.GetActiveGameMode();
            messageWriter.WriteGameMode(customGameMode);
            AddSettingsChangeMessage(HudManager.Instance.Notifier, customGameMode.GameModeName, false);
        }
        public override void Handle(MessageReader messageReader)
        {
            CustomGameMode customGameMode = messageReader.ReadGameMode();
            GameModeManager.onlineMode = customGameMode.GetType().GetShortUniqueId();
            AddSettingsChangeMessage(HudManager.Instance.Notifier, customGameMode.GameModeName);
        }
        public static void AddSettingsChangeMessage(NotificationPopper notificationPopper, StringNames key, bool playSound = true)
        {
            string item = $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">GameMode</font>:" +
                $"<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\"> {key.GetString()}</font>";
            notificationPopper.SettingsChangeMessageLogic(key, item, playSound);
        }
    }
} 
