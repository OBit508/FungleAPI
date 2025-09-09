using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Configuration;
using FungleAPI.Patches;
using FungleAPI.Role;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSyncAllConfigs : CustomRpc<string>
    {
        public override void Write(MessageWriter writer, string value)
        {
            writer.Write(ConfigurationManager.Configs.Count);
            for (int i = 0; i < ConfigurationManager.Configs.Count; i++)
            {
                writer.WriteConfig(ConfigurationManager.Configs.Values.ToArray()[i]);
                writer.Write(ConfigurationManager.Configs.Values.ToArray()[i].GetValue());
            }
            writer.Write(ConfigurationManager.RoleCounts.Count);
            for (int i = 0; i < ConfigurationManager.RoleCounts.Count; i++)
            {
                writer.WriteCount(ConfigurationManager.RoleCounts[i]);
                writer.Write(ConfigurationManager.RoleCounts[i].GetCount());
            }
            writer.Write(ConfigurationManager.RoleChances.Count);
            for (int i = 0; i < ConfigurationManager.RoleChances.Count; i++)
            {
                writer.WriteChance(ConfigurationManager.RoleChances[i]);
                writer.Write(ConfigurationManager.RoleChances[i].GetChance());
            }
        }
        public override void Handle(MessageReader reader)
        {
            try
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    ModdedOption option = reader.ReadConfig();
                    option.SetValue(reader.ReadString());
                }
                int count2 = reader.ReadInt32();
                for (int i = 0; i < count2; i++)
                {
                    RoleCount c = reader.ReadCount();
                    c.SetCount(reader.ReadInt32());
                }
                int count3 = reader.ReadInt32();
                for (int i = 0; i < count2; i++)
                {
                    RoleChance c = reader.ReadChance();
                    c.SetChance(reader.ReadInt32());
                }
            }
            catch 
            {
                AmongUsClient.Instance.ExitGame(AmongUsClientPatch.FailedToSyncOptionsError);
            }
        }
    }
}
