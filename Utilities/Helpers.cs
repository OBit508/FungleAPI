using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.MonoBehaviours;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Roles;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
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
    public static class Helpers
    {
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            CustomRpcManager.Instance<RpcCustomMurder>().Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer.NetId);
        }
        public static T GetPlayerComponent<T>(this PlayerControl player) where T : PlayerComponent
        {
            return player.GetComponent<T>();
        }
        public static PlayerControl GetClosest(this PlayerControl target)
        {
            PlayerControl closest = null;
            float dis = target.Data.Role.GetAbilityDistance();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                Vector3 center = target.Collider.bounds.center;
                Vector3 position = player.transform.position;
                float num = Vector2.Distance(center, position);
                if (player != target && !player.Data.IsDead && !PhysicsHelpers.AnythingBetween(target.Collider, center, position, Constants.ShipOnlyMask, false) && num < dis)
                {
                    closest = player;
                    dis = num;
                }
            }
            return closest;
        }
        public static void ClearAndDestroy<T>(this List<T> list) where T : UnityEngine.Object
        {
            foreach (T t in list)
            {
                UnityEngine.Object.Destroy(t);
            }
            list.Clear();
        }
        public static void ClearAndDestroy<T>(this Il2CppSystem.Collections.Generic.List<T> list) where T : UnityEngine.Object
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
        public static ModdedDeadBody GetBodyById(byte id)
        {
            foreach (ModdedDeadBody body in ModdedDeadBody.AllBodies)
            {
                if (body.ParentId == id)
                {
                    return body;
                }
            }
            return null;
        }
        public static ModdedDeadBody GetBody(this PlayerControl player)
        {
            foreach (ModdedDeadBody body in ModdedDeadBody.AllBodies)
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
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> list)
        {
            Il2CppSystem.Collections.Generic.List<T> values = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (T item in list)
            {
                values.Add(item);
            }
            return values;
        }
    }
}
