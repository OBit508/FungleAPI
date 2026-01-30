using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Patches;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppSystem.Runtime.Remoting.Messaging;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSyncAllConfigs : SimpleRpc
    {
        public override void Write(MessageWriter writer)
        {
            writer.Write(ConfigurationManager.Options.Count);
            for (int i = 0; i < ConfigurationManager.Options.Count; i++)
            {
                writer.WriteConfig(ConfigurationManager.Options.ToArray()[i]);
                writer.Write(ConfigurationManager.Options.ToArray()[i].GetValue());
            }
            writer.Write(ConfigurationManager.RoleCountsAndChances.Count);
            for (int i = 0; i < ConfigurationManager.RoleCountsAndChances.Count; i++)
            {
                writer.WriteCountAndChance(ConfigurationManager.RoleCountsAndChances[i]);
                writer.Write(ConfigurationManager.RoleCountsAndChances[i].GetCount());
                writer.Write(ConfigurationManager.RoleCountsAndChances[i].GetChance());
            }
            writer.Write(ConfigurationManager.TeamCountAndPriorities.Count);
            for (int i = 0; i < ConfigurationManager.TeamCountAndPriorities.Count; i++)
            {
                writer.WriteCountAndPriority(ConfigurationManager.TeamCountAndPriorities[i]);
                writer.Write(ConfigurationManager.TeamCountAndPriorities[i].GetCount());
                writer.Write(ConfigurationManager.TeamCountAndPriorities[i].GetPriority());
            }
            writer.Write(GameData.Instance.AllPlayers.Count);
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                writer.Write(networkedPlayerInfo.NetId);
                writer.Write(networkedPlayerInfo.DefaultOutfit.ColorId);
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
                int count3 = reader.ReadInt32();
                for (int i = 0; i < count3; i++)
                {
                    TeamCountAndPriority c = reader.ReadCountAndPriority();
                    c.SetCount(reader.ReadInt32());
                    c.SetPriority(reader.ReadInt32());
                }
                int count4 = reader.ReadInt32();
                for (int i = 0; i < count4; i++)
                {
                    uint client = reader.ReadUInt32();
                    int color = reader.ReadInt32();
                    if (PlayerControlPatch.CachedColors.ContainsKey(client))
                    {
                        PlayerControlPatch.CachedColors.Remove(client);
                    }
                    PlayerControlPatch.CachedColors.Add(client, color);
                }
            }
            catch 
            {
                AmongUsClient.Instance.HandleDisconnect(HandShakeManager.FailedToSyncOptions);
            }
        }
    }
}
