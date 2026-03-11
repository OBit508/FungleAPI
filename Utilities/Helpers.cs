using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Unity.IL2CPP.Utils;
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
    /// <summary>
    /// A utility class
    /// </summary>
    public static class Helpers
    {
        internal static GenericPopup Popup;
        internal static EditName Screen;
        /// <summary>
        /// Show a popup on the screen
        /// </summary>
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
        /// <summary>
        /// Show the edit name screen
        /// </summary>
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
        /// <summary>
        /// Add DontUnloadUnusedAsset flag
        /// </summary>
        public static T DontUnload<T>(this T obj) where T : UnityEngine.Object
        {
            obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return obj;
        }
        /// <summary>
        /// Cast something
        /// </summary>
        public static T SimpleCast<T>(this object obj)
        {
            return (T)obj;
        }
        /// <summary>
        /// Get or Add a component to a GameObject
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T result = obj.GetComponent<T>();
            if (result == null)
            {
                result = obj.AddComponent<T>();
            }
            return result;
        }
        /// <summary>
        /// Get the translated string
        /// </summary>
        public static string GetString(this StringNames s)
        {
            return TranslationController.Instance.GetString(s);
        }
        /// <summary>
        /// Set a new action to this button
        /// </summary>
        public static void SetNewAction(this PassiveButton button, Action action)
        {
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnClick.AddListener(new Action(delegate
            {
                action();
            }));
        }
        /// <summary>
        /// Set a new action to this button
        /// </summary>
        public static void SetNewAction(this ButtonBehavior button, Action action)
        {
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnClick.AddListener(new Action(delegate
            {
                action();
            }));
        }
        /// <summary>
        /// Get the index of something inside a 
        /// </summary>
        public static int GetIndex<T>(this IEnumerable<T> values, T thing)
        {
            T[] list = values.ToArray();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].Equals(thing))
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// Make a color be more darker
        /// </summary>
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
        /// <summary>
        /// Make a color be more lighter
        /// </summary>
        public static Color Light(this Color color, float num = 0.5f)
        {
            return new Color(
                Mathf.Clamp01(color.r * num),
                Mathf.Clamp01(color.g * num),
                Mathf.Clamp01(color.b * num),
                color.a
                );
        }
        /// <summary>
        /// Add HideAndDontSave flag
        /// </summary>
        public static T DontDestroy<T>(this T obj) where T : UnityEngine.Object
        {
            obj.hideFlags |= HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(obj);
            return obj;
        }
        /// <summary>
        /// Cast something without errors
        /// </summary>
        public static T SafeCast<T>(this Il2CppObjectBase obj) where T : Il2CppObjectBase
        {
            try
            {
                if (obj == null || obj.TryCast<T>() == null)
                {
                    return null;
                }
                return obj.Cast<T>();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Start a coroutine
        /// </summary>
        public static Coroutine StartCoroutine(System.Collections.IEnumerator enumerator)
        {
            return FungleAPIPlugin.Helper.StartCoroutine(enumerator.WrapToIl2Cpp());
        }
        /// <summary>
        /// Start a coroutine
        /// </summary>
        public static Coroutine StartCoroutine(Il2CppSystem.Collections.IEnumerator enumerator)
        {
            return FungleAPIPlugin.Helper.StartCoroutine(enumerator);
        }
        /// <summary>
        /// Stop a coroutine
        /// </summary>
        public static void StopCoroutine(Coroutine coroutine)
        {
            FungleAPIPlugin.Helper.StopCoroutine(coroutine);
        }
    }
}
