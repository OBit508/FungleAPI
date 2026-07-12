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
using FungleAPI.ModCompatibility;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    internal static class LobbyViewSettingsPanePatch
    {
        public static List<LobbyTab> Tabs = new List<LobbyTab>();
        public static LobbyTab Tab;

        public static Scroller scroller;

        public static PluginChanger pluginChanger;
        public static bool FungleViewActive;

        [HarmonyPatch(nameof(LobbyViewSettingsPane.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(LobbyViewSettingsPane __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return;

            if (MiraCompatibility.IsLoaded)
            {
                AwakeWithMira(__instance);
                return;
            }

            __instance.gameModeText.gameObject.SetActive(false);

            pluginChanger = GameObject.Instantiate(FungleAssets.PluginChangerPrefab, __instance.rolesTabButton.transform.parent);
            pluginChanger.transform.localPosition = new Vector3(-4.2586f, 2.4241f, -1.9999f);
            pluginChanger.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            pluginChanger.Plugins = pluginChanger.Plugins.FindAll(p => p.LobbyTabs.FindAll(t => t.GetType() != typeof(GamemodeSettingsTab)).Count > 0);

            UiElement buttonPrefab = GameObject.Instantiate(__instance.ControllerSelectable[3], __instance.transform);

            buttonPrefab.gameObject.SetActive(false);

            pluginChanger.OnChange = new Action<ModPlugin>(delegate (ModPlugin plugin)
            {
                foreach (UiElement uiElement in __instance.ControllerSelectable)
                {
                    uiElement?.gameObject.Destroy();
                }
                __instance.ControllerSelectable.Clear();

                Tabs = plugin.LobbyTabs;

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

                scroller.ContentXBounds.min = -num;
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

            scroller = gameObject.AddComponent<Scroller>();
            scroller.allowX = true;
            scroller.allowY = false;
            scroller.ContentXBounds.max = 0;
            scroller.Inner = new GameObject()
            {
                name = "Inner",
                transform =
                    {
                        parent = buttonPrefab.transform.parent,
                        localPosition = Vector3.zero,
                        localScale = Vector3.one
                    }
            }.transform;

            ManualScrollHelper manualScrollHelper = gameObject.AddComponent<ManualScrollHelper>();
            manualScrollHelper.scroller = scroller;
            manualScrollHelper.verticalAxis = RewiredConstsEnum.Action.TaskLHorizontal;
            manualScrollHelper.scrollSpeed = 10f;

            SpriteMask spriteMask = gameObject.AddComponent<SpriteMask>();
            spriteMask.sprite = FungleAssets.Empty;
            spriteMask.alphaCutoff = 0f;

            scroller.ClickMask = gameObject.AddComponent<BoxCollider2D>();

            gameObject.AddComponent<Updater>().update = () =>
            {
                scroller.enabled = scroller.ClickMask.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                __instance.scrollBar.enabled = !scroller.enabled;
            };

            pluginChanger.OnChange(FungleApiPlugin.Plugin);
        }
        private static void AwakeWithMira(LobbyViewSettingsPane pane)
        {
            FungleViewActive = false;
            pluginChanger = GameObject.Instantiate(FungleAssets.PluginChangerPrefab, pane.rolesTabButton.transform.parent);
            pluginChanger.transform.localPosition = new Vector3(-4.2586f, 2.4241f, -1.9999f);
            pluginChanger.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            pluginChanger.Plugins = pluginChanger.Plugins.FindAll(p => p.LobbyTabs.Any(t => t.GetType() != typeof(GamemodeSettingsTab)));
            UiElement prefab = GameObject.Instantiate(pane.ControllerSelectable[3], pane.transform);
            prefab.gameObject.SetActive(false);
            pluginChanger.OnChange = plugin =>
            {
                foreach (UiElement element in pane.ControllerSelectable.ToArray())
                {
                    if (element != null && element.name.StartsWith("FungleView:"))
                    {
                        pane.ControllerSelectable.Remove(element);
                        element.gameObject.Destroy();
                    }
                }
                Tabs = plugin.LobbyTabs.Where(t => t.GetType() != typeof(GamemodeSettingsTab)).ToList();
                Tab = Tabs.FirstOrDefault();
                int index = 0;
                foreach (LobbyTab lobbyTab in Tabs)
                {
                    PassiveButton button = GameObject.Instantiate<PassiveButton>(prefab.SafeCast<PassiveButton>(), prefab.transform.parent);
                    button.name = $"FungleView:{index}";
                    button.gameObject.SetActive(true);
                    button.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
                    button.buttonText.text = lobbyTab.ViewTabButtonText;
                    button.transform.localPosition = new Vector3(-4.871f + (1.5f * index), 0.9f, 0f);
                    button.SetNewAction(() =>
                    {
                        Tab = lobbyTab;
                        FungleViewActive = true;
                        pane.ChangeTab(StringNames.None);
                    });
                    pane.ControllerSelectable.Add(button);
                    lobbyTab.ViewSettingsButton = button;
                    lobbyTab.RefreshViewTab = pane.RefreshTab;
                    index++;
                }
            };
            pluginChanger.OnChange(FungleApiPlugin.Plugin);
        }
        [HarmonyPatch(nameof(LobbyViewSettingsPane.ChangeTab))]
        [HarmonyPrefix]
        public static bool Change(LobbyViewSettingsPane __instance, StringNames tab)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;
            if (MiraCompatibility.IsLoaded && (!FungleViewActive || tab != StringNames.None))
            {
                FungleViewActive = false;
                return true;
            }

            __instance.RefreshTab();
            __instance.scrollBar.ScrollToTop();
            return false;
        }
        [HarmonyPatch(nameof(LobbyViewSettingsPane.RefreshTab))]
        [HarmonyPrefix]
        public static bool Refresh(LobbyViewSettingsPane __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;
            if (MiraCompatibility.IsLoaded && !FungleViewActive) return true;

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
            PassiveButton passiveButton = GameObject.Instantiate<PassiveButton>(prefab.SafeCast<PassiveButton>(), scroller.Inner);
            passiveButton.gameObject.SetActive(true);
            passiveButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
            passiveButton.buttonText.text = name;
            passiveButton.ClickMask = scroller.ClickMask;

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
