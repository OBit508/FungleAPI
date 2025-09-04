using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Configuration;
using FungleAPI.ModCompatibility;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Roles;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV09))]
    internal static class RoleOptionsCollectionV09Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AnyRolesEnabled")]
        public static bool AnyRolesEnabledPrefix(RoleOptionsCollectionV09 __instance, ref bool __result)
        {
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<RoleTypes, RoleDataV09> keyValuePair in __instance.roles)
            {
                if (__instance.GetNumPerGame(keyValuePair.Key) > 0)
                {
                    __result = true;
                }
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetChancePerGame")]
        public static bool GetChancePrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            ICustomRole role = CustomRoleManager.GetRole(roleType);
            if (role != null)
            {
                __result = role.RoleChance;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("GetNumPerGame")]
        public static bool GetNumPrefix([HarmonyArgument(0)] RoleTypes roleType, ref int __result)
        {
            ICustomRole role = CustomRoleManager.GetRole(roleType);
            if (role != null)
            {
                __result = role.RoleCount;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Serialize")]
        public static bool SerializePrefix([HarmonyArgument(0)] MessageWriter writer, [HarmonyArgument(1)] RoleOptionsCollectionV09 options)
        {
            writer.WritePacked(options.roles.Count);
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<RoleTypes, RoleDataV09> keyValuePair in options.roles)
            {
                RoleDataV09 value = keyValuePair.Value;
                writer.Write((ushort)keyValuePair.Key);
                writer.Write((byte)value.Rate.MaxCount);
                writer.Write((byte)value.Rate.Chance);
                writer.StartMessage(0);
                IRoleOptions roleOptions = value.RoleOptions;
                if (roleOptions != null)
                {
                    roleOptions.Serialize(writer);
                }
                writer.EndMessage();
            }
            writer.WritePacked(CustomRoleManager.AllCustomRoles.Count);
            writer.StartMessage(0);
            for (int i = 0; i < CustomRoleManager.AllCustomRoles.Count; i++)
            {
                ICustomRole value = CustomRoleManager.AllCustomRoles[i];
                writer.WritePacked((int)value.Role);
                writer.WritePacked(value.RoleCount);
                writer.WritePacked(value.RoleChance);
                writer.WritePacked(value.Configuration.Configs.Count);
                int t = 0;
                while (t < value.Configuration.Configs.Count)
                {
                    ModdedOption config = value.Configuration.Configs[i];
                    writer.WriteConfig(config);
                    writer.Write(config.GetValue());
                    t++;
                }
            }
            writer.EndMessage();
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("Deserialize")]
        public static bool DeserializePrefix([HarmonyArgument(0)] MessageReader reader, ref RoleOptionsCollectionV09 __result)
        {
            RoleOptionsCollectionV09 roleOptionsCollectionV = new RoleOptionsCollectionV09();
            int num = reader.ReadPackedInt32();
            for (int i = 0; i < num; i++)
            {
                RoleTypes roleTypes = (RoleTypes)reader.ReadInt16();
                RoleRate roleRate = new RoleRate
                {
                    MaxCount = reader.ReadByte(),
                    Chance = reader.ReadByte()
                };
                RoleDataV09 roleDataV = new RoleDataV09(roleTypes);
                roleDataV.Rate = roleRate;
                MessageReader messageReader = reader.ReadMessage();
                IRoleOptions roleOptions = roleDataV.RoleOptions;
                if (roleOptions != null)
                {
                    roleOptions.Deserialize(messageReader);
                }
                roleOptionsCollectionV.roles[roleTypes] = roleDataV;
            }
            __result = roleOptionsCollectionV;
            int count = reader.ReadPackedInt32();
            MessageReader message = reader.ReadMessage();
            for (int i = 0; i < count; i++)
            {
                ICustomRole role = CustomRoleManager.GetRole((RoleTypes)message.ReadPackedInt32());
                role.Configuration.onlineCount = message.ReadPackedInt32();
                role.Configuration.onlineChance = message.ReadPackedInt32();
                int count2 = message.ReadPackedInt32();
                int t = 0;
                while (t < count)
                {
                    ModdedOption config = message.ReadConfig();
                    if (config != null)
                    {
                        config.SetValue(message.ReadString());
                    }
                    t++;
                }
            }
            return false;
        }
    }
}
