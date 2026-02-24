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
    /// A class to help send and read values ​​in Hazel
    /// </summary>
    public static class MessageExtension
    {
        /// <summary>
        /// Write a dead body
        /// </summary>
        public static void WriteDeadBody(this MessageWriter messageWriter, DeadBody value)
        {
            messageWriter.Write(value.ParentId);
        }
        /// <summary>
        /// Write a vector2
        /// </summary>
        public static void WriteVector2(this MessageWriter messageWriter, Vector2 vector)
        {
            messageWriter.Write(vector.x);
            messageWriter.Write(vector.y);
        }
        /// <summary>
        /// Write a vector3
        /// </summary>
        public static void WriteVector3(this MessageWriter messageWriter, Vector3 vector)
        {
            messageWriter.Write(vector.x);
            messageWriter.Write(vector.y);
            messageWriter.Write(vector.z);
        }
        /// <summary>
        /// Write a option
        /// </summary>
        public static void WriteOption(this MessageWriter messageWriter, ModdedOption config)
        {
            messageWriter.Write(config.FullConfigName);
        }
        /// <summary>
        ///  Write a color
        /// </summary>
        public static void WriteColor(this MessageWriter messageWriter, Color color)
        {
            messageWriter.Write(color.r);
            messageWriter.Write(color.g);
            messageWriter.Write(color.b);
            messageWriter.Write(color.a);
        }
        /// <summary>
        /// Write a mod
        /// </summary>
        public static void WriteMod(this MessageWriter messageWriter, ModPlugin.Mod mod)
        {
            messageWriter.Write(mod.GUID);
        }
        /// <summary>
        /// Write a game over
        /// </summary>
        public static void WriteGameOver(this MessageWriter messageWriter, CustomGameOver customGameOver)
        {
            messageWriter.Write(customGameOver.GameOverId);
        }
        /// <summary>
        /// Write a role
        /// </summary>
        public static void WriteRole(this MessageWriter messageWriter, RoleBehaviour role)
        {
            messageWriter.Write((int)role.Role);
        }
        /// <summary>
        /// Write a team
        /// </summary>
        public static void WriteTeam(this MessageWriter messageWriter, ModdedTeam team)
        {
            messageWriter.Write(team.TeamId);
        }
        /// <summary>
        /// Write a rpc
        /// </summary>
        public static void WriteRPC(this MessageWriter messageWriter, RpcHelper rpcHelper)
        {
            messageWriter.Write(rpcHelper.RpcId);
        }
        /// <summary>
        /// Write a plugin
        /// </summary>
        public static void WritePlugin(this MessageWriter messageWriter, ModPlugin plugin)
        {
            messageWriter.WriteMod(plugin.LocalMod);
        }
        /// <summary>
        /// Write a config helper
        /// </summary>
        public static void WriteConfigHelper(this MessageWriter messageWriter, ConfigHelper configHelper)
        {
            messageWriter.Write(configHelper.Name);
        }
        /// <summary>
        /// Read a body
        /// </summary>
        public static DeadBody ReadBody(this MessageReader messageReader)
        {
            return Helpers.GetBodyById(messageReader.ReadByte());
        }
        /// <summary>
        /// Read a vector2
        /// </summary>
        public static Vector2 ReadVector2(this MessageReader messageReader)
        {
            float x = messageReader.ReadSingle();
            float y = messageReader.ReadSingle();
            return new Vector2(x, y);
        }
        /// <summary>
        /// Read a vector3
        /// </summary>
        public static Vector2 ReadVector3(this MessageReader messageReader)
        {
            float x = messageReader.ReadSingle();
            float y = messageReader.ReadSingle();
            float z = messageReader.ReadSingle();
            return new Vector3(x, y, z);
        }
        /// <summary>
        /// Read a option
        /// </summary>
        public static ModdedOption ReadOption(this MessageReader messageReader)
        {
            string fullConfigName = messageReader.ReadString();
            return ConfigurationManager.Options.FirstOrDefault(c => c.FullConfigName == fullConfigName);
        }
        /// <summary>
        /// Read a color
        /// </summary>
        public static Color ReadColor(this MessageReader messageReader)
        {
            float r = messageReader.ReadSingle();
            float g = messageReader.ReadSingle();
            float b = messageReader.ReadSingle();
            float a = messageReader.ReadSingle();
            return new Color(r, g, b, a);
        }
        /// <summary>
        /// Read a mod
        /// </summary>
        public static ModPlugin.Mod ReadMod(this MessageReader messageReader)
        {
            string GUID = messageReader.ReadString();
            return ModPlugin.Mod.AllMods.FirstOrDefault(m => m.GUID == GUID);
        }
        /// <summary>
        /// Read a game over
        /// </summary>
        public static CustomGameOver ReadGameOver(this MessageReader messageReader)
        {
            return GameOverManager.GetGameOverById(messageReader.ReadInt32());
        }
        /// <summary>
        /// Read a role
        /// </summary>
        public static RoleBehaviour ReadRole(this MessageReader messageReader)
        {
            return RoleManager.Instance.GetRole((RoleTypes)messageReader.ReadInt32());
        }
        /// <summary>
        /// Read a team
        /// </summary>
        public static ModdedTeam ReadTeam(this MessageReader messageReader)
        {
            int id = messageReader.ReadInt32();
            return ModdedTeam.Teams.FirstOrDefault(t => t.TeamId == id);
        }
        /// <summary>
        /// Read a rpc
        /// </summary>
        public static RpcHelper ReadRPC(this MessageReader messageReader)
        {
            int id = messageReader.ReadInt32();
            return CustomRpcManager.AllRpc.FirstOrDefault(r => r.RpcId == id);
        }
        /// <summary>
        /// Read a plugin
        /// </summary>
        public static ModPlugin ReadPlugin(this MessageReader messageReader)
        {
            return messageReader.ReadMod().Plugin;
        }
        /// <summary>
        /// Read a config helper
        /// </summary>
        public static T ReadConfigHelper<T>(this MessageReader messageReader) where T : ConfigHelper
        {
            string name = messageReader.ReadString();
            return ConfigurationManager.ConfigHelpers.FirstOrDefault(c => c.Name == name).SimpleCast<T>();
        }
    }
}
