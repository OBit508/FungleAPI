using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Assets;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using HarmonyLib;
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

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(TaskAdderGame))]
    public static class TaskAdderGamePatch
    {
        public static int index;
        public static ModPlugin LastPlugin;
        public static Scroller scroller;
        public static Dictionary<TaskFolder, List<RoleBehaviour>> Folders = new Dictionary<TaskFolder, List<RoleBehaviour>>();
        [HarmonyPatch("Begin")]
        [HarmonyPrefix]
        public static void BeginPrefix(TaskAdderGame __instance)
        {
            __instance.RootFolderPrefab.Text.fontMaterial.SetFloat("_Stencil", 1f);
            __instance.RootFolderPrefab.Text.fontMaterial.SetFloat("_StencilComp", 4f);
            __instance.RootFolderPrefab.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            __instance.TaskPrefab.Text.fontMaterial.SetFloat("_Stencil", 1f);
            __instance.TaskPrefab.Text.fontMaterial.SetFloat("_StencilComp", 4f);
            __instance.TaskPrefab.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(TaskAdderGame __instance)
        {
            Folders.Clear();
            TaskFolder RoleFolder = GameObject.Instantiate<TaskFolder>(__instance.RootFolderPrefab, __instance.transform);
            RoleFolder.gameObject.SetActive(false);
            RoleFolder.FolderName = TranslationController.Instance.GetString(StringNames.Roles);
            __instance.Root.SubFolders.Add(RoleFolder);
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                TaskFolder folder = GameObject.Instantiate<TaskFolder>(__instance.RootFolderPrefab, __instance.transform);
                folder.gameObject.SetActive(false);
                folder.FolderName = plugin.ModName;
                Dictionary<ModdedTeam, List<RoleBehaviour>> roles = new Dictionary<ModdedTeam, List<RoleBehaviour>>();
                foreach (RoleBehaviour role in plugin.Roles)
                {
                    if (roles.ContainsKey(role.GetTeam()))
                    {
                        roles[role.GetTeam()].Add(role);
                    }
                    else
                    {
                        roles.Add(role.GetTeam(), new List<RoleBehaviour>() { role });
                    }
                }
                foreach (KeyValuePair<ModdedTeam, List<RoleBehaviour>> pair in roles)
                {
                    TaskFolder folder2 = GameObject.Instantiate<TaskFolder>(__instance.RootFolderPrefab, __instance.transform);
                    folder2.gameObject.SetActive(false);
                    folder2.FolderName = pair.Key.TeamName.GetString();
                    Folders.Add(folder2, pair.Value);
                    folder.SubFolders.Add(folder2);
                }
                RoleFolder.SubFolders.Add(folder);
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
            gameObject.transform.localScale = new Vector3(7.5f, 6.5f, 1f);
            gameObject.transform.localPosition = new Vector3(2.8f, -2.2f, 0f);
            SpriteMask spriteMask = gameObject.AddComponent<SpriteMask>();
            spriteMask.sprite = ResourceHelper.EmptySprite;
            spriteMask.alphaCutoff = 0f;
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
            collider.enabled = true;
            scroller.ClickMask = collider;
            __instance.GoToRoot();
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
                GameObject.Destroy(__instance.ActiveItems[j].gameObject);
            }
            __instance.ActiveItems.Clear();
            float num = 0f;
            float num2 = 1.7f;
            float num3 = 0f;
            for (int k = 0; k < taskFolder.SubFolders.Count; k++)
            {
                TaskFolder folder = taskFolder.SubFolders[k];
                TaskFolder taskFolder2 = GameObject.Instantiate<TaskFolder>(folder, __instance.TaskParent);
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
                    if (!string.IsNullOrEmpty(__instance.restorePreviousSelectionByFolderName) && taskFolder2.FolderName.Equals(__instance.restorePreviousSelectionByFolderName))
                    {
                        __instance.restorePreviousSelectionFound = taskFolder2.Button;
                    }
                }
            }
            bool flag = false;
            List<PlayerTask> list = taskFolder.Children.ToArray().ToList().OrderBy((PlayerTask t) => t.TaskType).ToList<PlayerTask>();
            for (int l = 0; l < list.Count; l++)
            {
                TaskAddButton taskAddButton = GameObject.Instantiate<TaskAddButton>(__instance.TaskPrefab);
                taskAddButton.MyTask = list[l];
                if (taskAddButton.MyTask.TaskType == TaskTypes.DivertPower)
                {
                    SystemTypes targetSystem = ((DivertPowerTask)taskAddButton.MyTask).TargetSystem;
                    taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.DivertPowerTo);
                }
                else if (taskAddButton.MyTask.TaskType == TaskTypes.FixWeatherNode)
                {
                    int nodeId = ((WeatherNodeTask)taskAddButton.MyTask).NodeId;
                    taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixWeatherNode) + " " + DestroyableSingleton<TranslationController>.Instance.GetString(WeatherSwitchGame.ControlNames[nodeId]);
                }
                else
                {
                    taskAddButton.Text.text = DestroyableSingleton<TranslationController>.Instance.GetString(taskAddButton.MyTask.TaskType);
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
                            __instance.restorePreviousSelectionByFolderName = component.FolderName;
                        }
                        ControllerManager.Instance.SetDefaultSelection(taskAddButton.Button, null);
                        flag = true;
                    }
                }
            }
            List<RoleBehaviour> folderRoles;
            if (__instance.Hierarchy.Count > 1 && Folders.TryGetValue(taskFolder, out folderRoles))
            {
                for (int m = 0; m < folderRoles.Count; m++)
                {
                    RoleBehaviour roleBehaviour = folderRoles[m];
                    if (roleBehaviour.Role != RoleTypes.ImpostorGhost && roleBehaviour.Role != RoleTypes.CrewmateGhost && roleBehaviour != CustomRoleManager.NeutralGhost)
                    {
                        TaskAddButton taskAddButton2 = GameObject.Instantiate<TaskAddButton>(__instance.RoleButton);
                        taskAddButton2.SafePositionWorld = __instance.SafePositionWorld;
                        taskAddButton2.Text.text = "Be_" + roleBehaviour.NiceName + ".exe";
                        __instance.AddFileAsChild(__instance.Root, taskAddButton2, ref num, ref num2, ref num3);
                        taskAddButton2.Role = roleBehaviour;
                        taskAddButton2.RolloverHandler.OutColor = taskAddButton2.Role.TeamColor;
                        if (taskAddButton2 != null && taskAddButton2.Button != null)
                        {
                            ControllerManager.Instance.AddSelectableUiElement(taskAddButton2.Button, false);
                            if (m == 0 && __instance.restorePreviousSelectionFound != null)
                            {
                                ControllerManager.Instance.SetDefaultSelection(__instance.restorePreviousSelectionFound, null);
                                __instance.restorePreviousSelectionByFolderName = string.Empty;
                                __instance.restorePreviousSelectionFound = null;
                            }
                            else if (m == 0)
                            {
                                ControllerManager.Instance.SetDefaultSelection(taskAddButton2.Button, null);
                            }
                        }
                    }
                }
            }
            if (scroller)
            {
                scroller.CalculateAndSetYBounds((float)__instance.ActiveItems.Count, 6f, 4.5f, 1.65f);
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
    }
}
