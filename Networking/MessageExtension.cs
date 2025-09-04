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
    }
}
