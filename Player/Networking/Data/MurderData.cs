using FungleAPI.Networking;
using Hazel;
using InnerNet;
using System;

namespace FungleAPI.Player.Networking.Data
{
    [Flags]
    internal enum CustomMurderFlags : byte
    {
        None = 0,
        DidSucceed = 1 << 0,
        ResetKillTimer = 1 << 1,
        CreateDeadBody = 1 << 2,
        Teleport = 1 << 3,
        ShowAnim = 1 << 4,
        PlayKillSound = 1 << 5,
    }
    internal struct MurderData
    {
        public PlayerControl Target;
        public bool DidSucceed;
        public bool ResetKillTimer;
        public bool CreateDeadBody;
        public bool Teleport;
        public bool ShowAnim;
        public bool PlayKillSound;
        public MurderData(PlayerControl target, bool didSucceed, bool resetKillTimer, bool createDeadBody, bool teleport, bool showAnim, bool playKillSound)
        {
            Target = target;
            DidSucceed = didSucceed;
            ResetKillTimer = resetKillTimer;
            CreateDeadBody = createDeadBody;
            Teleport = teleport;
            ShowAnim = showAnim;
            PlayKillSound = playKillSound;
        }
        public MurderData(MessageReader reader)
        {
            Target = reader.ReadPlayer();
            CustomMurderFlags flags = (CustomMurderFlags)reader.ReadByte();
            DidSucceed = (flags & CustomMurderFlags.DidSucceed) != 0;
            ResetKillTimer = (flags & CustomMurderFlags.ResetKillTimer) != 0;
            CreateDeadBody = (flags & CustomMurderFlags.CreateDeadBody) != 0;
            Teleport = (flags & CustomMurderFlags.Teleport) != 0;
            ShowAnim = (flags & CustomMurderFlags.ShowAnim) != 0;
            PlayKillSound = (flags & CustomMurderFlags.PlayKillSound) != 0;
        }
        public void Serialize(MessageWriter writer)
        {
            writer.WritePlayer(Target);
            CustomMurderFlags flags = CustomMurderFlags.None;
            if (DidSucceed)
            {
                flags |= CustomMurderFlags.DidSucceed;
            }
            if (ResetKillTimer)
            {
                flags |= CustomMurderFlags.ResetKillTimer;
            }
            if (CreateDeadBody)
            {
                flags |= CustomMurderFlags.CreateDeadBody;
            }
            if (Teleport)
            {
                flags |= CustomMurderFlags.Teleport;
                if (ShowAnim)
                {
                    flags |= CustomMurderFlags.ShowAnim;
                }
                if (PlayKillSound)
                {
                    flags |= CustomMurderFlags.PlayKillSound;
                }
                writer.Write((byte)flags);
            }
        }
    }
}