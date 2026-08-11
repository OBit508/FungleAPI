using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Modifiers.Networking
{
    internal class RpcAddModifier : AdvancedRpc<(uint, PlayerControl), PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, (uint, PlayerControl) data)
        {
            messageWriter.WritePacked(data.Item1);
            messageWriter.WritePlayer(data.Item2);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            uint modifierId = messageReader.ReadPackedUInt32();
            PlayerControl playerControl = messageReader.ReadPlayer();
            playerControl.AddModifier(modifierId);
        }
    }
}
