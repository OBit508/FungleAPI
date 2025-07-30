using FungleAPI.MonoBehaviours;
using FungleAPI.Roles;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities
{
    public static class Utils
    {
        public static void ClearAndDestroy<T>(this List<T> list) where T : UnityEngine.Object
        {
            foreach (T t in list)
            {
                UnityEngine.Object.Destroy(t);
            }
            list.Clear();
        }
        public static T DontUnload<T>(this T obj) where T : UnityEngine.Object
        {
            ref T ptr = ref obj;
            ptr.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return obj;
        }
        public static T SimpleCast<T>(this object obj)
        {
            return (T)obj;
        }
        public static PlayerControl GetPlayerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }
            return null;
        }
        public static CustomDeadBody GetBodyById(byte id)
        {
            foreach (CustomDeadBody body in CustomDeadBody.AllBodies)
            {
                if (body.ParentId == id)
                {
                    return body;
                }
            }
            return null;
        }
        public static CustomDeadBody GetBody(this PlayerControl player)
        {
            foreach (CustomDeadBody body in CustomDeadBody.AllBodies)
            {
                if (body.Owner == player)
                {
                    return body;
                }
            }
            return null;
        }
        public static string GetString(this StringNames s)
        {
            return TranslationController.Instance.GetString(s);
        }
        public static PlayerVoteArea GetVoteArea(this PlayerControl player)
        {
            if (MeetingHud.Instance)
            {
                foreach (PlayerVoteArea playerVoteArea in MeetingHud.Instance.playerStates)
                {
                    if (playerVoteArea.TargetPlayerId == player.PlayerId)
                    {
                        return playerVoteArea;
                    }
                }
            }
            return null;
        }
        public static List<ChatBubble> GetChatBubble(this PlayerControl player)
        {
            List<ChatBubble> list = new List<ChatBubble>();
            foreach (ChatBubble chatBubble in HudManager.Instance.Chat.chatBubblePool.activeChildren)
            {
                if (chatBubble.playerInfo.Object.PlayerId == player.PlayerId)
                {
                    list.Add(chatBubble);
                }
            }
            return list;
        }
        public static void SetNewAction(this PassiveButton button, Action action)
        {
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnClick.AddListener(new Action(delegate
            {
                action();
            }));
        }
        public static void SetNewAction(this ButtonBehavior button, Action action)
        {
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnClick.AddListener(new Action(delegate
            {
                action();
            }));
        }
        public static int GetIndex<T>(this T[] list, T thing)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Equals(thing))
                {
                    return i;
                }
            }
            return -1;
        }
        public static Color Dark(Color color, float num = 0.5f)
        {
            num = Mathf.Clamp01(num);
            return new Color(
                color.r * num,
                color.g * num,
                color.b * num,
                color.a
            );
        }
        public static Color Light(Color color, float num = 0.5f)
        {
            return new Color(
                Mathf.Clamp01(color.r * num),
                Mathf.Clamp01(color.g * num),
                Mathf.Clamp01(color.b * num),
                color.a
                );
        }
        public static T DontDestroy<T>(this T obj) where T : Component
        {
            obj.hideFlags |= HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(obj);
            return obj;
        }
        public static T SafeCast<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
        {
            if (obj == null || obj.TryCast<T>() == null)
            {
                return null;
            }
            return obj.Cast<T>();
        }
        public static void InvokeMethod(this object obj, string methodName, Type[] types, object[] arguments)
        {
            MethodInfo method = obj.GetType().GetMethod(methodName, types);
            try
            {
                if (method != null)
                {
                    method.Invoke(obj, arguments);
                }
            }
            catch
            {
            }
        }
        public static object InvokeMethod(this object obj, MethodInfo method, object[] arguments)
        {
            try
            {
                if (method != null)
                {
                    return method.Invoke(obj, arguments);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public static void PatchAllDerivedMethods(this Harmony harmony, Type baseType, MethodInfo prefix = null, MethodInfo postfix = null)
        {
            foreach (Type type in baseType.Assembly.GetTypes())
            {
                if (type == baseType || !baseType.IsAssignableFrom(type))
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        if (method.IsSpecialName)
                        {
                            ParameterInfo[] parameters = method.GetParameters();
                            if (parameters.Length >= 2 &&
                                parameters[0].ParameterType == typeof(byte) &&
                                parameters[1].ParameterType == typeof(Hazel.MessageReader))
                            {
                                HarmonyMethod prefixAttr = prefix != null ? new HarmonyMethod(prefix) : null;
                                HarmonyMethod postfixAttr = postfix != null ? new HarmonyMethod(postfix) : null;
                                harmony.Patch(method, prefix: prefixAttr, postfix: postfixAttr);
                            }
                        }
                    }
                }
            }
        }
    }
}
