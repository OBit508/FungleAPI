using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Player.Networking.Data
{
    internal struct MurderData
    {
        public PlayerControl Target;
        public MurderResultFlags MurderResult;
        public bool ResetKillTimer;
        public bool CreateDeadBody; 
        public bool Teleport;
        public bool ShowAnim;
        public bool PlayKillSound;
        public MurderData(PlayerControl targer, MurderResultFlags murderResult, bool resetKillTimer, bool createDeadBody, bool teleport, bool showAnim, bool playKillSound)
        {
            Target = targer;
            MurderResult = murderResult;
            ResetKillTimer = resetKillTimer;
            CreateDeadBody = createDeadBody;
            Teleport = teleport;
            ShowAnim = showAnim;
            PlayKillSound = playKillSound;
        }
        public MurderData(MessageReader messageReader)
        {
            Target = messageReader.ReadNetObject<PlayerControl>();
            MurderResult = (MurderResultFlags)messageReader.ReadByte();
            ResetKillTimer = messageReader.ReadBoolean();
            CreateDeadBody = messageReader.ReadBoolean();
            Teleport = messageReader.ReadBoolean();
            ShowAnim = messageReader.ReadBoolean();
            PlayKillSound = messageReader.ReadBoolean();
        }
        public void Serialize(MessageWriter messageWriter)
        {
            messageWriter.WriteNetObject(Target);
            messageWriter.Write((byte)MurderResult);
            messageWriter.Write(ResetKillTimer);
            messageWriter.Write(CreateDeadBody);
            messageWriter.Write(Teleport);
            messageWriter.Write(ShowAnim);
            messageWriter.Write(PlayKillSound);
        }
    }
}
