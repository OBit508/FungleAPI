using FungleAPI.Base.Rpc;
using FungleAPI.GameOver;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    internal class RpcEndGame : AdvancedRpc<CustomGameOver>
    {
        public override void Write(MessageWriter messageWriter, CustomGameOver value)
        {
            messageWriter.WriteGameOver(value);
            value.Serialize(messageWriter);
            if (AmongUsClient.Instance.AmHost)
            {
                GameManager.Instance.RpcEndGame(value.Reason, false);
            }
        }
        public override void Handle(InnerNetObject innerNetObject, MessageReader messageReader)
        {
            CustomGameOver customGameOver = messageReader.ReadGameOver();
            customGameOver.Deserialize(messageReader);
            if (AmongUsClient.Instance.AmHost)
            {
                GameManager.Instance.RpcEndGame(customGameOver.Reason, false);
            }
        }
    }
}
