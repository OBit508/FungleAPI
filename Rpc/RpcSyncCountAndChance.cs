using AmongUs.GameOptions;
using FungleAPI.Role;
using FungleAPI.Roles;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Rpc
{
    public class RpcSyncCountAndChance : CustomRpc<(ICustomRole role, int count, int chance, string s)>
    {
        public override void Read(MessageReader reader)
        {
            ICustomRole role = CustomRoleManager.GetRole((RoleTypes)reader.ReadInt32());
            role.RoleCount.Value = reader.ReadInt32();
            role.RoleChance.Value = reader.ReadInt32();
            string s = reader.ReadString();
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, s, true);
        }
        public override void Write(MessageWriter writer, (ICustomRole role, int count, int chance, string s) value)
        {
            writer.Write((int)value.role.Role);
            writer.Write(value.count);
            writer.Write(value.chance);
            writer.Write(value.s);
            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, value.s, true);
        }
    }
}
