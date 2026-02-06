using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.GameOver;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime;
using Il2CppSystem.Runtime.Serialization;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace FungleAPI.Networking
{
    /// <summary>
    /// 
    /// </summary>
    public static class MessageExtension
    {
        /// <summary>
        /// 
        /// </summary>
        public static void WriteDeadBody(this MessageWriter Writer, DeadBody value)
        {
            Writer.Write(value.ParentId);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteVector2(this MessageWriter Writer, Vector2 vector)
        {
            Writer.Write(vector.x);
            Writer.Write(vector.y);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteVector3(this MessageWriter Writer, Vector3 vector)
        {
            Writer.Write(vector.x);
            Writer.Write(vector.y);
            Writer.Write(vector.z);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteOption(this MessageWriter Writer, ModdedOption config)
        {
            Writer.Write(config.FullConfigName);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteCountAndChance(this MessageWriter Writer, RoleCountAndChance count)
        {
            Writer.Write(count.Name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteColor(this MessageWriter Writer, Color color)
        {
            Writer.Write(color.r);
            Writer.Write(color.g);
            Writer.Write(color.b);
            Writer.Write(color.a);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteMod(this MessageWriter Writer, ModPlugin.Mod mod)
        {
            Writer.Write(mod.GUID);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteGameOver(this MessageWriter Writer, CustomGameOver customGameOver)
        {
            Writer.Write(customGameOver.GameOverId);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteRole(this MessageWriter Writer, RoleBehaviour role)
        {
            Writer.Write((int)role.Role);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteCountAndPriority(this MessageWriter Writer, TeamCountAndPriority count)
        {
            Writer.Write(count.Name);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteTeam(this MessageWriter Writer, ModdedTeam team)
        {
            Writer.Write(team.TeamId);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WriteRPC(this MessageWriter Writer, RpcHelper rpcHelper)
        {
            Writer.Write(rpcHelper.RpcId);
        }
        /// <summary>
        /// 
        /// </summary>
        public static void WritePlugin(this MessageWriter Writer, ModPlugin plugin)
        {
            Writer.WriteMod(plugin.LocalMod);
        }
        /// <summary>
        /// 
        /// </summary>
        public static Vector2 ReadVector2(this MessageReader Reader)
        {
            float x = Reader.ReadSingle();
            float y = Reader.ReadSingle();
            return new Vector2(x, y);
        }
        /// <summary>
        /// 
        /// </summary>
        public static Vector2 ReadVector3(this MessageReader Reader)
        {
            float x = Reader.ReadSingle();
            float y = Reader.ReadSingle();
            float z = Reader.ReadSingle();
            return new Vector3(x, y, z);
        }
        /// <summary>
        /// 
        /// </summary>
        public static DeadBody ReadBody(this MessageReader Reader)
        {
            return Helpers.GetBodyById(Reader.ReadByte());
        }
        /// <summary>
        /// 
        /// </summary>
        public static ModdedOption ReadOption(this MessageReader Reader)
        {
            string fullConfigName = Reader.ReadString();
            foreach (ModdedOption config in ConfigurationManager.Options)
            {
                if (config.FullConfigName == fullConfigName)
                {
                    return config;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public static RoleCountAndChance ReadCountAndChance(this MessageReader Reader)
        {
            string fullCountName = Reader.ReadString();
            return ConfigurationManager.RoleCountsAndChances.FirstOrDefault(count => count.Name == fullCountName);
        }
        /// <summary>
        /// 
        /// </summary>
        public static Color ReadColor(this MessageReader Reader)
        {
            float r = Reader.ReadSingle();
            float g = Reader.ReadSingle();
            float b = Reader.ReadSingle();
            float a = Reader.ReadSingle();
            return new Color(r, g, b, a);
        }
        /// <summary>
        /// 
        /// </summary>
        public static ModPlugin.Mod ReadMod(this MessageReader Reader)
        {
            string GUID = Reader.ReadString();
            return ModPlugin.Mod.AllMods.FirstOrDefault(m => m.GUID == GUID);
        }
        /// <summary>
        /// 
        /// </summary>
        public static CustomGameOver ReadGameOver(this MessageReader Reader)
        {
            return GameOverManager.GetGameOverById(Reader.ReadInt32());
        }
        /// <summary>
        /// 
        /// </summary>
        public static RoleBehaviour ReadRole(this MessageReader Reader)
        {
            return RoleManager.Instance.GetRole((RoleTypes)Reader.ReadInt32());
        }
        /// <summary>
        /// 
        /// </summary>
        public static TeamCountAndPriority ReadCountAndPriority(this MessageReader Reader)
        {
            string fullCountName = Reader.ReadString();
            return ConfigurationManager.TeamCountAndPriorities.FirstOrDefault(count => count.Name == fullCountName);
        }
        /// <summary>
        /// 
        /// </summary>
        public static ModdedTeam ReadTeam(this MessageReader Reader)
        {
            int id = Reader.ReadInt32();
            return ModdedTeam.Teams.FirstOrDefault(t => t.TeamId == id);
        }
        /// <summary>
        /// 
        /// </summary>
        public static RpcHelper ReadRPC(this MessageReader Reader)
        {
            int id = Reader.ReadInt32();
            return CustomRpcManager.AllRpc.FirstOrDefault(r => r.RpcId == id);
        }
        /// <summary>
        /// 
        /// </summary>
        public static ModPlugin ReadPlugin(this MessageReader Reader)
        {
            return Reader.ReadMod().Plugin;
        }
    }
}
