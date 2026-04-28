using AmongUs.GameOptions;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.GameMode;
using FungleAPI.GameOptions;
using FungleAPI.GameOver;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppInterop.Runtime;
using Il2CppSystem.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace FungleAPI.Networking
{
    /// <summary>
    /// Extensions for the Hazel
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
        public static void WriteOption(this MessageWriter messageWriter, IModdedOption config)
        {
            messageWriter.Write(config.OptionId);
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
        public static void WriteMod(this MessageWriter messageWriter, BepInMod mod)
        {
            messageWriter.Write(mod.GUID);
        }
        /// <summary>
        /// Write a game mode
        /// </summary>
        public static void WriteGameMode(this MessageWriter messageWriter, CustomGameMode customGameMode)
        {
            messageWriter.Write(customGameMode.GameModeId);
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
        /// Read a body
        /// </summary>
        public static DeadBody ReadBody(this MessageReader messageReader)
        {
            return BodyUtils.GetBodyById(messageReader.ReadByte());
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
        public static IModdedOption ReadOption(this MessageReader messageReader)
        {
            string optionId = messageReader.ReadString();
            if (OptionManager.AllOptions.TryGetValue(optionId, out IModdedOption moddedOption))
            {
                return moddedOption;
            }
            return null;
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
        public static BepInMod ReadMod(this MessageReader messageReader)
        {
            string GUID = messageReader.ReadString();
            return BepInMod.Mods.Values.FirstOrDefault(m => m.GUID == GUID);
        }
        /// <summary>
        /// Read a game mode
        /// </summary>
        public static CustomGameMode ReadGameMode(this MessageReader messageReader)
        {
            return GameModeManager.GameModes.Values.FirstOrDefault(g => g.GameModeId == messageReader.ReadInt32());
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
            return ModdedTeamManager.Teams.Values.FirstOrDefault(t => t.TeamId == id);
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
            return ModPluginManager.GetModPlugin(messageReader.ReadMod().Assembly);
        }
    }
}
