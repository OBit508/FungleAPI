using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Freeplay.Helpers;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Il2CppSystem.Threading.Tasks;
using MS.Internal.Xml.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using xCloud;

namespace FungleAPI.Freeplay
{
    [HarmonyPatch(typeof(TaskAdderGame))]
    internal static class TaskAdderGamePatch
    {
        public static Scroller scroller;
        public static Dictionary<TaskFolder, List<FolderItem>> Items = new Dictionary<TaskFolder, List<FolderItem>>();
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        public static bool BeginPrefix(TaskAdderGame __instance, [HarmonyArgument(0)] PlayerTask t)
        {
            Items.Clear();
            __instance.RootFolderPrefab.Text.fontMaterial.SetFloat("_Stencil", 1f);
            __instance.RootFolderPrefab.Text.fontMaterial.SetFloat("_StencilComp", 4f);
            __instance.RootFolderPrefab.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            __instance.TaskPrefab.Text.fontMaterial.SetFloat("_Stencil", 1f);
            __instance.TaskPrefab.Text.fontMaterial.SetFloat("_StencilComp", 4f);
            __instance.TaskPrefab.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            Minigame.Instance = __instance;
            __instance.MyTask = t;
            __instance.MyNormTask = t.SafeCast<NormalPlayerTask>();
            __instance.timeOpened = Time.realtimeSinceStartup;
            if (PlayerControl.LocalPlayer)
            {
                if (MapBehaviour.Instance)
                {
                    MapBehaviour.Instance.Close();
                }
                PlayerControl.LocalPlayer.MyPhysics.SetNormalizedVelocity(Vector2.zero);
            }
            __instance.logger.Info("Opening minigame " + __instance.GetType().Name, null);
            __instance.StartCoroutine(__instance.CoAnimateOpen());
            DestroyableSingleton<DebugAnalytics>.Instance.Analytics.MinigameOpened(PlayerControl.LocalPlayer.Data, __instance.TaskType);
            __instance.Root = UnityEngine.Object.Instantiate(__instance.RootFolderPrefab, __instance.transform);
            __instance.Root.gameObject.SetActive(false);
            Il2CppSystem.Collections.Generic.Dictionary<string, TaskFolder> dictionary = new Il2CppSystem.Collections.Generic.Dictionary<string, TaskFolder>();
            __instance.PopulateRoot(TaskAdderGame.FolderType.Tasks, __instance.Root, dictionary, ShipStatus.Instance.CommonTasks);
            __instance.PopulateRoot(TaskAdderGame.FolderType.Tasks, __instance.Root, dictionary, ShipStatus.Instance.LongTasks);
            __instance.PopulateRoot(TaskAdderGame.FolderType.Tasks, __instance.Root, dictionary, ShipStatus.Instance.ShortTasks);
            __instance.Root.SubFolders = __instance.Root.SubFolders.ToSystemList().OrderBy((f) => f.FolderName).ToList().ToIl2CppList();
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                ModFolderConfig folderConfig = plugin.FolderConfig;
                folderConfig.Initialize(plugin);

                TaskFolder folder = UnityEngine.Object.Instantiate(__instance.RootFolderPrefab, __instance.transform);
                folder.gameObject.SetActive(false);
                folder.FolderName = folderConfig.FolderName;
                folder.SetFolderColor(folderConfig.FolderColor);
                Items[folder] = folderConfig.Items;
                
                void PopulateFolder(Folder folder, TaskFolder parent)
                {
                    TaskFolder taskFolder = UnityEngine.Object.Instantiate(__instance.RootFolderPrefab, __instance.transform);
                    taskFolder.gameObject.SetActive(false);
                    taskFolder.FolderName = folder.FolderName;
                    taskFolder.SetFolderColor(folder.FolderColor);
                    Items[taskFolder] = folder.Items;
                    parent.SubFolders.Add(taskFolder);
                    foreach (Folder sub in folder.SubFolders)
                    {
                        PopulateFolder(sub, taskFolder);
                    }
                }
                foreach (Folder sub in folderConfig.SubFolders)
                {
                    PopulateFolder(sub, folder);
                }

                __instance.Root.SubFolders.Add(folder);
            }
            scroller = __instance.gameObject.AddComponent<Scroller>();
            scroller.allowX = false;
            scroller.allowY = true;
            scroller.Inner = __instance.TaskParent;
            ManualScrollHelper manualScrollHelper = __instance.gameObject.AddComponent<ManualScrollHelper>();
            manualScrollHelper.scroller = scroller;
            manualScrollHelper.verticalAxis = RewiredConstsEnum.Action.TaskRVertical;
            manualScrollHelper.scrollSpeed = 10f;
            GameObject gameObject = new GameObject("Hitbox");
            gameObject.layer = 5;
            gameObject.transform.SetParent(__instance.transform, false);
            gameObject.transform.localScale = new Vector3(0.54f, 0.43f, 1f);
            gameObject.transform.localPosition = new Vector3(0f, -0.25f, 0f);
            SpriteMask spriteMask = gameObject.AddComponent<SpriteMask>();
            spriteMask.sprite = FungleAssets.Empty;
            spriteMask.alphaCutoff = 0f;
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
            collider.enabled = true;
            scroller.ClickMask = collider;
            __instance.GoToRoot();
            return false;
        }
        [HarmonyPatch("ShowFolder")]
        [HarmonyPrefix]
        public static bool ShowFolderPrefix(TaskAdderGame __instance, [HarmonyArgument(0)] TaskFolder taskFolder)
        {
            StringBuilder stringBuilder = new StringBuilder(64);
            __instance.Hierarchy.Add(taskFolder);
            for (int i = 0; i < __instance.Hierarchy.Count; i++)
            {
                stringBuilder.Append(__instance.Hierarchy[i].FolderName);
                stringBuilder.Append("\\");
            }
            __instance.PathText.text = stringBuilder.ToString();
            for (int j = 0; j < __instance.ActiveItems.Count; j++)
            {
                UnityEngine.Object.Destroy(__instance.ActiveItems[j].gameObject);
            }
            __instance.ActiveItems.Clear();
            float num = 0f;
            float num2 = 1.7f;
            float num3 = 0f;
            for (int k = 0; k < taskFolder.SubFolders.Count; k++)
            {
                TaskFolder folder = taskFolder.SubFolders[k];
                TaskFolder taskFolder2 = UnityEngine.Object.Instantiate(folder, __instance.TaskParent);
                taskFolder2.gameObject.SetActive(true);
                taskFolder2.Parent = __instance;
                taskFolder2.transform.localPosition = new Vector3(num, num2, 0f);
                taskFolder2.transform.localScale = Vector3.one;
                taskFolder2.Button.SetNewAction(delegate
                {
                    __instance.ShowFolder(folder);
                });
                num3 = Mathf.Max(num3, taskFolder2.Text.bounds.size.y + 1.1f);
                num += __instance.folderWidth;
                if (num > __instance.lineWidth)
                {
                    num = 0f;
                    num2 -= num3;
                    num3 = 0f;
                }
                __instance.ActiveItems.Add(taskFolder2.transform);
                if (taskFolder2 != null && taskFolder2.Button != null)
                {
                    ControllerManager.Instance.AddSelectableUiElement(taskFolder2.Button, false);
                }
            }
            bool flag = false;
            List<PlayerTask> list = taskFolder.TaskChildren.ToArray().ToList().OrderBy((PlayerTask t) => t.TaskType).ToList();
            for (int l = 0; l < list.Count; l++)
            {
                TaskAddButton taskAddButton = UnityEngine.Object.Instantiate(__instance.TaskPrefab);
                taskAddButton.MyTask = list[l];
                if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower)
                {
                    SystemTypes targetSystem = taskAddButton.MyTask.SafeCast<DivertPowerTask>().TargetSystem;
                    taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.DivertPowerTo, new Il2CppSystem.Object[] { TranslationController.Instance.GetString(targetSystem) });
                }
                else if (taskAddButton.MyTask.TaskType == TaskTypes.FixWeatherNode)
                {
                    int nodeId = taskAddButton.MyTask.SafeCast<WeatherNodeTask>().NodeId;
                    taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixWeatherNode) + " " + TranslationController.Instance.GetString(WeatherSwitchGame.ControlNames[nodeId]);
                }
                else
                {
                    taskAddButton.Text.text = TranslationController.Instance.GetString(taskAddButton.MyTask.TaskType);
                }
                __instance.AddFileAsChild(taskFolder, taskAddButton, ref num, ref num2, ref num3);
                if (taskAddButton != null && taskAddButton.Button != null)
                {
                    ControllerManager.Instance.AddSelectableUiElement(taskAddButton.Button, false);
                    if (__instance.Hierarchy.Count != 1 && !flag)
                    {
                        TaskFolder component = ControllerManager.Instance.CurrentUiState.CurrentSelection.GetComponent<TaskFolder>();
                        if (component != null)
                        {
                            __instance.previouslySelectedFolderName = component.FolderName;
                        }
                        ControllerManager.Instance.SetDefaultSelection(taskAddButton.Button, null);
                        flag = true;
                    }
                }
            }
            if (__instance.Hierarchy.Count > 1 && Items.TryGetValue(taskFolder, out List<FolderItem> items) && items.Count > 0)
            {
                for (int m = 0; m < items.Count; m++)
                {
                    FolderItem folderItem = items[m];
                    TaskAddButton taskAddButton2 = GameObject.Instantiate<TaskAddButton>(__instance.RoleButton);
                    taskAddButton2.Overlay.gameObject.SetActive(false);
                    taskAddButton2.SafePositionWorld = __instance.SafePositionWorld;
                    taskAddButton2.Text.text = folderItem.Name;
                    taskAddButton2.GetComponent<SpriteRenderer>().color = folderItem.Color;
                    taskAddButton2.RolloverHandler.OutColor = folderItem.Color;
                    taskAddButton2.Button?.SetNewAction(folderItem.OnClick.Invoke);
                    __instance.AddFileAsChild(__instance.Root, taskAddButton2, ref num, ref num2, ref num3);
                    if (taskAddButton2 != null && taskAddButton2.Button != null)
                    {
                        ControllerManager.Instance.AddSelectableUiElement(taskAddButton2.Button, false);
                        if (m == 0)
                        {
                            ControllerManager.Instance.SetDefaultSelection(taskAddButton2.Button, null);
                        }
                    }
                }
            }
            if (scroller)
            {
                scroller.CalculateAndSetYBounds(__instance.ActiveItems.Count, 6f, 4.5f, 1.65f);
                scroller.SetYBoundsMin(0f);
                scroller.ScrollToTop();
            }
            if (__instance.Hierarchy.Count == 1)
            {
                ControllerManager.Instance.SetBackButton(__instance.BackButton);
                return false;
            }
            ControllerManager.Instance.SetBackButton(__instance.FolderBackButton);
            return false;
        }
        public static void SetFolderColor(this TaskFolder taskFolder, Color color)
        {
            taskFolder.currentFolderColor = color;
            taskFolder.folderSpriteRenderer.color = taskFolder.currentFolderColor;
            taskFolder.buttonRolloverHandler.OutColor = taskFolder.currentFolderColor;
            taskFolder.buttonRolloverHandler.UnselectedColor = taskFolder.currentFolderColor;
        }
    }
}
