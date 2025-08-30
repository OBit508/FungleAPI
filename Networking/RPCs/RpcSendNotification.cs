using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static Rewired.Platforms.Custom.CustomPlatformUnifiedKeyboardSource.KeyPropertyMap;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSendNotification : CustomRpc<(string text, bool playSound, bool handlePlaySound)>
    {
        public override void Handle(MessageReader reader)
        {
            string text = reader.ReadString();
            bool playSound = reader.ReadBoolean();
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, text, playSound);
        }
        public override void Write(MessageWriter writer, (string text, bool playSound, bool handlePlaySound) value)
        {
            writer.Write(value.text);
            writer.Write(value.handlePlaySound);
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value.text, value.playSound);
        }
    }
}
