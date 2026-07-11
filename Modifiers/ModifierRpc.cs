using FungleAPI.Attributes;
using FungleAPI.Base.Rpc;
using Hazel;
using System.Linq;

namespace FungleAPI.Modifiers
{
    public sealed class ModifierRpc : SimpleRpc
    {
        public override bool RequiresNetObject => false;
        private byte _playerId;
        private uint _modifierId;
        private float _duration;
        private bool _remove;

        public void SendAdd(byte playerId, uint modifierId, float duration)
        {
            _playerId = playerId;
            _modifierId = modifierId;
            _duration = duration;
            _remove = false;
            Send();
        }

        public void SendRemove(byte playerId, uint modifierId)
        {
            _playerId = playerId;
            _modifierId = modifierId;
            _duration = -1f;
            _remove = true;
            Send();
        }

        public override void Write(MessageWriter messageWriter)
        {
            messageWriter.Write(_playerId);
            messageWriter.WritePacked(_modifierId);
            messageWriter.Write(_duration);
            messageWriter.Write(_remove);
        }

        public override void Handle(MessageReader messageReader)
        {
            byte playerId = messageReader.ReadByte();
            uint modifierId = messageReader.ReadPackedUInt32();
            float duration = messageReader.ReadSingle();
            bool remove = messageReader.ReadBoolean();
            PlayerControl player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(control => control.PlayerId == playerId);
            if (player == null)
            {
                return;
            }
            if (remove)
            {
                ModifierManager.RemoveModifier(player, modifierId);
                return;
            }
            ModifierManager.AddModifier(player, modifierId, duration);
        }
    }
}
