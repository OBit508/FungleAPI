using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Utilities;
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
    public class RpcSyncAllConfigs : CustomRpc
    {
        public override void Write(MessageWriter writer)
        {
            writer.Write(ConfigurationManager.Configs.Count);
            for (int i = 0; i < ConfigurationManager.Configs.Count; i++)
            {
                writer.WriteConfig(ConfigurationManager.Configs.Values.ToArray()[i]);
                writer.Write(ConfigurationManager.Configs.Values.ToArray()[i].GetValue());
            }
            writer.Write(ConfigurationManager.RoleCountsAndChances.Count);
            for (int i = 0; i < ConfigurationManager.RoleCountsAndChances.Count; i++)
            {
                writer.WriteCountAndChance(ConfigurationManager.RoleCountsAndChances[i]);
                writer.Write(ConfigurationManager.RoleCountsAndChances[i].GetCount());
                writer.Write(ConfigurationManager.RoleCountsAndChances[i].GetChance());
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
                    RoleCountAndChance c = reader.ReadCountAndChance();
                    c.SetCount(reader.ReadInt32());
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
