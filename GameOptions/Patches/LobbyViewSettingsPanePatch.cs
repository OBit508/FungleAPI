using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameModes;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using xCloud;
using static Il2CppMono.Security.X509.X520;
using static UnityEngine.GraphicsBuffer;
using Sentry.Internal.Extensions;
using System.Reflection;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    internal static class LobbyViewSettingsPanePatch
    {
        public static List<LobbyTab> Tabs = new List<LobbyTab>();
        public static LobbyTab Tab;

        public static float min;
        public static Transform Inner;
        public static BoxCollider2D scroller;

        public static bool Dragging;
        public static float StartMouseX;
        public static float StartContentX;

        public static PluginChanger pluginChanger;

        [HarmonyPatch(nameof(LobbyViewSettingsPane.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(LobbyViewSettingsPane __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return;

            __instance.gameModeText.gameObject.SetActive(false);

            pluginChanger = GameObject.Instantiate(FungleAssets.PluginChangerPrefab, __instance.rolesTabButton.transform.parent);
            pluginChanger.transform.localPosition = new Vector3(-4.2586f, 2.4241f, -1.9999f);
            pluginChanger.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            pluginChanger.Plugins = OptionManager.GetAllAssembliesWithTabs();

            UiElement buttonPrefab = GameObject.Instantiate(__instance.ControllerSelectable[3], __instance.transform);

            buttonPrefab.gameObject.SetActive(false);

            pluginChanger.OnChange = new Action<Assembly>(delegate (Assembly plugin)
            {
                foreach (UiElement uiElement in __instance.ControllerSelectable)
                {
                    uiElement?.gameObject.Destroy();
                }
                __instance.ControllerSelectable.Clear();

                Tabs = OptionManager.LobbyTabs[plugin];

                Tab = null;
                if (Tabs.Count > 0)
                {
                    Tab = Tabs.First();
                }

                foreach (LobbyTab lobbyTab in Tabs)
                {
                    lobbyTab.ViewSettingsButton = CreateButton(__instance, buttonPrefab, lobbyTab.ViewTabButtonText, delegate
                    {
                        Tab = lobbyTab;
                        __instance.ChangeTab(StringNames.None);
                    });
                    lobbyTab.RefreshViewTab = __instance.RefreshTab;
                }

                float num = 0;

                for (int i = 0; i < __instance.ControllerSelectable.Count; i++)
                {
                    UiElement uiElement = __instance.ControllerSelectable[i];
                    uiElement.transform.localPosition = new Vector3(-4.871f + (3.471f * i), 1.404f, 0);
                    num += 1.1f;
                }

                min = -num;
                scroller.transform.localPosition = new Vector3(-0.7f, 1.35f, 0);
            });

            GameObject gameObject = new GameObject("Hitbox")
            {
                layer = 5,
                transform =
                    {
                        parent = __instance.transform,
                        localScale = new Vector3(1.14f, 0.07f, 1),
                        localPosition = new Vector3(-0.7f, 1.35f, 0f)
                    }
            };

            Inner = new GameObject()
            {
                name = "Inner",
                transform =
                    {
                        parent = buttonPrefab.transform.parent,
                        localPosition = Vector3.zero,
                        localScale = Vector3.one
                    }
            }.transform;

            SpriteMask spriteMask = gameObject.AddComponent<SpriteMask>();
            spriteMask.sprite = FungleAssets.Empty;
            spriteMask.alphaCutoff = 0f;

            scroller = gameObject.AddComponent<BoxCollider2D>();

            gameObject.AddComponent<Updater>().update = () =>
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 mouseLocal = Inner.parent.InverseTransformPoint(mouseWorld);

                bool mouseHover = scroller.OverlapPoint(mouseWorld);
                __instance.scrollBar.enabled = !Dragging;

                if (mouseHover && Input.GetMouseButtonDown(0))
                {
                    Dragging = true;

                    StartMouseX = mouseLocal.x;
                    StartContentX = Inner.localPosition.x;
                }

                if (Dragging && Input.GetMouseButton(0))
                {
                    float delta = mouseLocal.x - StartMouseX;

                    Vector3 pos = Inner.localPosition;
                    pos.x = Mathf.Clamp(StartContentX + delta, min, 0f);

                    Inner.localPosition = pos;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Dragging = false;
                }
            };

            pluginChanger.OnChange(FungleApiPlugin.Plugin.ModAssembly);
        }
        [HarmonyPatch(nameof(LobbyViewSettingsPane.ChangeTab))]
        [HarmonyPrefix]
        public static bool Change(LobbyViewSettingsPane __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            __instance.RefreshTab();
            __instance.scrollBar.ScrollToTop();
            return false;
        }
        [HarmonyPatch(nameof(LobbyViewSettingsPane.RefreshTab))]
        [HarmonyPrefix]
        public static bool Refresh(LobbyViewSettingsPane __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            foreach (GameObject gameObject in __instance.settingsInfo)
            {
                gameObject?.Destroy();
            }
            __instance.settingsInfo.Clear();
            foreach (LobbyTab lobbyTab in Tabs)
            {
                lobbyTab.ViewSettingsButton?.SelectButton(false);
                lobbyTab.ViewSettingsButton.buttonText.text = lobbyTab.ViewTabButtonText;
            }
            Tab.ViewSettingsButton?.SelectButton(true);
            Tab.BuildViewTab(__instance);
            return false;
        }
        public static PassiveButton CreateButton(LobbyViewSettingsPane lobbyViewSettingsPane, UiElement prefab, string name, Action onClick)
        {
            PassiveButton passiveButton = GameObject.Instantiate<PassiveButton>(prefab.SafeCast<PassiveButton>(), Inner);
            passiveButton.gameObject.SetActive(true);
            passiveButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
            passiveButton.buttonText.text = name;
            passiveButton.ClickMask = scroller;

            foreach (SpriteRenderer spriteRenderer in passiveButton.GetComponentsInChildren<SpriteRenderer>(true))
            {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            foreach (TextMeshPro textMeshPro in passiveButton.GetComponentsInChildren<TextMeshPro>(true))
            {
                textMeshPro.fontMaterial.SetFloat("_Stencil", 1f);
                textMeshPro.fontMaterial.SetFloat("_StencilComp", 4f);
            }

            foreach (Behaviour behaviour in passiveButton.GetComponents<Behaviour>())
            {
                behaviour.enabled = true;
            }

            passiveButton.SetNewAction(onClick);

            lobbyViewSettingsPane.ControllerSelectable.Add(passiveButton);
            return passiveButton;
        }
    }
}