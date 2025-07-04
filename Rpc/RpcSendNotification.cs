using Hazel;
using Sentry.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Rpc
{
    public class RpcSendNotification : CustomRpc<(string text, bool playSound)>
    {
        public override void Read(MessageReader reader)
        {
            string text = reader.ReadString();
            bool playSound = reader.ReadBoolean();
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, text, playSound);
        }
        public override void Write(MessageWriter writer, (string text, bool playSound) value)
        {
            writer.Write(value.text);
            writer.Write(value.playSound);
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value.text, value.playSound);
        }
    }
}
