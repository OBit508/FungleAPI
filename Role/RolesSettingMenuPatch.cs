using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI;
using FungleAPI.Roles;
using HarmonyLib;
using UnityEngine;
using UnityEngine.ProBuilder;
using System.Reflection;
using TMPro;
using static Rewired.UI.ControlMapper.ControlMapper;
using FungleAPI.MonoBehaviours;
using FungleAPI.Assets;
using FungleAPI.Rpc;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using AmongUs.GameOptions;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RolesSettingsMenu))]
    internal class RolesSettingMenuPatch
    {
        public static ModPlugin currentPlugin;
        public static ModPlugin chanceTabPlugin;
        public static float scrollMax;
        public static ToggleOption togglePrefab;
        public static NumberOption numberPrefab;
        public static SpriteRenderer labelSprite;
        public static TextMeshPro labelText;
        public static GameObject AdvancedTab;
        public static List<OptionBehaviour> options = new List<OptionBehaviour>();
        public static PassiveButton SwitchButton;
        public static int currentIndex;
        public static void Reset()
        {
            currentPlugin = FungleAPIPlugin.Plugin;
            chanceTabPlugin = null;
            scrollMax = 0.7f;
            options.Clear();
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void OnUpdate(RolesSettingsMenu __instance)
        {
            if (SwitchButton == null)
            {
                Reset();
                SwitchButton = GameObject.Instantiate<PassiveButton>(__instance.transform.parent.parent.GetChild(4).GetChild(0).GetComponent<PassiveButton>(), __instance.transform.parent.parent.GetChild(4));
                SwitchButton.transform.localPosition = new Vector3(-2.96f, 1.57f, -2);
                SwitchButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                SwitchButton.OnClick.AddListener(new Action(delegate
                {
                    Reset();
                    if ((currentIndex + 1) >= ModPlugin.AllPlugins.Count)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        currentIndex++;
                        currentPlugin = ModPlugin.AllPlugins[currentIndex];
                    }
                }));
                SwitchButton.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    __instance.transform.parent.parent.GetChild(2).gameObject.SetActive(!__instance.RoleChancesSettings.active);
                    SwitchButton.gameObject.SetActive(__instance.RoleChancesSettings.active);
                    SwitchButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = currentPlugin.ModName;
                });
            }
            if (togglePrefab == null)
            {
                __instance.roleTabs[2].ReceiveClickDown();
                togglePrefab = UnityEngine.Object.Instantiate(__instance.AdvancedRolesSettings.transform.GetChild(9).GetComponent<ToggleOption>(), __instance.transform);
                numberPrefab = UnityEngine.Object.Instantiate(__instance.AdvancedRolesSettings.transform.GetChild(8).GetComponent<NumberOption>(), __instance.transform);
                togglePrefab.gameObject.SetActive(false);
                numberPrefab.gameObject.SetActive(false);
                __instance.roleTabs[0].ReceiveClickDown();
            }
            if (AdvancedTab == null)
            {
                AdvancedTab = UnityEngine.Object.Instantiate(__instance.AdvancedRolesSettings, __instance.AdvancedRolesSettings.transform.parent);
                for (int i = 1; i < AdvancedTab.transform.GetChildCount(); i++)
                {
                    Transform t = AdvancedTab.transform.GetChild(i);
                    if (i == 2)
                    {
                        labelSprite = t.GetChild(0).GetComponent<SpriteRenderer>();
                        labelText = t.GetChild(1).GetComponent<TextMeshPro>();
                    }
                    else
                    {
                        t.gameObject.SetActive(false);
                    }
                }
                AdvancedTab.SetActive(false);
            }
            if (currentPlugin != chanceTabPlugin)
            {
                chanceTabPlugin = currentPlugin;
                for (int i = 10; i < __instance.RoleChancesSettings.transform.GetChildCount(); i++)
                {
                    UnityEngine.Object.Destroy(__instance.RoleChancesSettings.transform.GetChild(i).gameObject);
                }
                if (currentPlugin != FungleAPIPlugin.Plugin)
                {
                    LoadChanceTab(currentPlugin, __instance);
                    for (int i = 9; i >= 0; i--)
                    {
                        __instance.RoleChancesSettings.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    for (int i = 9; i >= 0; i--)
                    {
                        __instance.RoleChancesSettings.transform.GetChild(i).gameObject.SetActive(true);
                    }
                    __instance.AllButton.transform.parent.gameObject.SetActive(true);
                    __instance.scrollBar.transform.localPosition = new Vector3(-1.4957f, 0.657f, -4);
                    __instance.scrollBar.transform.GetChild(1).localScale = new Vector3(6.6811f, 3.5563f, 0.5598f);
                    Vector3 vec = __instance.scrollBar.Inner.transform.localPosition;
                    vec.y = 0;
                    __instance.scrollBar.Inner.transform.localPosition = vec;
                    __instance.scrollBar.ContentYBounds.min = 0;
                    __instance.scrollBar.ContentYBounds.max = 8;
                }
            }
            if (__instance.RoleChancesSettings.active)
            {
                if (options.Count > 0)
                {
                    foreach (OptionBehaviour op in options)
                    {
                        UnityEngine.Object.Destroy(op.gameObject);
                    }
                    options.Clear();
                }
                AdvancedTab.SetActive(false);
            }
            SwitchButton.GetComponent<Updater>().Update();
        }
        public static void LoadChanceTab(ModPlugin plugin, RolesSettingsMenu menu)
        {
            Vector3 Pos = new Vector3(4.986f, 0.662f, -2);
            menu.AllButton.transform.parent.gameObject.SetActive(false);
            menu.scrollBar.transform.localPosition = new Vector3(-1.4957f, 1.057f, -4);
            menu.scrollBar.transform.GetChild(1).localScale = new Vector3(6.6811f, 4.2563f, 0.5598f);
            Vector3 vec = menu.scrollBar.Inner.transform.localPosition;
            vec.y = 0.7f;
            menu.scrollBar.Inner.transform.localPosition = vec;
            CategoryHeaderEditRole crewHeader = CreateHeader(menu, new Color(0.4706f, 0.8f, 0.925f), new Color(0.0392f, 0.3412f, 0.451f), new Color(0.0392f, 0.3412f, 0.451f, 0.498f), new Color(0.0549f, 0.3569f, 0.4667f), "Crewmate Roles");
            CategoryHeaderEditRole impHeader = CreateHeader(menu, new Color(0.8f, 0.3255f, 0.3255f), new Color(0.5373f, 0.0863f, 0.0863f), new Color(0.5373f, 0.0863f, 0.0863f, 0.498f), new Color(0.3451f, 0.1373f, 0.1373f), "Impostor Roles");
            CategoryHeaderEditRole neutralHeader = CreateHeader(menu, Utils.Light(Color.gray, 0.7f), Color.gray, Utils.Dark(Color.gray, 0.7f), Utils.Light(Color.gray, 0.9f), "Neutral Roles");
            List<RoleBehaviour> crewmateRoles = new List<RoleBehaviour>();
            List<RoleBehaviour> impostorRoles = new List<RoleBehaviour>();
            List<RoleBehaviour> neutralRoles = new List<RoleBehaviour>();
            List<ModdedTeam> customTeams = new List<ModdedTeam>();
            foreach ((RoleTypes role, Type type) roleType in plugin.Roles)
            {
                RoleBehaviour role = RoleManager.Instance.GetRole(roleType.role);
                if (role.GetTeam() == ModdedTeam.Crewmates)
                {
                    crewmateRoles.Add(role);
                }
                else if (role.GetTeam() == ModdedTeam.Impostors)
                {
                    impostorRoles.Add(role);
                }
                else if (role.GetTeam() == ModdedTeam.Neutrals)
                {
                    neutralRoles.Add(role);
                }
                else if (!customTeams.Contains(role.GetTeam()))
                {
                    customTeams.Add(role.GetTeam());
                }
            }
            if (crewmateRoles.Count > 0)
            {
                crewHeader.gameObject.SetActive(true);
                crewHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(crewmateRoles, Pos, menu);
            }
            if (impostorRoles.Count > 0)
            {
                impHeader.gameObject.SetActive(true);
                impHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(impostorRoles, Pos, menu);
            }
            if (neutralRoles.Count > 0)
            {
                neutralHeader.gameObject.SetActive(true);
                neutralHeader.transform.localPosition = Pos;
                Pos = OrganizeRoles(neutralRoles, Pos, menu);
            }
            foreach (ModdedTeam team in customTeams)
            {
                List<RoleBehaviour> teamRoles = new List<RoleBehaviour>();
                foreach ((RoleTypes role, Type type) roleType in plugin.Roles)
                {
                    RoleBehaviour role = RoleManager.Instance.GetRole(roleType.role);
                    if (role.GetTeam() == team)
                    {
                        teamRoles.Add(role);
                    }
                }
                CategoryHeaderEditRole header = CreateHeader(menu, Utils.Light(team.TeamColor, 0.7f), team.TeamColor, Utils.Dark(team.TeamColor, 0.7f), Utils.Light(team.TeamColor, 0.9f), team.TeamName.GetString());
                header.transform.localPosition = Pos;
                Pos = OrganizeRoles(teamRoles, Pos, menu);
                header.gameObject.SetActive(true);
            }
            menu.scrollBar.ContentYBounds.min = 0.7f;
            menu.scrollBar.ContentYBounds.max = scrollMax;
            menu.AdvancedRolesSettings.transform.GetChild(0).localPosition = new Vector3(1.4041f, -1.3688f, 0);
            menu.AdvancedRolesSettings.transform.GetChild(0).localScale = new Vector3(0.0675f, 0.2094f, 0.5687f);
        }
        public static Vector3 OrganizeRoles(List<RoleBehaviour> roles, Vector3 currentVec, RolesSettingsMenu menu)
        {
            currentVec.y = currentVec.y - 0.522f;
            float x = currentVec.x;
            for (int i = 0; i < roles.Count; i++)
            {
                scrollMax = scrollMax + 0.15f;
                currentVec.x = menu.RoleChancesSettings.transform.GetChild(2).localPosition.x;
                ICustomRole role = roles[i].CustomRole();
                RoleOptionSetting option = UnityEngine.Object.Instantiate(menu.RoleChancesSettings.transform.GetChild(2).GetComponent<RoleOptionSetting>(), menu.RoleChancesSettings.transform);
                option.transform.localPosition = currentVec;
                option.titleText.text = role.RoleName.GetString();
                option.labelSprite.color = role.RoleColor;
                int count = role.RoleCount.Value;
                int chance = role.RoleChance.Value;
                option.countText.text = count.ToString();
                option.chanceText.text = chance.ToString();
                Action send = new Action(delegate
                {
                    string s = "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName.GetString() + "</color>: " + count.ToString() + ", Chance: " + chance.ToString() + "%.";
                    CustomRpcManager.GetInstance<RpcSyncCountAndChance>().Send((role, count, chance, s), PlayerControl.LocalPlayer.NetId);
                });
                option.CountMinusBtn.SetNewAction(delegate
                {
                    if (count - 1 >= 0)
                    {
                        count--;
                        option.countText.text = count.ToString();
                        role.RoleCount.Value = count;
                        send();
                    }
                });
                option.CountMinusBtn.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    option.CountMinusBtn.SetInteractable(count > 0);
                });
                option.CountPlusBtn.SetNewAction(delegate
                {
                    count++;
                    option.countText.text = count.ToString();
                    role.RoleCount.Value = count;
                    send();
                });
                option.ChanceMinusBtn.SetNewAction(delegate
                {
                    if (chance - 10 >= 0)
                    {
                        chance = chance - 10;
                        option.chanceText.text = chance.ToString();
                        role.RoleChance.Value = chance;
                        send();
                    }
                });
                option.ChanceMinusBtn.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    option.ChanceMinusBtn.SetInteractable(chance > 0);
                });
                option.ChancePlusBtn.SetNewAction(delegate
                {
                    if (chance + 10 <= 100)
                    {
                        chance = chance + 10;
                        option.chanceText.text = chance.ToString();
                        role.RoleChance.Value = chance;
                        send();
                    }
                });
                option.ChanceMinusBtn.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    option.ChanceMinusBtn.SetInteractable(chance < 100);
                });
                GameOptionButton cog = UnityEngine.Object.Instantiate(option.ChanceMinusBtn, option.transform);
                cog.transform.GetChild(0).gameObject.SetActive(false);
                cog.buttonSprite.sprite = Cog;
                cog.transform.localPosition = new Vector3(-1.278f, -0.3f, 0f);
                cog.transform.localScale = new Vector3(1, 1, 1);
                cog.SetNewAction(delegate
                {
                    if (role.CachedConfiguration.Configs.Count() > 0)
                    {
                        GameSettingMenu.Instance.MenuDescriptionText.text = role.RoleBlurLong.GetString();
                        menu.AdvancedRolesSettings.gameObject.SetActive(false);
                        menu.RoleChancesSettings.gameObject.SetActive(false);
                        AdvancedTab.SetActive(true);
                        labelSprite.color = Utils.Light(role.RoleColor);
                        labelText.text = role.RoleName.GetString();
                        labelText.color = role.RoleColor;
                        for (int i = 0; i < role.CachedConfiguration.Configs.Count(); i++)
                        {
                            OptionBehaviour op = null;
                            if (role.CachedConfiguration.Configs[i] is NumConfig f)
                            {
                                op = CreateOption(numberPrefab, role, f, AdvancedTab.transform);
                            }
                            else if (role.CachedConfiguration.Configs[i] is BoolConfig b)
                            {
                                op = CreateOption(togglePrefab, role, b, AdvancedTab.transform);
                            }
                            else if (role.CachedConfiguration.Configs[i] is EnumConfig e)
                            {
                                op = CreateOption(numberPrefab, role, e, AdvancedTab.transform);
                            }
                            op.gameObject.SetActive(true);
                            op.transform.localPosition = new Vector3(0.5f, 0.1f - 0.4f * i, -10f);
                            options.Add(op);
                        }
                    }
                });
                cog.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
                {
                    cog.SetInteractable(role.CachedConfiguration.Configs.Count() > 0);
                });
                currentVec.y = currentVec.y - 0.43f;
                option.gameObject.SetActive(true);
            }
            currentVec.y = currentVec.y - 0.65f;
            currentVec.x = x;
            return currentVec;
        }
        public static OptionBehaviour CreateOption(NumberOption prefab, ICustomRole role, NumConfig config, Transform transform)
        {
            NumberOption option = UnityEngine.Object.Instantiate(prefab, transform);
            float num = config.ConfigEntry.Value;
            option.MinusBtn.SetNewAction(delegate
            {
                if (num - config.ReduceValue >= 0)
                {
                    num = num - config.ReduceValue;
                    CustomRpcManager.GetInstance<RpcSetNewRoleValue>().Send((role, config, num, "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName + " (" + config.ConfigName + ")</color>: " + num.ToString() + "."), PlayerControl.LocalPlayer.NetId);
                }
            });
            option.PlusBtn.SetNewAction(delegate
            {
                num = num + config.IncreceValue;
                CustomRpcManager.GetInstance<RpcSetNewRoleValue>().Send((role, config, num, "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName + " (" + config.ConfigName + ")</color>: " + num.ToString() + "."), PlayerControl.LocalPlayer.NetId);
            });
            option.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
            {
                option.TitleText.enabled = true;
                option.TitleText.text = config.ConfigName;
                option.ValueText.enabled = true;
                option.ValueText.text = num.ToString();
            });
            return option;
        }
        public static OptionBehaviour CreateOption(ToggleOption prefab, ICustomRole role, BoolConfig config, Transform transform)
        {
            ToggleOption option = UnityEngine.Object.Instantiate(prefab, transform);
            bool num = config.ConfigEntry.Value;
            option.transform.GetChild(1).GetComponent<PassiveButton>().SetNewAction(delegate
            {
                num = !num;
                CustomRpcManager.GetInstance<RpcSetNewRoleValue>().Send((role, config, num, "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName + " (" + config.ConfigName + ")</color>: " + num.ToString() + "."), PlayerControl.LocalPlayer.NetId);
            });
            option.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
            {
                option.TitleText.enabled = true;
                option.TitleText.text = config.ConfigName;
                option.CheckMark.gameObject.SetActive(num);
            });
            return option;
        }
        public static OptionBehaviour CreateOption(NumberOption prefab, ICustomRole role, EnumConfig config, Transform transform)
        {
            NumberOption option = UnityEngine.Object.Instantiate(prefab, transform);
            option.MinusBtn.SetNewAction(delegate
            {
                config.BackValue();
                CustomRpcManager.GetInstance<RpcSetNewRoleValue>().Send((role, config, config.ConfigEntry.Value, "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName + " (" + config.ConfigName + ")</color>: " + config.ConfigEntry.Value + "."), PlayerControl.LocalPlayer.NetId);
            });
            option.PlusBtn.SetNewAction(delegate
            {
                config.NextValue();
                CustomRpcManager.GetInstance<RpcSetNewRoleValue>().Send((role, config, config.ConfigEntry.Value, "<color=#" + ColorUtility.ToHtmlStringRGB(role.RoleColor) + ">" + role.RoleName + " (" + config.ConfigName + ")</color>: " + config.ConfigEntry.Value + "."), PlayerControl.LocalPlayer.NetId);
            });
            option.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
            {
                option.TitleText.enabled = true;
                option.TitleText.text = config.ConfigName;
                option.ValueText.enabled = true;
                option.ValueText.text = config.ConfigEntry.Value;
            });
            return option;
        }
        public static CategoryHeaderEditRole CreateHeader(RolesSettingsMenu menu, Color background, Color label1, Color label2, Color titleColor, string titleText)
        {
            scrollMax = scrollMax + 0.45f;
            CategoryHeaderEditRole header = UnityEngine.Object.Instantiate(menu.RoleChancesSettings.transform.GetChild(1).GetComponent<CategoryHeaderEditRole>(), menu.RoleChancesSettings.transform);
            header.Background.color = background;
            header.countLabel.color = label1;
            header.chanceLabel.color = label1;
            header.blankLabel.color = label2;
            header.Title.text = titleText;
            header.Title.color = titleColor;
            header.gameObject.SetActive(false);
            return header;
        }
        public static Sprite Cog = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.cog", 100f);
    }
}
