using FungleAPI.Components;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using FungleAPI.Utilities;
using FungleAPI.Configuration;
using FungleAPI.Role;
using FungleAPI.Configuration.Attributes;
using Il2CppSystem.Runtime.Serialization;
using Il2CppInterop.Runtime;
using AmongUs.GameOptions;
using FungleAPI.GameOver;

namespace FungleAPI.Networking
{
    public static class MessageExtension
    {
        public static void WritePlayer(this MessageWriter Writer, PlayerControl value)
        {
            Writer.Write(value.PlayerId);
        }
        public static void WriteDeadBody(this MessageWriter Writer, DeadBody value)
        {
            Writer.Write(value.ParentId);
        }
        public static void WriteVector2(this MessageWriter Writer, Vector2 vector)
        {
            Writer.Write(vector.x);
            Writer.Write(vector.y);
        }
        public static void WriteVector3(this MessageWriter Writer, Vector3 vector)
        {
            Writer.Write(vector.x);
            Writer.Write(vector.y);
            Writer.Write(vector.z);
        }
        public static void WriteConfig(this MessageWriter Writer, ModdedOption config)
        {
            Writer.Write(config.FullConfigName);
        }
        public static void WriteCountAndChance(this MessageWriter Writer, RoleCountAndChance count)
        {
            Writer.Write(count.Name);
        }
        public static void WriteCachedPlayerData(this MessageWriter Writer, CachedPlayerData cachedPlayerData, int clientId)
        {
            Writer.Write(cachedPlayerData.PlayerName);
            cachedPlayerData.Outfit.Serialize(Writer);
            Writer.Write(clientId);
            Writer.Write(cachedPlayerData.IsDead);
            Writer.Write((int)cachedPlayerData.RoleWhenAlive);
        }
        public static void WriteColor(this MessageWriter Writer, Color color)
        {
            Writer.Write(color.r);
            Writer.Write(color.g);
            Writer.Write(color.b);
            Writer.Write(color.a);
        }
        public static void WriteMod(this MessageWriter Writer, ModPlugin.Mod mod)
        {
            Writer.Write(mod.Version);
            Writer.Write(mod.Name);
            Writer.Write(mod.RealName);
            Writer.Write(mod.GUID);
        }
        public static void WriteGameOver(this MessageWriter Writer, CustomGameOver customGameOver)
        {
            Writer.Write(customGameOver.GetType().FullName);
        }
        public static Vector2 ReadVector2(this MessageReader Reader)
        {
            float x = Reader.ReadSingle();
            float y = Reader.ReadSingle();
            return new Vector2(x, y);
        }
        public static Vector2 ReadVector3(this MessageReader Reader)
        {
            float x = Reader.ReadSingle();
            float y = Reader.ReadSingle();
            float z = Reader.ReadSingle();
            return new Vector3(x, y, z);
        }
        public static PlayerControl ReadPlayer(this MessageReader Reader)
        {
            return Helpers.GetPlayerById(Reader.ReadByte());
        }
        public static DeadBody ReadBody(this MessageReader Reader)
        {
            return Helpers.GetBodyById(Reader.ReadByte());
        }
        public static ModdedOption ReadConfig(this MessageReader Reader)
        {
            string fullConfigName = Reader.ReadString();
            foreach (ModdedOption config in ConfigurationManager.Configs.Values)
            {
                if (config.FullConfigName == fullConfigName)
                {
                    return config;
                }
            }
            return null;
        }
        public static RoleCountAndChance ReadCountAndChance(this MessageReader Reader)
        {
            string fullCountName = Reader.ReadString();
            return ConfigurationManager.RoleCountsAndChances.FirstOrDefault(count => count.Name == fullCountName);
        }
        public static CachedPlayerData ReadCachedPlayerData(this MessageReader Reader)
        {
            CachedPlayerData cachedPlayerData = new CachedPlayerData(PlayerControl.LocalPlayer.Data);
            cachedPlayerData.PlayerName = Reader.ReadString();
            cachedPlayerData.Outfit = new NetworkedPlayerInfo.PlayerOutfit();
            cachedPlayerData.Outfit.Deserialize(Reader);
            cachedPlayerData.IsYou = AmongUsClient.Instance.ClientId == Reader.ReadInt32();
            cachedPlayerData.IsDead = Reader.ReadBoolean();
            cachedPlayerData.RoleWhenAlive = (RoleTypes)Reader.ReadInt32();
            return cachedPlayerData;
        }
        public static Color ReadColor(this MessageReader Reader)
        {
            float r = Reader.ReadSingle();
            float g = Reader.ReadSingle();
            float b = Reader.ReadSingle();
            float a = Reader.ReadSingle();
            return new Color(r, g, b, a);
        }
        public static ModPlugin.Mod ReadMod(this MessageReader Reader)
        {
            string Version = Reader.ReadString();
            string Name = Reader.ReadString();
            string RealName = Reader.ReadString();
            string GUID = Reader.ReadString();
            ModPlugin.Mod mod = new ModPlugin.Mod(Version, Name, RealName, GUID);
            mod.Plugin = ModPlugin.AllPlugins.FirstOrDefault(pl => pl.LocalMod.Equals(mod));
            return mod;
        }
        public static CustomGameOver ReadGameOver(this MessageReader Reader)
        {
            string fullName = Reader.ReadString();
            foreach (CustomGameOver gameOver in GameOverManager.AllCustomGameOver)
            {
                if (gameOver.GetType().FullName == fullName)
                {
                    return gameOver;
                }
            }
            return null;
        }
    }
}
