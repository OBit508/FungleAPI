using FungleAPI.Base.Rpc;
using Hazel;

namespace FungleAPI.Modifiers
{
    public sealed class ModifierRpc : SimpleRpc<PlayerControl>
    {
        private uint _modifierId;
        private float _duration;
        private byte _operation;

        public void SendAdd(PlayerControl player, uint modifierId, float duration)
        {
            _modifierId = modifierId;
            _duration = duration;
            _operation = 0;
            Send(player);
        }

        public void SendRemove(PlayerControl player, uint modifierId)
        {
            _modifierId = modifierId;
            _duration = -1f;
            _operation = 1;
            Send(player);
        }

        public void SendClear(PlayerControl player)
        {
            _modifierId = 0;
            _duration = -1f;
            _operation = 2;
            Send(player);
        }

        public override void Write(PlayerControl player, MessageWriter writer)
        {
            writer.WritePacked(_modifierId);
            writer.Write(_duration);
            writer.Write(_operation);
        }

        public override void Handle(PlayerControl player, MessageReader reader)
        {
            var modifierId = reader.ReadPackedUInt32();
            var duration = reader.ReadSingle();
            var operation = reader.ReadByte();
            if (operation == 0) ModifierManager.AddModifier(player, modifierId, duration);
            else if (operation == 1) ModifierManager.RemoveModifier(player, modifierId);
            else ModifierManager.ClearModifiers(player);
        }
    }
}
