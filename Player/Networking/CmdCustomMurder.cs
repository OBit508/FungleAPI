using FungleAPI.Base.Rpc;
using FungleAPI.Player;
using FungleAPI.Player.Networking.Data;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Player.Networking
{
    internal class CmdCustomMurder : AdvancedRpc<MurderData, PlayerControl>
    {
        public override void Write(PlayerControl innerNetObject, MessageWriter messageWriter, MurderData value)
        {
            value.Serialize(messageWriter);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            MurderData murderData = new MurderData(messageReader);
            innerNetObject.CheckCustomMurder(murderData.Target, murderData.MurderResult, murderData.ResetKillTimer, murderData.CreateDeadBody, murderData.Teleport, murderData.ShowAnim, murderData.PlayKillSound);
        }
    }
}
