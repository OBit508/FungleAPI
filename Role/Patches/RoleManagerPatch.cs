using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Types;
using FungleAPI.GameOver;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(RoleManager))]
    internal static class RoleManagerPatch
    {
        [HarmonyPatch("SetRole")]
        [HarmonyPrefix]
        public static void SetRolePrefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer)
        {
            RoleBehaviour role = targetPlayer.Data.Role;
            if (role != null)
            {
                targetPlayer.GetComponent<PlayerHelper>().OldRole = __instance.GetRole(role.Role);
                if (role.CanUseKillButton)
                {
                    CustomRoleManager.CurrentKillConfig?.ResetButton?.Invoke();
                }
                if (role.CanSabotage())
                {
                    CustomRoleManager.CurrentSabotageConfig?.ResetButton?.Invoke();
                }
                if (role.CanVent)
                {
                    CustomRoleManager.CurrentVentConfig?.ResetButton?.Invoke();
                }
                CustomRoleManager.CurrentReportConfig?.ResetButton?.Invoke();
            }
        }
        [HarmonyPatch("SetRole")]
        [HarmonyPostfix]
        public static void SetRolePostfix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer)
        {
            RoleBehaviour role = targetPlayer.Data.Role;
            if (role != null)
            {
                CustomRoleManager.UpdateRole(role);
                if (role.CanUseKillButton)
                {
                    CustomRoleManager.CurrentKillConfig.InitializeButton?.Invoke();
                }
                if (role.CanSabotage())
                {
                    CustomRoleManager.CurrentSabotageConfig.InitializeButton?.Invoke();
                }
                if (role.CanVent)
                {
                    CustomRoleManager.CurrentVentConfig.InitializeButton?.Invoke();
                }
                CustomRoleManager.CurrentReportConfig.InitializeButton?.Invoke();
            }
            EventManager.CallEvent(new OnSetRole() { Player = targetPlayer, Role = role });
        }
        [HarmonyPrefix]
        [HarmonyPatch("AssignRoleOnDeath")]
        public static bool AssignRoleOnDeath(RoleManager __instance, [HarmonyArgument(0)] PlayerControl plr)
        {
            if (!plr || !plr.Data.IsDead)
            {
                return false;
            }
            ICustomRole role = plr.Data.Role.CustomRole();
            if (role == null)
            {
                return true;
            }
            if (role.AvaibleGhostRoles == null || role.AvaibleGhostRoles.Count <= 0)
            {
                plr.RpcSetRole(role.GhostRole);
                return false;
            }
            plr.RpcSetRole(role.AvaibleGhostRoles[new System.Random().Next(0, role.AvaibleGhostRoles.Count - 1)], false);
            return false;
        }

        [HarmonyPatch("SelectRoles")]
        [HarmonyPrefix]
        public static bool SelectRolesPrefix(RoleManager __instance)
        {
            if (GameManager.Instance.LogicRoleSelection is LogicRoleSelectionNormal)
            {
                SelectRoles();
                return false;
            }
            return true;
        }
        public static void SelectRoles()
        {
            if (GameManager.Instance != null && GameManager.Instance.LogicRoleSelection is LogicRoleSelectionNormal logicRoleSelectionNormal)
            {
                IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
                Il2CppSystem.Collections.Generic.List<ClientData> list = new Il2CppSystem.Collections.Generic.List<ClientData>();
                AmongUsClient.Instance.GetAllClients(list);
                List<NetworkedPlayerInfo> list2 = Enumerable.ToList<NetworkedPlayerInfo>(Enumerable.Select<ClientData, NetworkedPlayerInfo>(Enumerable.OrderBy<ClientData, int>(Enumerable.Where<ClientData>(Enumerable.Where<ClientData>(Enumerable.Where<ClientData>(list.ToSystemList(), (ClientData c) => c.Character != null), (ClientData c) => c.Character.Data != null), (ClientData c) => !c.Character.Data.Disconnected && !c.Character.Data.IsDead), (ClientData c) => c.Id), (ClientData c) => c.Character.Data));
                foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
                {
                    if (networkedPlayerInfo.Object != null && networkedPlayerInfo.Object.isDummy)
                    {
                        list2.Add(networkedPlayerInfo);
                    }
                }
                List<ModdedTeam> teams = ModdedTeam.Teams.ToArray().ToList();
                teams.Sort((a, b) => b.CountAndPriority.GetCount().CompareTo(a.CountAndPriority.GetCount()));
                Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players = list2.ToIl2CppList();
                foreach (ModdedTeam team in teams)
                {
                    IRoleOptionsCollection roleOptions = currentGameOptions.RoleOptions;
                    int assignedCount = 0;
                    IEnumerable<RoleBehaviour> availableRoles = DestroyableSingleton<RoleManager>.Instance.AllRoles.ToSystemList().Where(role => role.GetTeam() == team && !RoleManager.IsGhostRole(role.Role) && (team.AssignOnlyEnabledRoles && roleOptions.GetChancePerGame(role.Role) > 0 && roleOptions.GetNumPerGame(role.Role) > 0 || !team.AssignOnlyEnabledRoles));
                    Il2CppSystem.Collections.Generic.List<RoleTypes> roleList = new Il2CppSystem.Collections.Generic.List<RoleTypes>();
                    foreach (RoleManager.RoleAssignmentData roleData in availableRoles.Where(role => roleOptions.GetChancePerGame(role.Role) == 100).Select(role => new RoleManager.RoleAssignmentData(role, roleOptions.GetNumPerGame(role.Role), 100)))
                    {
                        for (int i = 0; i < roleData.Count; i++)
                        {
                            roleList.Add(roleData.Role.Role);
                        }
                    }
                    logicRoleSelectionNormal.AssignRolesFromList(players, team.CountAndPriority.GetCount(), roleList, ref assignedCount);
                    roleList.Clear();
                    foreach (RoleManager.RoleAssignmentData roleData in availableRoles.Select(role => new { role, chance = roleOptions.GetChancePerGame(role.Role) }).Where(x => x.chance > 0 && x.chance < 100).Select(x => new RoleManager.RoleAssignmentData(x.role, roleOptions.GetNumPerGame(x.role.Role), x.chance)))
                    {
                        for (int i = 0; i < roleData.Count; i++)
                        {
                            if (HashRandom.Next(101) < roleData.Chance)
                            {
                                roleList.Add(roleData.Role.Role);
                            }
                        }
                    }
                    logicRoleSelectionNormal.AssignRolesFromList(players, team.CountAndPriority.GetCount(), roleList, ref assignedCount);
                    while (roleList.Count < players.Count && roleList.Count + assignedCount < team.CountAndPriority.GetCount())
                    {
                        roleList.Add(team.DefaultRole);
                    }
                    logicRoleSelectionNormal.AssignRolesFromList(players, team.CountAndPriority.GetCount(), roleList, ref assignedCount);
                }
            }
        }
    }
}
