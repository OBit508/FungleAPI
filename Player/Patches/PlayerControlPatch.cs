using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Chat;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.Modifiers;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FungleAPI.Player.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    internal static class PlayerControlPatch
    {
        internal static List<Il2CppSystem.Type> AllPlayerComponents = new List<Il2CppSystem.Type>();
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(PlayerControl __instance)
        {
            if (__instance.GetComponent<PlayerHelper>() == null)
            {
                DoStart(__instance);
            }
        }
        [HarmonyPatch("SetKillTimer")]
        [HarmonyPrefix]
        public static bool SetKillTimerPrefix(PlayerControl __instance, float time)
        {
            if (__instance.Data.Role.CanUseKillButton || __instance.AnyModifierForceKill())
            {
                float @float = RoleConfigManager.KillConfig.Cooldown();
                if (@float <= 0f)
                {
                    return false;
                }
                __instance.killTimer = Mathf.Clamp(time, 0f, @float);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, @float);
            }
            return false;
        }
        [HarmonyPatch("RpcMurderPlayer")]
        [HarmonyPrefix]
        public static bool RpcMurderPlayerPrefix(PlayerControl __instance, PlayerControl target, bool didSucceed)
        {
            __instance.RpcCustomMurderPlayer(target, didSucceed);
            return false;
        }
        [HarmonyPatch(nameof(PlayerControl.RpcSetRole))]
        [HarmonyPrefix]
        public static bool RpcSetRolePrefix(PlayerControl __instance, RoleTypes roleType)
        {
            __instance.RpcCustomSetRole(roleType, !__instance.roleAssigned && !TutorialManager.InstanceExists);
            return false;
        }
        [HarmonyPatch("CheckMurder")]
        [HarmonyPrefix]
        public static bool CheckMurderPrefix(PlayerControl __instance, PlayerControl target)
        {
            __instance.CheckCustomMurder(target);
            return false;
        }
        [HarmonyPatch("MurderPlayer")]
        [HarmonyPrefix]
        public static bool MurderPlayerPrefix(PlayerControl __instance, PlayerControl target, MurderResultFlags resultFlags)
        {
            __instance.CustomMurderPlayer(target, resultFlags);
            return false;
        }
        [HarmonyPatch("CompleteTask")]
        [HarmonyPostfix]
        public static void CompleteTaskPostfix(PlayerControl __instance, uint idx)
        {
            EventManager.CallEvent(new CompleteTaskEvent(__instance, idx));

            if (GameModeManager.GetCurrentGameMode() is HideNSeekMode hideNSeekMode)
            {
                NormalPlayerTask normalPlayerTask = ShipStatus.Instance.GetTaskById((byte)idx);
                if (normalPlayerTask != null)
                {
                    switch (normalPlayerTask.Length)
                    {
                        case NormalPlayerTask.TaskLength.None:
                        case NormalPlayerTask.TaskLength.Common:
                            hideNSeekMode.OnTaskComplete(hideNSeekMode.GetCommonTaskTimeValue());
                            break;
                        case NormalPlayerTask.TaskLength.Short:
                            hideNSeekMode.OnTaskComplete(hideNSeekMode.GetShortTaskTimeValue());
                            break;
                        case NormalPlayerTask.TaskLength.Long:
                            hideNSeekMode.OnTaskComplete(hideNSeekMode.GetLongTaskTimeValue());
                            break;
                    }
                    SoundManager.Instance.PlaySoundImmediate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.TaskFinishedSound, false, 1f, 1f, null);
                }
            }
        }
        [HarmonyPatch("Die")]
        [HarmonyPostfix]
        public static void DiePostfix(PlayerControl __instance, DeathReason reason)
        {
            EventManager.CallEvent(new PlayerDieEvent(__instance, reason));
        }
        [HarmonyPatch("ReportDeadBody")]
        [HarmonyPostfix]
        public static void ReportDeadBodyPostfix(PlayerControl __instance, NetworkedPlayerInfo target)
        {
            EventManager.CallEvent(new ReportBodyEvent(__instance, target));
        }
        [HarmonyPatch("AdjustLighting")]
        [HarmonyPrefix]
        public static bool AdjustLightingPrefix(PlayerControl __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            GameModeManager.GetCurrentGameMode().AdjustLighting(__instance);
            return false;
        }
        [HarmonyPatch("IsFlashlightEnabled")]
        [HarmonyPrefix]
        public static bool IsFlashlightEnabledPrefix(PlayerControl __instance, ref bool __result)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            __result = GameModeManager.GetCurrentGameMode().IsFlashlightEnabled(__instance);
            return false;
        }
        [HarmonyPatch(nameof(PlayerControl.Revive))]
        [HarmonyPostfix]
        public static void RevivePostfix(PlayerControl __instance)
        {
            if (__instance.AmOwner && !GameManager.Instance.IsHideAndSeek() && GameModeManager.GetCurrentGameMode().GetChatInGame())
            {
                HudManager.Instance.Chat.SetVisible(true);
            }
        }
        [HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
        [HarmonyPrefix]
        public static bool FixedUpdatePrefix(PlayerControl __instance)
        {
            if (!GameData.Instance)
            {
                return false;
            }
            NetworkedPlayerInfo data = __instance.Data;
            if (data == null || data.Role == null)
            {
                return false;
            }
            if (data.IsDead && PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.Data != null)
            {
                __instance.Visible = PlayerControl.LocalPlayer.Data.IsDead;
                __instance.cosmetics.SetPetVisible(true);
            }
            if (__instance.AmOwner)
            {
                if (ShipStatus.Instance && __instance.lightSource)
                {
                    float num = ShipStatus.Instance.CalculateLightRadius(data);
                    if (!Mathf.Approximately(num, __instance.lightSource.ViewDistance))
                    {
                        __instance.AdjustLighting();
                    }
                    __instance.lightSource.SetViewDistance(num);
                }
                PlayerControl playerControl = data.Role.FindClosestTarget();
                if (!((__instance.IsKillTimerEnabled || __instance.ForceKillTimerContinue) && data.Role.CanUseKillButton && !data.IsDead))
                {
                    __instance.Data.Role.SetPlayerTarget(playerControl);
                }
                if (__instance.CanMove && PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled)
                {
                    PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = false;
                }
                if (__instance.CanMove || __instance.inVent)
                {
                    __instance.newItemsInRange.Clear();
                    bool flag2 = (GameOptionsManager.Instance.CurrentGameOptions.GetBool(BoolOptionNames.GhostsDoTasks) || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && __instance.CanMove;
                    Vector2 truePosition = __instance.GetTruePosition();
                    int num2 = Physics2D.OverlapCircleNonAlloc(truePosition, __instance.MaxReportDistance, __instance.hitBuffer, Constants.Usables);
                    IUsable usable = null;
                    float num3 = float.MaxValue;
                    bool flag3 = false;
                    List<Vent> list = new List<Vent>();
                    for (int i = 0; i < num2; i++)
                    {
                        Collider2D collider2D = __instance.hitBuffer[i];
                        IUsable[] array;
                        if (!__instance.cache.TryGetValue(collider2D, out Il2CppReferenceArray<IUsable> cached))
                        {
                            array = __instance.cache[collider2D] = collider2D.GetComponents<IUsable>().ToArray();
                        }
                        else
                        {
                            array = cached.ToArray();
                        }
                        if (array != null && (flag2 || __instance.inVent))
                        {
                            foreach (IUsable usable2 in array)
                            {
                                bool flag4;
                                bool flag5;
                                float num4 = usable2.CanUse(data, out flag4, out flag5);
                                if (flag4 || flag5)
                                {
                                    __instance.newItemsInRange.Add(usable2);
                                }
                                if (flag4 && num4 < num3)
                                {
                                    if (usable2.Is(out Vent result))
                                    {
                                        list.Add(result);
                                    }
                                    else
                                    {
                                        num3 = num4;
                                        usable = usable2;
                                    }
                                }
                            }
                        }
                        if (flag2 && !data.IsDead && !flag3 && collider2D.tag == "DeadBody")
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component.enabled && !component.Reported && Vector2.Distance(truePosition, component.TruePosition) <= __instance.MaxReportDistance && !PhysicsHelpers.AnythingBetween(truePosition, component.TruePosition, Constants.ShipAndObjectsMask, false))
                            {
                                flag3 = true;
                            }
                        }
                    }
                    Vent vent = ((list.Count > 0) ? Enumerable.First<Vent>(list) : null);
                    for (int k = __instance.itemsInRange.Count - 1; k > -1; k--)
                    {
                        IUsable item = __instance.itemsInRange[k];
                        int num5 = __instance.newItemsInRange.FindIndex((IUsable j) => j == item);
                        if (num5 == -1)
                        {
                            item.SetOutline(false, false);
                            __instance.itemsInRange.RemoveAt(k);
                        }
                        else
                        {
                            __instance.newItemsInRange.RemoveAt(num5);
                            bool flag6;
                            if (item.Is(out Vent result))
                            {
                                flag6 = result == vent;
                            }
                            else
                            {
                                flag6 = usable == item;
                            }
                            item.SetOutline(true, flag6);
                        }
                    }
                    for (int l = 0; l < __instance.newItemsInRange.Count; l++)
                    {
                        IUsable usable3 = __instance.newItemsInRange[l];
                        bool flag7;
                        if (usable3.Is(out Vent result))
                        {
                            flag7 = result == vent;
                        }
                        else
                        {
                            flag7 = usable == usable3;
                        }
                        usable3.SetOutline(true, flag7);
                        __instance.itemsInRange.Add(usable3);
                    }
                    __instance.closest = usable;
                    DestroyableSingleton<HudManager>.Instance.ToggleUseAndPetButton(usable, flag2, __instance.CanPet());
                    DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(flag3);
                    DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.SetTarget(vent);
                    __instance.Data.Role.SetUsableTarget(vent.SafeCast<IUsable>());
                }
                else
                {
                    __instance.closest = null;
                    DestroyableSingleton<HudManager>.Instance.UseButton.SetTarget(null);
                    DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.SetTarget(Vent.currentVent);
                    DestroyableSingleton<HudManager>.Instance.PetButton.SetDisabled();
                    DestroyableSingleton<HudManager>.Instance.ReportButton.SetActive(false);
                    __instance.Data.Role.SetUsableTarget(Vent.currentVent.SafeCast<IUsable>());
                    if (PlayerCustomizationMenu.Instance)
                    {
                        DestroyableSingleton<HudManager>.Instance.UseButton.gameObject.SetActive(false);
                    }
                }
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Refresh();
                DestroyableSingleton<HudManager>.Instance.AdminButton.Refresh();
            }
            return false;
        }
        public static void DoStart(PlayerControl player)
        {
            foreach (Il2CppSystem.Type type in AllPlayerComponents)
            {
                player.gameObject.AddComponent(type).SafeCast<PlayerComponent>().player = player;
            }
        }
    }
}
