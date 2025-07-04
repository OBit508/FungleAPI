using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using FungleAPI.Roles;
using HarmonyLib;
using PowerTools;
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
    public static class VentPatch
    {
        [HarmonyPatch("SetOutline")]
        [HarmonyPrefix]
        public static bool OnSetOutline(Vent __instance, [HarmonyArgument(0)] bool on, [HarmonyArgument(1)] bool mainTarget)
        {
            ICustomRole role = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (role != null)
            {
                if (on)
                {
                    __instance.myRend.material.SetFloat("_Outline", 1f);
                    __instance.myRend.material.SetColor("_OutlineColor", role.CachedConfiguration.OutlineColor);
                }
                else
                {
                    __instance.myRend.material.SetFloat("_Outline", 0f);
                }
                if (mainTarget)
                {
                    float num = Mathf.Clamp01(role.CachedConfiguration.OutlineColor.r * 0.5f);
                    float num2 = Mathf.Clamp01(role.CachedConfiguration.OutlineColor.g * 0.5f);
                    float num3 = Mathf.Clamp01(role.CachedConfiguration.OutlineColor.b * 0.5f);
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
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(Vent __instance)
        {
            List<Vent> list = new List<Vent>();
            if (__instance.Right != null)
            {
                list.Add(__instance.Right);
            }
            if (__instance.Center != null)
            {
                list.Add(__instance.Center);
            }
            if (__instance.Left != null)
            {
                list.Add(__instance.Left);
            }
            __instance.gameObject.AddComponent<CustomVent>().NearbyVents = list.ToArray();
        }
        public static void ConnectVent(this Vent v, Vent vent)
        {
            ButtonBehavior prefab = UnityEngine.Object.Instantiate(v.Buttons[0], v.Buttons[0].transform.parent);
            foreach (ButtonBehavior b in v.Buttons)
            {
                UnityEngine.Object.Destroy(b.gameObject);
            }
            float d = Vector2.Distance(v.Buttons[0].transform.position, v.transform.position);
            v.Buttons = new ButtonBehavior[] { };
            v.CleaningIndicators = new GameObject[] { };
            foreach (Vent v1 in v.NearbyVents)
            {
                CreateArrow(v, v1, prefab, d);
            }
            CreateArrow(v, vent, prefab, d);
            v.GetComponent<CustomVent>().NearbyVents = v.GetComponent<CustomVent>().NearbyVents.Add(vent);
            UnityEngine.Object.Destroy(prefab.gameObject);
            v.SetButtons(Vent.currentVent == v);
        }
        public static void UnconnectVent(this Vent v, Vent vent)
        {
            ButtonBehavior prefab = UnityEngine.Object.Instantiate(v.Buttons[0], v.Buttons[0].transform.parent);
            foreach (ButtonBehavior b in v.Buttons)
            {
                UnityEngine.Object.Destroy(b.gameObject);
            }
            float d = Vector2.Distance(v.Buttons[0].transform.position, v.transform.position);
            v.Buttons = new ButtonBehavior[] { };
            v.CleaningIndicators = new GameObject[] { };
            v.GetComponent<CustomVent>().NearbyVents = v.GetComponent<CustomVent>().NearbyVents.Remove(vent);
            foreach (Vent v1 in v.NearbyVents)
            {
                CreateArrow(v, v1, prefab, d);
            }
            UnityEngine.Object.Destroy(prefab.gameObject);
            v.SetButtons(Vent.currentVent == v);
        }
        [HarmonyPatch("CanUse")]
        [HarmonyPostfix]
        public static void OnCanUse(Vent __instance, NetworkedPlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result)
        {
            if (!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Data.RoleType != AmongUs.GameOptions.RoleTypes.Engineer)
            {
                PlayerControl @object = pc.Object;
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                float num = Vector2.Distance(center, position);
                canUse = num <= __instance.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false) && PlayerControl.LocalPlayer.Data.Role.CanVent;
                couldUse = PlayerControl.LocalPlayer.Data.Role.CanVent;
                __result = num;
            }
        }
        [HarmonyPatch("NearbyVents", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool OnGetVents(Vent __instance, ref Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Vent> __result)
        {
            __result = __instance.GetNearbyVents();
            return false;
        }
        public static Vent CreateVent()
        {
            int id = 0;
            while (ShipStatus.Instance.AllVents.Any((Vent v) => v.Id == id))
            {
                int id2 = id;
                id = id2 + 1;
            }
            Vent v = ShipStatus.Instance.AllVents[0];
            Vent vent2 = GameObject.Instantiate<Vent>(v, v.transform.parent);
            vent2.Id = id;
            vent2.Left = null;
            vent2.Center = null;
            vent2.Right = null;
            vent2.gameObject.SetActive(true);
            List<Vent> list = ShipStatus.Instance.AllVents.ToList<Vent>();
            list.Add(vent2);
            ShipStatus.Instance.AllVents = list.ToArray();
            return vent2;
        }
        public static Vent[] GetNearbyVents(this Vent vent)
        {
            if (vent.GetComponent<CustomVent>() != null)
            {
                return vent.GetComponent<CustomVent>().NearbyVents;
            }
            return new Vent[] { };
        }
        internal static void Organize(Transform button, Transform vent, Transform target, float d)
        {
            button.position = vent.position + (target.position - vent.position).normalized * d;
            Vector2 vec = (target.position - button.position).normalized;
            button.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg);
            Vector3 vec2 = button.localPosition;
            vec2.z = -3;
            button.localPosition = vec2;
        }
        internal static ButtonBehavior CreateArrow(this Vent v, Vent vent, ButtonBehavior prefab, float distance)
        {
            ButtonBehavior button = UnityEngine.Object.Instantiate(prefab, prefab.transform.parent);
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnClick.AddListener(new Action(delegate
            {
                string error = "F";
                v.TryMoveToVent(vent, out error);
            }));
            List<ButtonBehavior> l = v.Buttons.ToList();
            List<GameObject> l2 = v.CleaningIndicators.ToList();
            l.Add(button);
            l2.Add(button.transform.GetChild(0).gameObject);
            v.Buttons = l.ToArray();
            v.CleaningIndicators = l2.ToArray();
            Organize(button.transform, v.transform, vent.transform, distance);
            return button;
        }
    }
}
