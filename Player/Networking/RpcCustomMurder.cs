using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Player;
using FungleAPI.Player.Networking.Data;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using MS.Internal.Xml.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Player.Networking
{
    internal class RpcCustomMurder : AdvancedRpc<(MurderData, PlayerControl), PlayerControl>
    {
        public override void Write(PlayerControl innerNetObject, MessageWriter messageWriter, (MurderData, PlayerControl) data)
        {
            messageWriter.WritePlayer(data.Item2);
            data.Item1.Serialize(messageWriter);

            MurderResultFlags murderResultFlags = (data.Item1.DidSucceed ? MurderResultFlags.Succeeded : MurderResultFlags.FailedError);
            MurderResultFlags murderResultFlags2 = MurderResultFlags.DecisionByHost | murderResultFlags;

            innerNetObject.CustomMurderPlayer(data.Item1.Target, murderResultFlags2, data.Item1.ResetKillTimer, data.Item1.CreateDeadBody, data.Item1.Teleport, data.Item1.ShowAnim, data.Item1.PlayKillSound);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            PlayerControl source = messageReader.ReadPlayer();
            MurderData murderData = new MurderData(messageReader);

            MurderResultFlags murderResultFlags = (murderData.DidSucceed ? MurderResultFlags.Succeeded : MurderResultFlags.FailedError);
            MurderResultFlags murderResultFlags2 = MurderResultFlags.DecisionByHost | murderResultFlags;

            source.CustomMurderPlayer(murderData.Target, murderResultFlags2, murderData.ResetKillTimer, murderData.CreateDeadBody, murderData.Teleport, murderData.ShowAnim, murderData.PlayKillSound);
        }
    }
}
