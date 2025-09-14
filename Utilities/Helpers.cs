using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
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
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Utilities
{
    public static class Helpers
    {
        private static List<DeadBody> allDeadBodies = new List<DeadBody>();
        public static List<DeadBody> AllDeadBodies
        {
            get
            {
                allDeadBodies.RemoveAll(body => body == null || body.IsDestroyedOrNull());
                return allDeadBodies;
            }
        }
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            CustomRpcManager.Instance<RpcCustomMurder>().Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer.NetId);
        }
        public static List<DeadBody> GetClosestsDeadBodies(this PlayerControl target, float distance, bool includeReporteds = false)
        {
            List<DeadBody> bodies = new List<DeadBody>();
            foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(target.GetTruePosition(), distance, Constants.PlayersOnlyMask))
            {
                if (collider2D.tag == "DeadBody")
                {
                    DeadBody component = collider2D.GetComponent<DeadBody>();
                    if (component && (!component.Reported || includeReporteds))
                    {
                        bodies.Add(component);
                    }
                }
            }
            return bodies;
        }
        public static DeadBody CreateCustomBody(PlayerControl from, DeadBodyType deadBodyType)
        {
            DeadBody body = GameObject.Instantiate<DeadBody>(GameManager.Instance.deadBodyPrefab[deadBodyType == DeadBodyType.Normal ? 0 : 1]);
            body.enabled = false;
            body.ParentId = from.PlayerId;
            body.bodyRenderers.ToList().ForEach(delegate (SpriteRenderer b)
            {
                from.SetPlayerMaterialColors(b);
            });
            from.SetPlayerMaterialColors(body.bloodSplatter);
            Vector3 vector = from.transform.position + from.KillAnimations[0].BodyOffset;
            vector.z = vector.y / 1000f;
            body.transform.position = vector;
            body.enabled = true;
            return body;
        }
        public static DeadBody GetClosestDeadBody(this PlayerControl target, float distance, bool includeReporteds = false)
        {
            DeadBody closest = null;
            float dis = distance;
            foreach (DeadBody body in GetClosestsDeadBodies(target, distance, includeReporteds))
            {
                float d = Vector2.Distance(target.GetTruePosition(), body.TruePosition);
                if (dis > d)
                {
                    dis = d;
                    closest = body;
                }
            }
            return closest;
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
        public static DeadBody GetBodyById(byte id)
        {
            foreach (DeadBody body in AllDeadBodies)
            {
                if (body.ParentId == id)
                {
                    return body;
                }
            }
            return null;
        }
        public static DeadBody GetBody(this PlayerControl player)
        {
            return GetBodyById(player.PlayerId);
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
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> list)
        {
            Il2CppSystem.Collections.Generic.List<T> values = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (T item in list)
            {
                values.Add(item);
            }
            return values;
        }
        public static List<T> ToSystemList<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            List<T> values = new List<T>();
            foreach (T item in list)
            {
                values.Add(item);
            }
            return values;
        }
        public static Vent CreateVent(Vector2 position, List<Vent> nearbyVents = null)
        {
            Vent vent = GameObject.Instantiate<Vent>(ShipStatus.Instance.AllVents[0], ShipStatus.Instance.transform);
            vent.Id = ShipStatus.Instance.AllVents.Count;
            ShipStatus.Instance.AllVents = ShipStatus.Instance.AllVents.Concat(new Vent[] { vent }).ToArray();
            vent.EnterVentAnim = null;
            vent.ExitVentAnim = null;
            vent.Right = null;
            vent.Center = null;
            vent.Left = null;
            vent.myRend.enabled = false;
            vent.transform.position = new Vector3(position.x, position.y, position.y / 1000 + 0.001f);
            if (nearbyVents != null)
            {
                vent.gameObject.AddComponent<VentHelper>().Vents = nearbyVents;
            }
            return vent;
        }
        public static void ConnectVent(this Vent vent, Vent target, bool connectBoth = true)
        {
            VentHelper helper = VentHelper.ShipVents[vent];
            if (!helper.Vents.Contains(target))
            {
                helper.Vents.Add(target);
            }
            VentHelper helper2 = VentHelper.ShipVents[target];
            if (connectBoth && !helper2.Vents.Contains(vent))
            {
                helper2.Vents.Add(vent);
            }
        }
        public static void DisconnectVent(this Vent vent, Vent target, bool disconnectBoth = true)
        {
            VentHelper helper = VentHelper.ShipVents[vent];
            if (helper.Vents.Contains(target))
            {
                helper.Vents.Remove(target);
            }
            VentHelper helper2 = VentHelper.ShipVents[target];
            if (disconnectBoth && helper2.Vents.Contains(vent))
            {
                helper2.Vents.Remove(vent);
            }
        }
    }
}
