using AmongUs.GameOptions;
using FungleAPI.Assets;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.UIR;
using xCloud;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ShipStatus), "Start")]
    internal class ShipPatch
    {
        public static int index;
        public static ModPlugin LastPlugin;
        public static Vector3 PcPosition 
        {
            get
            {
                if (ShipStatus.Instance.name == "MiraShip(Clone)")
                {
                    return new Vector3(24.3f, 3.8f, 0.002f);
                }
                else if (ShipStatus.Instance.name == "PolusShip(Clone)")
                {
                    return new Vector3(18.3031f, -16.943f, -0.117f);
                }
                else if (ShipStatus.Instance.name == "Airship(Clone)")
                {
                    return new Vector3(-2.14f, 0.924f, 0.1f);
                }
                else if (ShipStatus.Instance.name == "FungleShip(Clone)")
                {
                    return new Vector3(-3.758f, -2.411f, 1.052f);
                }
                else
                {
                    return new Vector3(1.08f, 3.37f, 0.002f);
                }
            }
        }
        public static void Postfix(ShipStatus __instance)
        {
            string name = "TaskAddConsole";
            if (__instance.name == "FungleShip(Clone)")
            {
                name = "Laptop";
            }
            SystemConsole obj = GameObject.Find(name).GetComponent<SystemConsole>();
            CustomConsole customConsole = CustomConsole.CreateConsole(obj.UsableDistance, true, delegate
            {
                TaskAdderGame taskAdderGame = GameObject.Instantiate<TaskAdderGame>(obj.MinigamePrefab.Cast<TaskAdderGame>(), Camera.main.transform);
                Minigame.Instance = taskAdderGame;
                Transform transform = new GameObject("RolesParent").transform;
                taskAdderGame.transform.Find("BackButton").GetComponent<PassiveButton>().SetNewAction(delegate
                {
                    Back(taskAdderGame, transform);
                });
                taskAdderGame.transform.Find("HomeButton").GetComponent<PassiveButton>().SetNewAction(delegate
                {
                    LoadMain(taskAdderGame, transform);
                });
                taskAdderGame.transform.GetChild(2).GetComponent<TextMeshPro>().text = "Role Tester 5080";
                Scroller scroller = taskAdderGame.gameObject.AddComponent<Scroller>();
                scroller.ContentYBounds.min = 0;
                scroller.allowX = false;
                scroller.allowY = true;
                scroller.Inner = transform;
                GameObject gameObject = new GameObject("Hitbox");
                gameObject.layer = 5;
                gameObject.transform.SetParent(taskAdderGame.transform, false);
                gameObject.transform.localScale = new Vector3(10, 0.4f, 1f);
                gameObject.transform.localPosition = new Vector3(2.8f, -0.1f, 0f);
                SpriteMask spriteMask = gameObject.AddComponent<SpriteMask>();
                spriteMask.sprite = ResourceHelper.EmptySprite;
                spriteMask.alphaCutoff = 0f;
                BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(1f, 1f);
                collider.enabled = true;
                scroller.ClickMask = collider;
                transform.SetParent(taskAdderGame.transform);
                transform.localScale = new Vector3(0.8f, 0.8f);
                transform.localPosition = new Vector3(-0.4f, 0.1f, 0);
                LoadMain(taskAdderGame, transform);
                taskAdderGame.StartCoroutine(taskAdderGame.CoAnimateOpen());
                PlayerControl.LocalPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }, obj.GetComponent<SpriteRenderer>().sprite);
            customConsole.transform.SetParent(__instance.transform);
            customConsole.transform.localRotation = new Quaternion(0f, 1f, 0f, 0f);
            customConsole.transform.localScale = obj.transform.localScale;
            customConsole.transform.position = PcPosition;
            customConsole.useIcon = ImageNames.OptionsButton;
            customConsole.OutlineColor = Color.white;
        }
        public static void Back(TaskAdderGame minigame, Transform transform)
        {
            if (index == 1)
            {
                LoadMain(minigame, transform);
            }
            else if (index == 2)
            {
                LoadPluginFolder(LastPlugin, minigame, transform);
            }
        }
        public static void LoadMain(TaskAdderGame minigame, Transform transform)
        {
            index = 0;
            for (int i = 0; i < transform.GetChildCount(); i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
            minigame.transform.GetChild(6).GetComponent<TextMeshPro>().text = "RoleTester/";
            for (int i = 0; i < ModPlugin.AllPlugins.Count; i++)
            {
                ModPlugin plugin = ModPlugin.AllPlugins[i];
                float num = -2.3f;
                float num2 = 1.7f;
                int num3 = i % 6;
                int num4 = i / 6;
                float num5 = num + 1.15f * (float)num3;
                float num6 = num2 - 1.1f * (float)num4;
                TaskFolder folder = GameObject.Instantiate<TaskFolder>(minigame.RootFolderPrefab, transform);
                folder.transform.SetParent(transform);
                folder.transform.localPosition = new Vector3(num5, num6);
                TextMeshPro text = folder.transform.GetChild(0).GetComponent<TextMeshPro>();
                folder.FolderName = plugin.ModName;
                text.fontMaterial.SetFloat("_Stencil", 1f);
                text.fontMaterial.SetFloat("_StencilComp", 4f);
                folder.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                PassiveButton button = folder.GetComponent<PassiveButton>();
                button.SetNewAction(delegate
                {
                    LastPlugin = plugin;
                    LoadPluginFolder(plugin, minigame, transform);
                });
            }
        }
        public static void LoadPluginFolder(ModPlugin plugin, TaskAdderGame minigame, Transform transform)
        {
            index = 1;
            for (int i = 0; i < transform.GetChildCount(); i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
            minigame.transform.GetChild(6).GetComponent<TextMeshPro>().text = "RoleTester/" + plugin.ModName;
            List<ModdedTeam> teams = new List<ModdedTeam>();
            foreach (RoleBehaviour role in plugin.Roles)
            {
                if (!teams.Contains(role.GetTeam()))
                {
                    teams.Add(role.GetTeam());
                }
            }
            for (int i = 0; i < teams.Count(); i++)
            {
                ModdedTeam team = teams[i];
                float num = -2.3f;
                float num2 = 1.7f;
                int num3 = i % 6;
                int num4 = i / 6;
                float num5 = num + 1.15f * (float)num3;
                float num6 = num2 - 1.3f * (float)num4;
                TaskFolder folder = GameObject.Instantiate<TaskFolder>(minigame.RootFolderPrefab, transform);
                folder.transform.SetParent(transform);
                folder.transform.localPosition = new Vector3(num5, num6);
                TextMeshPro text = folder.transform.GetChild(0).GetComponent<TextMeshPro>();
                folder.FolderName = team.TeamName.GetString();
                text.fontMaterial.SetFloat("_Stencil", 1f);
                text.fontMaterial.SetFloat("_StencilComp", 4f);
                folder.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                folder.GetComponent<PassiveButton>().SetNewAction(delegate
                {
                    LoadRoleFolder(plugin, team, minigame, transform);
                });
            }
        }
        public static void LoadRoleFolder(ModPlugin plugin, ModdedTeam team, TaskAdderGame minigame, Transform transform)
        {
            index = 2;
            for (int i = 0; i < transform.GetChildCount(); i++)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
            minigame.transform.GetChild(6).GetComponent<TextMeshPro>().text = "RoleTester/" + plugin.ModName + "/" + team.TeamName.GetString();
            List<RoleBehaviour> roles = new List<RoleBehaviour>();
            foreach (RoleBehaviour role in plugin.Roles)
            {
                if (role.GetTeam() == team)
                {
                    roles.Add(role);
                }
            }
            for (int i = 0; i < roles.Count; i++)
            {
                float num = -2.3f;
                float num2 = 1.7f;
                int num3 = i % 6;
                int num4 = i / 6;
                float num5 = num + 1.15f * (float)num3;
                float num6 = num2 - 1.3f * (float)num4;
                CreateRoleButton(roles[i], new Vector3(num5, num6), minigame, transform);
            }
        }
        public static void CreateRoleButton(RoleBehaviour role, Vector3 pos, TaskAdderGame minigame, Transform transform)
        {
            GameObject button = GameObject.Instantiate<TaskAddButton>(minigame.RoleButton, transform).gameObject;
            button.GetComponent<TaskAddButton>().Overlay.gameObject.SetActive(false);
            GameObject.Destroy(button.GetComponent<TaskAddButton>());
            button.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.GetComponent<PassiveButton>().OnClick.AddListener(new Action(delegate
            {
                PlayerControl.LocalPlayer.RpcSetRole(role.Role);
            }));
            button.GetComponent<ButtonRolloverHandler>().OutColor = role.TeamColor;
            button.GetComponent<SpriteRenderer>().color = role.TeamColor;
            button.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            TextMeshPro text = button.transform.GetChild(1).GetComponent<TextMeshPro>();
            text.text = "Be_" + role.NiceName + ".exe";
            text.fontMaterial.SetFloat("_Stencil", 1f);
            text.fontMaterial.SetFloat("_StencilComp", 4f);
            Vector3 localPosition = button.transform.GetChild(1).localPosition;
            localPosition.y = -0.6f;
            button.transform.GetChild(1).localPosition = localPosition;
            button.transform.localPosition = pos;
        }
    }
}
