using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Networking
{
    internal class RpcSendEndData : AdvancedRpc<(BaseGameOver, object), PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, (BaseGameOver, object) data)
        {
            BaseGameOver.CachedGameOver = data.Item1;

            messageWriter.WriteGameOver(data.Item1);
            data.Item1.SerializeWithData(messageWriter, data.Item2);
            BaseGameOver.CachedGameOver.SetWinners();
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            BaseGameOver.CachedGameOver = messageReader.ReadGameOver();
            BaseGameOver.CachedGameOver.Deserialize(messageReader);
            BaseGameOver.CachedGameOver.SetWinners();
        }
    }
}
