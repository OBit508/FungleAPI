using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using Steamworks;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Utilities
{
    public static class Helpers
    {
        internal static GenericPopup Popup;
        internal static EditName Screen;
        internal static Dictionary<Vent, (List<Vent>, bool)> Connecteds = new Dictionary<Vent, (List<Vent>, bool)>();
        private static List<DeadBody> allDeadBodies = new List<DeadBody>();
        public static List<DeadBody> AllDeadBodies
        {
            get
            {
                allDeadBodies.RemoveAll(body => body == null || body.IsDestroyedOrNull());
                return allDeadBodies;
            }
        }
        public static void ShowPopup(string text)
        {
            if (Popup == null)
            {
                Popup = GameObject.Instantiate<GenericPopup>(DestroyableSingleton<DiscordManager>.Instance.discordPopup, Camera.main.transform);
                Popup.transform.localPosition = new Vector3(0, 0, -500);
                SpriteRenderer background = Popup.transform.Find("Background").GetComponent<SpriteRenderer>();
                Vector2 size = background.size;
                size.x *= 2.5f;
                background.size = size;
                Popup.TextAreaTMP.fontSizeMin = 2f;
            }
            Popup.Show(text);
        }
        public static void ShowEditNameScreen(string tittleText, string defaultText, Action<string> OnSubmit = null, Action<string> OnBack = null)
        {
            if (Screen == null)
            {
                Screen = GameObject.Instantiate<EditName>(AccountManager.Instance.accountTab.editNameScreen, Camera.main.transform);
                Screen.transform.localPosition = new Vector3(0, 0, -500);
                Screen.gameObject.layer = 5;
                foreach (Transform tr in Screen.GetComponentsInChildren<Transform>())
                {
                    tr.gameObject.layer = 5;
                }
                Screen.nameText.name = "ModdedEditNameTab";
                Screen.nameText.nameSource.characterLimit = 30;
                Screen.transform.GetChild(0).GetComponent<BoxCollider2D>().isTrigger = true;
                Screen.transform.GetChild(3).gameObject.SetActive(false);
                Screen.transform.GetChild(7).gameObject.SetActive(false);
            }
            TextMeshPro tittle = Screen.transform.GetChild(2).GetComponent<TextMeshPro>();
            tittle.text = tittleText;
            tittle.GetComponent<TextTranslatorTMP>().enabled = false;
            Screen.transform.GetChild(6).GetComponent<PassiveButton>().SetNewAction(delegate
            {
                Screen.Close();
                OnSubmit?.Invoke(Screen.nameText.nameSource.text);
            });
            Screen.BackButton.SafeCast<PassiveButton>().SetNewAction(delegate
            {
                Screen.Close();
                OnBack?.Invoke(Screen.nameText.nameSource.text);
            });
            Screen.nameText.nameSource.SetText(defaultText);
            Screen.gameObject.SetActive(true);
            Screen.StartCoroutine(Screen.Show());
        }
        public static ModdedConsole CreateConsole(float distance, Predicate<PlayerControl> predicate, Action onUse, Sprite sprite)
        {
            ModdedConsole console = new GameObject("CustomConsole").AddComponent<ModdedConsole>();
            console.gameObject.layer = 12;
            console.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
            console.Image = console.gameObject.AddComponent<SpriteRenderer>();
            console.gameObject.AddComponent<PassiveButton>().SetNewAction(console.Use);
            console.MinigamePrefab = RoleManager.Instance.AllRoles.ToSystemList().FirstOrDefault(obj => obj.Role == AmongUs.GameOptions.RoleTypes.Scientist).SafeCast<ScientistRole>().VitalsPrefab;
            console.Image.material = new Material(Shader.Find("Sprites/Outline"));
            console.Predicate = predicate;
            console.OnUse = onUse;
            console.Image.sprite = sprite;
            console.usableDistance = distance;
            console.transform.localScale = Vector3.one;
            console.transform.position = Vector3.zero;
            return console;
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
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T result = obj.GetComponent<T>();
            if (result == null)
            {
                result = obj.AddComponent<T>();
            }
            return result;
        }
        public static string GetString(this StringNames s)
        {
            return TranslationController.Instance.GetString(s);
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
        public static Color Dark(this Color color, float num = 0.5f)
        {
            num = Mathf.Clamp01(num);
            return new Color(
                color.r * num,
                color.g * num,
                color.b * num,
                color.a
            );
        }
        public static Color Light(this Color color, float num = 0.5f)
        {
            return new Color(
                Mathf.Clamp01(color.r * num),
                Mathf.Clamp01(color.g * num),
                Mathf.Clamp01(color.b * num),
                color.a
                );
        }
        public static T DontDestroy<T>(this T obj) where T : UnityEngine.Object
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
        public static Vent CreateVent(VentType type, Vector2 position, List<Vent> nearbyVents = null, bool connectBoth = true)
        {
            Vent vent = GameObject.Instantiate<Vent>(type == VentType.Skeld ? PrefabUtils.SkeldPrefab.AllVents[0] : type == VentType.Polus ? PrefabUtils.PolusPrefab.AllVents[0] : PrefabUtils.FunglePrefab.AllVents[0], ShipStatus.Instance.transform);
            vent.gameObject.SetActive(true);
            vent.Id = ShipStatus.Instance.AllVents.Count;
            ShipStatus.Instance.AllVents = ShipStatus.Instance.AllVents.Concat(new Vent[] { vent }).ToArray();
            vent.Right = null;
            vent.Center = null;
            vent.Left = null;
            vent.transform.position = new Vector3(position.x, position.y, position.y / 1000 + 0.001f);
            if (nearbyVents != null)
            {
                Connecteds.Add(vent, (nearbyVents, connectBoth));
            }
            return vent;
        }
        public static void ConnectVent(this Vent vent, Vent target, bool connectBoth = true)
        {
            VentHelper helper = vent.TryGetHelper();
            if (!helper.Vents.Contains(target))
            {
                helper.Vents.Add(target);
            }
            VentHelper helper2 = target.TryGetHelper();
            if (connectBoth && !helper2.Vents.Contains(vent))
            {
                helper2.Vents.Add(vent);
            }
        }
        public static void DisconnectVent(this Vent vent, Vent target, bool disconnectBoth = true)
        {
            VentHelper helper = vent.TryGetHelper();
            if (helper.Vents.Contains(target))
            {
                helper.Vents.Remove(target);
            }
            VentHelper helper2 = target.TryGetHelper();
            if (disconnectBoth && helper2.Vents.Contains(vent))
            {
                helper2.Vents.Remove(vent);
            }
        }
        public static VentHelper TryGetHelper(this Vent target)
        {
            try
            {
                return VentHelper.ShipVents[target];
            }
            catch
            {
                VentHelper ventHelper = target.GetComponent<VentHelper>();
                if (ventHelper == null)
                {
                    VentPatch.DoStart(target);
                    ventHelper = target.GetComponent<VentHelper>();
                }
                return ventHelper;
            }
        }
        [Comment("MiraAPI method")]
        public static MethodBase GetStateMachineMoveNext<T>(string methodName)
        {
            string typeName = typeof(T).FullName;
            Type showRoleStateMachine = typeof(T).GetNestedTypes().FirstOrDefault((Type x) => x.Name.Contains(methodName));
            bool flag;
            if (showRoleStateMachine == null)
            {
                BepInExErrorLogInterpolatedStringHandler bepInExErrorLogInterpolatedStringHandler = new BepInExErrorLogInterpolatedStringHandler(34, 2, out flag);
                if (flag)
                {
                    bepInExErrorLogInterpolatedStringHandler.AppendLiteral("Failed to find ");
                    bepInExErrorLogInterpolatedStringHandler.AppendFormatted<string>(methodName);
                    bepInExErrorLogInterpolatedStringHandler.AppendLiteral(" state machine for ");
                    bepInExErrorLogInterpolatedStringHandler.AppendFormatted<string>(typeName);
                }
                FungleAPIPlugin.Instance.Log.LogError(bepInExErrorLogInterpolatedStringHandler);
                return null;
            }
            MethodInfo moveNext = AccessTools.Method(showRoleStateMachine, "MoveNext", null, null);
            if (moveNext == null)
            {
                BepInExErrorLogInterpolatedStringHandler bepInExErrorLogInterpolatedStringHandler = new BepInExErrorLogInterpolatedStringHandler(36, 2, out flag);
                if (flag)
                {
                    bepInExErrorLogInterpolatedStringHandler.AppendLiteral("Failed to find MoveNext method for ");
                    bepInExErrorLogInterpolatedStringHandler.AppendFormatted<string>(typeName);
                    bepInExErrorLogInterpolatedStringHandler.AppendLiteral(".");
                    bepInExErrorLogInterpolatedStringHandler.AppendFormatted<string>(methodName);
                }
                FungleAPIPlugin.Instance.Log.LogError(bepInExErrorLogInterpolatedStringHandler);
                return null;
            }
            BepInExInfoLogInterpolatedStringHandler bepInExInfoLogInterpolatedStringHandler = new BepInExInfoLogInterpolatedStringHandler(15, 1, out flag);
            if (flag)
            {
                bepInExInfoLogInterpolatedStringHandler.AppendLiteral("Found ");
                bepInExInfoLogInterpolatedStringHandler.AppendFormatted<string>(methodName);
                bepInExInfoLogInterpolatedStringHandler.AppendLiteral(".MoveNext");
            }
            FungleAPIPlugin.Instance.Log.LogInfo(bepInExInfoLogInterpolatedStringHandler);
            return moveNext;
        }
        public enum VentType 
        {
            Skeld,
            Polus,
            Fungle
        }
    }
}
