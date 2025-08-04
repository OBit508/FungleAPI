using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Rpc
{
    [HarmonyPatch(typeof(MessageReader))]
    internal static class MessageReaderPatch
    {
        [HarmonyPatch("ReadBoolean")]
        [HarmonyPrefix]
        public static bool ReadPrefix1(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadByte")]
        [HarmonyPrefix]
        public static bool ReadPrefix2(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadBytes")]
        [HarmonyPrefix]
        public static bool ReadPrefix3(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadBytesAndSize")]
        [HarmonyPrefix]
        public static bool ReadPrefix4(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadInt16")]
        [HarmonyPrefix]
        public static bool ReadPrefix5(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadInt32")]
        [HarmonyPrefix]
        public static bool ReadPrefix6(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadPackedInt32")]
        [HarmonyPrefix]
        public static bool ReadPrefix7(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadPackedUInt32")]
        [HarmonyPrefix]
        public static bool ReadPrefix8(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadSByte")]
        [HarmonyPrefix]
        public static bool ReadPrefix9(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadSingle")]
        [HarmonyPrefix]
        public static bool ReadPrefix10(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadString")]
        [HarmonyPrefix]
        public static bool ReadPrefix11(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadUInt16")]
        [HarmonyPrefix]
        public static bool ReadPrefix12(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadUInt32")]
        [HarmonyPrefix]
        public static bool ReadPrefix13(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
        [HarmonyPatch("ReadUInt64")]
        [HarmonyPrefix]
        public static bool ReadPrefix14(MessageReader __instance, ref object __result)
        {
            if (CustomRpcManager.CustomReaders.ContainsKey(__instance))
            {
                object obj = CustomRpcManager.CustomReaders[__instance][0];
                CustomRpcManager.CustomReaders[__instance].Remove(obj);
                __result = obj;
                return false;
            }
            return true;
        }
    }
}
