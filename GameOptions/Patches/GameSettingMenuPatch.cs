using FungleAPI.Components;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Rewired.Demos.GamepadTemplateUI.GamepadTemplateUI;

namespace FungleAPI.GameOptions.Patches
{
    [HarmonyPatch(typeof(GameSettingMenu))]
    internal static class GameSettingMenuPatch
    {
        public static Scroller scroller;
        public static PluginChanger pluginChanger;

        public static LobbyTab CurrentTab;

        [HarmonyPatch(nameof(GameSettingMenu.Start))]
        [HarmonyPostfix]
        public static void StartPostfix(GameSettingMenu __instance)
        {
            RolesSettingMenuPatch.chanceTabPlugin = null;

            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.HideNSeek && GameOptionsManager.Instance.CurrentGameOptions.GameMode != AmongUs.GameOptions.GameModes.SeekFools)
            {
                UiElement presetTab = __instance.ControllerSelectable[0];

                foreach (SpriteRenderer spriteRenderer in presetTab.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }

                foreach (TextMeshPro textMeshPro in presetTab.GetComponentsInChildren<TextMeshPro>(true))
                {
                    textMeshPro.fontMaterial.SetFloat("_Stencil", 1f);
                    textMeshPro.fontMaterial.SetFloat("_StencilComp", 4f);
                }

                pluginChanger = FungleAssets.PluginChangerPrefab.Instantiate(__instance.ControllerSelectable[0].transform.parent).GetComponent<PluginChanger>();
                pluginChanger.transform.localPosition = new Vector3(-3.36f, 1.67f, -2);
                pluginChanger.OnChange = new Action<ModPlugin>(delegate (ModPlugin plugin)
                {
                    foreach (UiElement uiElement in __instance.ControllerSelectable)
                    {
                        if (uiElement == presetTab) continue;
                        uiElement.Destroy();
                    }
                    __instance.ControllerSelectable.Clear();
                    __instance.ControllerSelectable.Add(presetTab);


                    for (int i = 0; i < plugin.LobbyTabs.Count; i++)
                    {
                        int id = i + 1;
                        LobbyTab lobbyTab = plugin.LobbyTabs[i];
                        lobbyTab.EditSettingsButton = CreateButton(__instance, presetTab, lobbyTab.EditTabButtonText, delegate
                        {
                            __instance.ChangeTab(id, false);
                        });
                    }

                    RolesSettingMenuPatch.chanceTabPlugin = null;
                    __instance.GameSettingsTab.RefreshChildren();

                    float num = 0;
                    for (int i = 0; i < __instance.ControllerSelectable.Count; i++)
                    {
                        UiElement uiElement = __instance.ControllerSelectable[i];
                        uiElement.transform.localPosition = new Vector3(-2.96f, -0.02f - (0.6f * i), -2);
                        num += 0.243f;
                    }
                    scroller.ContentYBounds = new FloatRange(0, num);
                });
                __instance.transform.GetChild(2).gameObject.SetActive(false);
                __instance.MenuDescriptionText.transform.parent.localPosition = new Vector3(0, 0.3f, -1);

                Transform parent = new GameObject()
                {
                    name = "Inner",
                    transform =
                    {
                        parent = __instance.ControllerSelectable[0].transform.parent,
                        localPosition = Vector3.zero
                    }
                }.transform;

                GameObject gameObject = new GameObject("Hitbox")
                {
                    layer = 5,
                    transform =
                    {
                        parent = __instance.transform,
                        localScale = new Vector3(0.28f, 0.235f, 1),
                        localPosition = new Vector3(-3.355f, -1.03f, 0f)
                    }
                };

                scroller = gameObject.AddComponent<Scroller>();
                scroller.allowX = false;
                scroller.allowY = true;
                scroller.Inner = parent;

                ManualScrollHelper manualScrollHelper = gameObject.AddComponent<ManualScrollHelper>();
                manualScrollHelper.scroller = scroller;
                manualScrollHelper.verticalAxis = RewiredConstsEnum.Action.TaskRVertical;
                manualScrollHelper.scrollSpeed = 10f;

                SpriteMask spriteMask = gameObject.AddComponent<SpriteMask>();
                spriteMask.sprite = FungleAssets.Empty;
                spriteMask.alphaCutoff = 0f;

                scroller.ClickMask = gameObject.AddComponent<BoxCollider2D>();

                gameObject.AddComponent<Updater>().update = () =>
                {
                    scroller.enabled = scroller.ClickMask.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    __instance.GameSettingsTab.scrollBar.enabled = !scroller.enabled;
                    __instance.RoleSettingsTab.scrollBar.enabled = !scroller.enabled;
                };

                presetTab.transform.parent = scroller.Inner;

                pluginChanger.OnChange(FungleAPIPlugin.Plugin);
            }
        }

        public static PassiveButton CreateButton(GameSettingMenu gameSettingMenu, UiElement prefab, string name, Action onClick)
        {
            PassiveButton passiveButton = GameObject.Instantiate<PassiveButton>(prefab.SafeCast<PassiveButton>(), scroller.Inner);
            passiveButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
            passiveButton.buttonText.text = name;
            passiveButton.SetNewAction(onClick);
            gameSettingMenu.ControllerSelectable.Add(passiveButton);
            return passiveButton;
        }
        [HarmonyPatch(nameof(GameSettingMenu.ChangeTab))]
        [HarmonyPrefix]
        public static bool ChangeTabPrefix(GameSettingMenu __instance, int tabNum, bool previewOnly)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek || GameOptionsManager.Instance.CurrentGameOptions.GameMode == AmongUs.GameOptions.GameModes.SeekFools) return true;

            __instance.PresetsTab.gameObject.SetActive(false);
            __instance.GameSettingsTab.gameObject.SetActive(false);
            __instance.RoleSettingsTab.gameObject.SetActive(false);

            foreach (UiElement uiElement in __instance.ControllerSelectable)
            {
                PassiveButton passiveButton = uiElement.SafeCast<PassiveButton>();
                if (passiveButton != null)
                {
                    passiveButton.SelectButton(false);
                }
            }

            if (tabNum == 0)
            {
                __instance.PresetsTab.gameObject.SetActive(true);
                __instance.PresetsTab.OpenMenu();
                __instance.GamePresetsButton.SelectButton(true);
                __instance.MenuDescriptionText.text = StringNames.GamePresetsDescription.GetString();
            }
            else
            {
                CurrentTab = pluginChanger.CurrentPlugin.LobbyTabs[tabNum - 1];

                CurrentTab.EditSettingsButton.SelectButton(true);

                if (CurrentTab is RoleTab)
                {
                    __instance.RoleSettingsTab.gameObject.SetActive(true);
                    __instance.RoleSettingsTab.OpenMenu(false);
                    __instance.MenuDescriptionText.text = CurrentTab.TabDescriptionText;
                    __instance.RoleSettingsTab.OpenMenu(true);
                }
                else
                {
                    __instance.GameSettingsTab.Initialize();
                    __instance.GameSettingsTab.RefreshChildren();
                    __instance.GameSettingsTab.gameObject.SetActive(true);
                    __instance.MenuDescriptionText.text = CurrentTab.TabDescriptionText;
                }
            }

            __instance.ToggleLeftSideDarkener(true);
            __instance.ToggleRightSideDarkener(false);

            return false;
        }
    }
}