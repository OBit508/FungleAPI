using FungleAPI.Components;
using FungleAPI.Configuration.Presets;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FungleAPI.Configuration.Patches
{
    [HarmonyPatch(typeof(GamePresetsTab))]
    internal static class GamePresetsTabPatch
    {
        public static int Selected;
        public static List<PassiveButton> Buttons = new List<PassiveButton>();
        public static Transform Parent;
        public static void Update(GamePresetsTab menu)
        {
            if (Parent != null)
            {
                GameObject.Destroy(Parent.gameObject);
            }
            if (GameSettingMenuPatch.pluginChanger.CurrentPlugin == FungleAPIPlugin.Plugin)
            {
                menu.PresetDescriptionText.gameObject.SetActive(true);
                menu.DefaultButtonSelected.gameObject.SetActive(true);
                menu.SecondPresetButton.gameObject.SetActive(true);
            }
            else
            {
                Parent = new GameObject("CustomPresets").transform;
                Parent.SetParent(menu.transform);
                Parent.localPosition = Vector3.zero;
                Parent.localScale = Vector3.one;
                Selected = -1;
                Buttons.Clear();
                foreach (PassiveButton b in menu.PresetCancelButtons)
                {
                    b.SetNewAction(delegate
                    {
                        ControllerManager.Instance.CloseOverlayMenu(menu.ConfirmPresetPopUp.name);
                        menu.ConfirmPresetPopUp.Close();
                    });
                }
                menu.PresetDescriptionText.gameObject.SetActive(false);
                menu.DefaultButtonSelected.gameObject.SetActive(false);
                menu.SecondPresetButton.gameObject.SetActive(false);
                Transform divider = GameObject.Instantiate<Transform>(menu.transform.GetChild(3), Parent);
                divider.localPosition = new Vector3(1.4f, 0.11f, -1);
                divider.localScale = new Vector3(1.14f, 0.7f, 1);
                divider.rotation = new Quaternion(0, 0, 0.7071f, 0.7071f);
                PassiveButton load = GameObject.Instantiate<PassiveButton>(GameSettingMenu.Instance.ControllerSelectable[0].SafeCast<PassiveButton>(), Parent);
                load.transform.localPosition = new Vector3(2.9f, 1.76f, -2);
                load.transform.localScale = new Vector3(0.7f, 0.8f, 0.8f);
                load.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
                load.buttonText.text = FungleTranslation.LoadPresetText.GetString();
                load.SetNewAction(delegate
                {
                    if (Selected > -1)
                    {
                        menu.ConfirmPresetPopUp.gameObject.SetActive(true);
                        menu.PresetConfirmButton.SetNewAction(delegate
                        {
                            PresetV1 preset = GameSettingMenuPatch.pluginChanger.CurrentPlugin.PluginPreset.Presets[Selected];
                            preset.LoadConfigs();
                            foreach (PassiveButton button in Buttons)
                            {
                                button.SelectButton(false);
                            }
                            Selected = -1;
                            menu.GameOptionsMenu.RefreshChildren();
                            menu.GameOptionsMenu.RolesMenu.RefreshChildren();
                            ControllerManager.Instance.CloseOverlayMenu(menu.ConfirmPresetPopUp.name);
                            menu.ConfirmPresetPopUp.Close();
                            CustomRpcManager.Instance<RpcUpdatePreset>().Send(preset, PlayerControl.LocalPlayer.NetId);
                        });
                    }
                });
                PassiveButton edit = GameObject.Instantiate<PassiveButton>(GameSettingMenu.Instance.ControllerSelectable[0].SafeCast<PassiveButton>(), Parent);
                edit.transform.localPosition = new Vector3(2.9f, 1.2f, -2);
                edit.transform.localScale = new Vector3(0.7f, 0.8f, 0.8f);
                edit.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
                edit.buttonText.text = FungleTranslation.EditText.GetString();
                edit.SetNewAction(delegate
                {
                    if (Selected > -1)
                    {
                        PresetV1 ps = GameSettingMenuPatch.pluginChanger.CurrentPlugin.PluginPreset.Presets[Selected];
                        Utilities.Helpers.ShowEditNameScreen(FungleTranslation.PresetNameText.GetString(), ps.Empty ? "" : ps.PresetName, delegate (string text)
                        {
                            ps.PresetName = text;
                            Buttons[Selected].buttonText.text = ps.PresetName;
                            File.WriteAllText(ps.Path, JsonSerializer.Serialize(ps));
                        });
                    }
                });
                PassiveButton defaultButton = GameObject.Instantiate<PassiveButton>(menu.DefaultButtonSelected.SafeCast<PassiveButton>(), Parent);
                defaultButton.transform.localScale = Vector3.one * 0.7f;
                defaultButton.transform.localPosition = new Vector3(2.75f, -0.6f, 0);
                defaultButton.activeSprites.transform.GetChild(3).gameObject.SetActive(false);
                defaultButton.gameObject.SetActive(true);
                defaultButton.SetNewAction(delegate
                {
                    menu.ConfirmPresetPopUp.gameObject.SetActive(true);
                    menu.PresetConfirmButton.SetNewAction(delegate
                    {
                        PresetV1 preset = GameSettingMenuPatch.pluginChanger.CurrentPlugin.PluginPreset.GetDefault();
                        preset.LoadConfigs();
                        menu.GameOptionsMenu.RefreshChildren();
                        menu.GameOptionsMenu.RolesMenu.RefreshChildren();
                        ControllerManager.Instance.CloseOverlayMenu(menu.ConfirmPresetPopUp.name);
                        menu.ConfirmPresetPopUp.Close();
                        CustomRpcManager.Instance<RpcUpdatePreset>().Send(preset, PlayerControl.LocalPlayer.NetId);
                    });
                });
                Collider2D collider = load.GetComponent<Collider2D>();
                Collider2D collider2 = edit.GetComponent<Collider2D>();
                load.gameObject.AddComponent<Updater>().update = new Action(delegate
                {
                    collider.enabled = Selected > -1;
                    if (load.selected && !collider.enabled)
                    {
                        load.SelectButton(false);
                    }
                });
                edit.gameObject.AddComponent<Updater>().update = new Action(delegate
                {
                    collider2.enabled = Selected > -1;
                    if (edit.selected && !collider2.enabled)
                    {
                        edit.SelectButton(false);
                    }
                });
                if (GameSettingMenu.Instance != null)
                {
                    List<PresetV1> presets = GameSettingMenuPatch.pluginChanger.CurrentPlugin.PluginPreset.Presets;
                    for (int i = 0; i < 5; i++)
                    {
                        int id = i;
                        PresetV1 ps = presets[id];
                        PassiveButton preset = GameObject.Instantiate<PassiveButton>(GameSettingMenu.Instance.ControllerSelectable[0].SafeCast<PassiveButton>(), Parent);
                        preset.transform.localPosition = new Vector3(-1.35f, 1.7f - i * 0.75f, -2);
                        preset.transform.localScale = Vector3.one * 1.05f;
                        preset.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
                        preset.buttonText.text = ps.Empty ? FungleTranslation.EmptyText.GetString() : ps.PresetName;
                        preset.SetNewAction(delegate
                        {
                            foreach (PassiveButton button in Buttons)
                            {
                                button.SelectButton(false);
                            }
                            preset.SelectButton(!preset.selected);
                            if (preset.selected)
                            {
                                Selected = id;
                            }
                        });
                        float pos = 1.62f - i * 0.75f;
                        PassiveButton saveButton = FungleAssets.SaveButtonPrefab.Instantiate(Parent).GetComponent<PassiveButton>();
                        saveButton.transform.localPosition = new Vector3(0.95f, pos, -2);
                        saveButton.transform.localScale = Vector3.one * 0.7f;
                        saveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                        saveButton.OnClick.AddListener(new Action(delegate
                        {
                            Utilities.Helpers.ShowEditNameScreen(FungleTranslation.PresetNameText.GetString(), ps.Empty ? "" : ps.PresetName, delegate (string text)
                            {
                                ps.SaveConfigs(string.IsNullOrEmpty(text) ? "Preset " + (id + 1).ToString() : text);
                                preset.buttonText.text = ps.PresetName;
                                GameSettingMenu gameSettingMenu = GameSettingMenu.Instance;
                                if (gameSettingMenu != null)
                                {
                                    gameSettingMenu.GameSettingsTab.RefreshChildren();
                                    gameSettingMenu.GameSettingsTab.RolesMenu.RefreshChildren();
                                }
                            });
                        }));
                        PassiveButton destroyButton = FungleAssets.DestroyButtonPrefab.Instantiate(Parent).GetComponent<PassiveButton>();
                        destroyButton.transform.localPosition = new Vector3(0.23f, pos, -2);
                        destroyButton.transform.localScale = Vector3.one * 0.7f;
                        destroyButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                        destroyButton.OnClick.AddListener(new Action(delegate
                        {
                            ps.CleanConfigs();
                            if (Selected == id)
                            {
                                preset.SelectButton(false);
                                Selected = -1;
                            }
                            preset.buttonText.text = FungleTranslation.EmptyText.GetString();
                        }));
                        Collider2D saveCollider = saveButton.GetComponent<Collider2D>();
                        Collider2D destroyCollider = destroyButton.GetComponent<Collider2D>();
                        Collider2D presetCollider = preset.Colliders[0];
                        preset.gameObject.AddComponent<Updater>().update = delegate
                        {
                            saveButton.enabled = ps.Empty;
                            destroyCollider.enabled = !saveButton.enabled;
                            presetCollider.enabled = destroyCollider.enabled;
                        };
                        Buttons.Add(preset);
                    }
                }
            }
        }
    }
}
