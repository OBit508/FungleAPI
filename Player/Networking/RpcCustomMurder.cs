using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
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
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Player.Networking
{
    internal class RpcCustomMurder : AdvancedRpc<MurderData, PlayerControl>
    {
        public override void Write(PlayerControl innerNetObject, MessageWriter messageWriter, MurderData value)
        {
            value.Serialize(messageWriter);
            innerNetObject.CustomMurderPlayer(value.Target, value.MurderResult, value.ResetKillTimer, value.CreateDeadBody, value.Teleport, value.ShowAnim, value.PlayKillSound);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            MurderData murderData = new MurderData(messageReader);
            innerNetObject.CustomMurderPlayer(murderData.Target, murderData.MurderResult, murderData.ResetKillTimer, murderData.CreateDeadBody, murderData.Teleport, murderData.ShowAnim, murderData.PlayKillSound);
        }
    }
}
