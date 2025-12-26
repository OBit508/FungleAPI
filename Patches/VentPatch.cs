using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Types;
using FungleAPI.Role;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using PowerTools;
using Rewired.Utils.Classes.Data;
using Sentry.Unity.NativeUtils;
using UnityEngine;
using static Rewired.UI.ControlMapper.ControlMapper;
using static UnityEngine.UIElements.MouseCaptureDispatchingStrategy;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(Vent))]
    internal static class VentPatch
    {
        public static List<Il2CppSystem.Type> AllVentComponents = new List<Il2CppSystem.Type>();
        [HarmonyPatch("SetOutline")]
        [HarmonyPrefix]
        public static bool SetOutlinePrefix(Vent __instance, [HarmonyArgument(0)] bool on, [HarmonyArgument(1)] bool mainTarget)
        {
            if (!CustomRoleManager.CurrentVentConfig.ShowOutline())
            {
                if (!on && !mainTarget)
                {
                    return true;
                }
                __instance.SetOutline(false, false);
                return false;
            }
            ICustomRole role = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (role != null)
            {
                if (on)
                {
                    __instance.myRend.material.SetFloat("_Outline", 1f);
                    __instance.myRend.material.SetColor("_OutlineColor", role.OutlineColor);
                }
                else
                {
                    __instance.myRend.material.SetFloat("_Outline", 0f);
                }
                if (mainTarget)
                {
                    float num = Mathf.Clamp01(role.OutlineColor.r * 0.5f);
                    float num2 = Mathf.Clamp01(role.OutlineColor.g * 0.5f);
                    float num3 = Mathf.Clamp01(role.OutlineColor.b * 0.5f);
                    __instance.myRend.material.SetColor("_AddColor", new Color(num, num2, num3, 1f));
                }
                else
                {
                    __instance.myRend.material.SetColor("_AddColor", new Color(0f, 0f, 0f, 0f));
                }
                return false;
            }
            return true;
        }
        [HarmonyPatch("CanUse")]
        [HarmonyPrefix]
        public static bool CanUsePrefix(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;
            couldUse = pc.Role.CanVent() && GameManager.Instance.LogicUsables.CanUse(__instance.SafeCast<IUsable>(), @object) && pc.Role.CanUse(__instance.SafeCast<IUsable>()) && (!@object.MustCleanVent(__instance.Id) || (@object.inVent && Vent.currentVent == __instance)) && !pc.IsDead && (@object.CanMove || @object.inVent);
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Ventilation, out systemType))
            {
                VentilationSystem ventilationSystem = systemType.SafeCast<VentilationSystem>();
                if (ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(__instance.Id))
                {
                    couldUse = false;
                }
            }
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance(center, position);
                canUse &= num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
            }
            __result = num;
            return false;
        }
        [HarmonyPatch("NearbyVents", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool NearbyVentsPrefix(Vent __instance, ref Il2CppReferenceArray<Vent> __result)
        {
            try
            {
                __result = (Vent[])__instance.TryGetHelper().Vents.ToArray();
            }
            catch
            {
                __result = new Vent[0];
            }
            return false;
        }
        [HarmonyPatch("EnterVent")]
        [HarmonyPostfix]
        public static void EnterVentPostfix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
        {
            pc.GetComponent<PlayerHelper>().__CurrentVent = __instance;
            List<PlayerControl> players = VentHelper.ShipVents[__instance].Players;
            if (!players.Contains(pc))
            {
                players.Add(pc);
            }
            EventManager.CallEvent(new OnEnterVent() { Vent = __instance, Player = pc });
        }
        [HarmonyPatch("ExitVent")]
        [HarmonyPostfix]
        public static void ExitVentPostfix(Vent __instance, [HarmonyArgument(0)] PlayerControl pc)
        {
            pc.GetComponent<PlayerHelper>().__CurrentVent = null;
            List<PlayerControl> players = VentHelper.ShipVents[__instance].Players;
            if (players.Contains(pc))
            {
                players.Remove(pc);
            }
            EventManager.CallEvent(new OnExitVent() { Vent = __instance, Player = pc });
        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(Vent __instance)
        {
            if (__instance.GetComponent<VentHelper>() == null)
            {
                DoStart(__instance);
            }
        }
        [HarmonyPatch("SetButtons")]
        [HarmonyPrefix]
        public static bool SetButtonsPrefix(Vent __instance, [HarmonyArgument(0)] bool enabled)
        {
            if (enabled)
            {
                List<(Vent vent, ButtonBehavior button, GameObject clean)> entries = new List<(Vent vent, ButtonBehavior button, GameObject clean)>();
                for (int i = 0; i < __instance.Buttons.Length; i++)
                {
                    Vent v = __instance.NearbyVents[i];
                    if (v)
                    {
                        entries.Add((v, __instance.Buttons[i], __instance.CleaningIndicators[i]));
                    }
                }
                VentilationSystem ventilationSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
                for (int i = 0; i < entries.Count; i++)
                {
                    (Vent vent, ButtonBehavior button, GameObject clean) e = entries[i];
                    ButtonBehavior buttonBehavior = e.button;
                    buttonBehavior.gameObject.SetActive(true);
                    __instance.ToggleNeighborVentBeingCleaned(ventilationSystem.IsVentCurrentlyBeingCleaned(e.vent.Id), e.button, e.clean);
                    Vector3 vector2 = (e.vent.transform.position - __instance.transform.position).normalized * (0.7f + __instance.spreadShift);
                    vector2.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                    vector2.y -= 0.08f;
                    vector2.z = -10f;
                    int offsetIndex = i - (entries.Count / 2);
                    if (entries.Count % 2 == 0)
                    {
                        offsetIndex += i < (entries.Count / 2) ? 0 : 1;
                    }
                    vector2 = vector2.RotateZ(offsetIndex * __instance.spreadAmount);
                    buttonBehavior.transform.localPosition = vector2;
                    buttonBehavior.transform.LookAt2d(e.vent.transform);
                    buttonBehavior.transform.Rotate(0f, 0f, offsetIndex * __instance.spreadAmount);
                }
            }
            else
            {
                for (int i = 0; i < __instance.Buttons.Count; i++)
                {
                    __instance.Buttons[i].gameObject.SetActive(false);
                }
            }
            return false;
        }
        public static void DoStart(Vent vent)
        {
            foreach (Il2CppSystem.Type type in AllVentComponents)
            {
                vent.gameObject.AddComponent(type).SafeCast<VentComponent>().vent = vent;
            }
            VentHelper helper = vent.GetComponent<VentHelper>();
            if (VentHelper.ArrowPrefab == null)
            {
                VentHelper.ArrowPrefab = PrefabUtils.SkeldPrefab.AllVents[0].Buttons[0];
            }
            VentHelper.ShipVents.Add(vent, helper);
            (List<Vent>, bool) connected;
            if (Helpers.Connecteds.TryGetValue(vent, out connected))
            {
                helper.Vents = connected.Item1;
                if (connected.Item2)
                {
                    foreach (Vent v in connected.Item1)
                    {
                        v.ConnectVent(v, false);
                    }
                }
                Helpers.Connecteds.Remove(vent);
            }
            else
            {
                helper.Vents = new List<Vent>();
            }
            if (vent.Right != null)
            {
                helper.Vents.Add(vent.Right);
            }
            if (vent.Center != null)
            {
                helper.Vents.Add(vent.Center);
            }
            if (vent.Left != null)
            {
                helper.Vents.Add(vent.Left);
            }
        }
    }
}
