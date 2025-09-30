using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using PowerTools;
using Rewired.Utils.Classes.Data;
using Sentry.Unity.NativeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ICustomRole role = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (role != null)
            {
                if (on)
                {
                    __instance.myRend.material.SetFloat("_Outline", 1f);
                    __instance.myRend.material.SetColor("_OutlineColor", role.Configuration.OutlineColor);
                }
                else
                {
                    __instance.myRend.material.SetFloat("_Outline", 0f);
                }
                if (mainTarget)
                {
                    float num = Mathf.Clamp01(role.Configuration.OutlineColor.r * 0.5f);
                    float num2 = Mathf.Clamp01(role.Configuration.OutlineColor.g * 0.5f);
                    float num3 = Mathf.Clamp01(role.Configuration.OutlineColor.b * 0.5f);
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
                __result = __instance.TryGetHelper().Vents.ToArray();
            }
            catch
            {
                __result = new Vent[0];
            }
            return false;
        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPostfix(Vent __instance)
        {
            if (Helpers.VentPrefab == null)
            {
                Helpers.VentPrefab = GameObject.Instantiate<Vent>(__instance);
                Helpers.VentPrefab.gameObject.SetActive(false);
            }
            foreach (Il2CppSystem.Type type in AllVentComponents)
            {
                if (__instance.gameObject.GetComponent(type) == null)
                {
                    __instance.gameObject.AddComponent(type).SafeCast<VentComponent>().vent = __instance;
                }
            }
            ButtonBehavior buttonPrefab = GameObject.Instantiate<ButtonBehavior>(__instance.Buttons[0], __instance.transform);
            VentHelper helper = __instance.GetComponent<VentHelper>();
            VentHelper.ShipVents.Add(__instance, helper);
            helper.ArrowPrefab = buttonPrefab;
            if (__instance.Right != null)
            {
                helper.Vents.Add(__instance.Right);
            }
            if (__instance.Center != null)
            {
                helper.Vents.Add(__instance.Center);
            }
            if (__instance.Left != null)
            {
                helper.Vents.Add(__instance.Left);
            }
            (List<Vent>, bool) connected;
            if (Helpers.Connecteds.TryGetValue(__instance, out connected))
            {
                helper.Vents = connected.Item1;
                if (connected.Item2)
                {
                    foreach (Vent vent in connected.Item1)
                    {
                        vent.ConnectVent(__instance, false);
                    }
                }
                Helpers.Connecteds.Remove(__instance);
            }
        }
        [HarmonyPatch("SetButtons")]
        [HarmonyPrefix]
        public static bool SetButtonsPrefix(Vent __instance, [HarmonyArgument(0)] bool enabled)
        {
            Vent[] nearbyVents = __instance.NearbyVents;
            Vector2 vector;
            if (nearbyVents.Count() > 0)
            {
                Vector3 positions = Vector3.zero;
                foreach (Vent vent in nearbyVents)
                {
                    positions += vent.transform.position;
                }
                vector = positions / 2f - __instance.transform.position;
            }
            else
            {
                vector = Vector2.zero;
            }
            for (int i = 0; i < __instance.Buttons.Length; i++)
            {
                ButtonBehavior buttonBehavior = __instance.Buttons[i];
                if (enabled)
                {
                    Vent vent = nearbyVents[i];
                    if (vent != null)
                    {
                        VentilationSystem ventilationSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].SafeCast<VentilationSystem>();
                        bool flag = ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(vent.Id);
                        buttonBehavior.gameObject.SetActive(true);
                        __instance.ToggleNeighborVentBeingCleaned(flag, buttonBehavior, __instance.CleaningIndicators[i]);
                        Vector3 vector2 = vent.transform.position - __instance.transform.position;
                        Vector3 vector3 = vector2.normalized * (0.7f + __instance.spreadShift);
                        vector3.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                        vector3.y -= 0.08f;
                        vector3.z = -10f;
                        buttonBehavior.transform.localPosition = vector3;
                        buttonBehavior.transform.LookAt2d(vent.transform);
                        vector3 = vector3.RotateZ((vector.AngleSigned(vector2) > 0f) ? __instance.spreadAmount : (-__instance.spreadAmount));
                        buttonBehavior.transform.localPosition = vector3;
                        buttonBehavior.transform.Rotate(0f, 0f, (vector.AngleSigned(vector2) > 0f) ? __instance.spreadAmount : (-__instance.spreadAmount));
                    }
                    else
                    {
                        buttonBehavior.gameObject.SetActive(enabled);
                    }
                }
                else
                {
                    buttonBehavior.gameObject.SetActive(false);
                }
            }
            return false;
        }
    }
}
