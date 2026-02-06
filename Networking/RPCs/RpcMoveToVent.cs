using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Player;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Networking.RPCs
{
    /// <summary>
    /// 
    /// </summary>
    public class RpcMoveToVent : AdvancedRpc<VentHelper, PlayerControl>
    {
        public override void Write(PlayerControl innerNetObject, MessageWriter messageWriter, VentHelper value)
        {
            messageWriter.Write(value.vent.Id);
            innerNetObject.GetCurrentVent().TryGetHelper().MoveToVent(innerNetObject, value.vent);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            int id = messageReader.ReadInt32();
            Vent vent = ShipStatus.Instance.AllVents.FirstOrDefault(v => v.Id == id);
            innerNetObject.GetCurrentVent().TryGetHelper().MoveToVent(innerNetObject, vent);
        }
    }
}
