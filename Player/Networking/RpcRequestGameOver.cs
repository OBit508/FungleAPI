using FungleAPI.Base.Rpc;
using FungleAPI.GameOver;
using FungleAPI.Networking;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Player.Networking
{
    internal class RpcRequestGameOver : AdvancedRpc<CustomGameOver, PlayerControl>
    {
        public override void Write(PlayerControl innerNetObejct, MessageWriter messageWriter, CustomGameOver value)
        {
            messageWriter.WriteGameOver(value);
            value.SerializeRequest(messageWriter, innerNetObejct);
        }
        public override void Handle(PlayerControl innerNetObejct, MessageReader messageReader)
        {
            CustomGameOver gameOver = messageReader.ReadGameOver();
            gameOver.DeserializeRequest(messageReader, innerNetObejct);
            if (AmongUsClient.Instance.AmHost && GameOverManager.AllowNonHostGameOverRequest)
            {
                GameManager.Instance.RpcEndGame(gameOver);
            }
        }
    }
}
