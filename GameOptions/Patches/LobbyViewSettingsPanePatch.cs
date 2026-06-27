using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GModes;
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

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(LobbyViewSettingsPane))]
    internal static class LobbyViewSettingsPanePatch
    {
        public static List<LobbyTab> Tabs = new List<LobbyTab>();
        public static LobbyTab Tab;

        public static Scroller scroller;

        public static PluginChanger pluginChanger;

        public static Action OnChangeGamemode;

        [HarmonyPatch(nameof(LobbyViewSettingsPane.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(LobbyViewSettingsPane __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return;

            __instance.gameModeText.gameObject.SetActive(false);

            OnChangeGamemode = delegate { try { __instance.gameModeText.text = GameModeManager.GetCurrentGameMode().GameModeName.GetString(); } catch { } };

            pluginChanger = GameObject.Instantiate(FungleAssets.PluginChangerPrefab, __instance.rolesTabButton.transform.parent);
            pluginChanger.transform.localPosition = new Vector3(-4.2586f, 2.4241f, -1.9999f);
            pluginChanger.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            pluginChanger.Plugins = pluginChanger.Plugins.FindAll(p => p.LobbyTabs.Count > 0);

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