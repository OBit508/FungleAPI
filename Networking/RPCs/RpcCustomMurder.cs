using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.Networking;
using FungleAPI.Utilities;
using Hazel;
using MS.Internal.Xml.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;
using static UnityEngine.GraphicsBuffer;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Player;
using FungleAPI.Base.Rpc;

namespace FungleAPI.Networking.RPCs
{
    public class RpcCustomMurder : AdvancedRpc<(PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound)>
    {
        public override void Write(MessageWriter writer, (PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound) value)
        {
            writer.WritePlayer(value.killer);
            writer.WritePlayer(value.target);
            writer.Write((int)value.resultFlags);
            writer.Write(value.resetKillTimer);
            writer.Write(value.createDeadBody);
            writer.Write(value.teleportMurderer);
            writer.Write(value.showKillAnim);
            writer.Write(value.playKillSound);
            value.killer.CustomMurderPlayer(value.target, value.resultFlags, value.resetKillTimer, value.createDeadBody, value.teleportMurderer, value.showKillAnim, value.playKillSound);
        }
        public override void Handle(MessageReader reader)
        {
            PlayerControl killer = reader.ReadPlayer();
            PlayerControl target = reader.ReadPlayer();
            MurderResultFlags resultFlags = (MurderResultFlags)reader.ReadInt32();
            bool resetKillTimer = reader.ReadBoolean();
            bool createDeadBody = reader.ReadBoolean();
            bool teleportMurderer = reader.ReadBoolean();
            bool showKillAnim = reader.ReadBoolean();
            bool playKillSound = reader.ReadBoolean();
            killer.CustomMurderPlayer(target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound);
        }
    }
}
