using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.Role.Teams;
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
            if (targetPlayer.Data.Role != null)
            {
                targetPlayer.GetComponent<PlayerHelper>().OldRole = __instance.GetRole(targetPlayer.Data.Role.Role);
            }
        }
        [HarmonyPatch("SetRole")]
        [HarmonyPostfix]
        public static void SetRolePostfix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer)
        {
            RoleBehaviour role = targetPlayer.Data.Role;
            if (role != null && role.CustomRole() != null)
            {
                ICustomRole customRole = role.CustomRole();
                role.StringName = customRole.RoleName;
                role.BlurbName = customRole.RoleBlur;
                role.BlurbNameMed = customRole.RoleBlurMed;
                role.BlurbNameLong = customRole.RoleBlurLong;
                role.NameColor = customRole.RoleColor;
                role.AffectedByLightAffectors = customRole.IsAffectedByLightOnAirship;
                role.CanUseKillButton = customRole.UseVanillaKillButton;
                role.CanVent = customRole.CanUseVent;
                role.TasksCountTowardProgress = customRole.CompletedTasksCountForProgress;
                role.RoleScreenshot = customRole.Screenshot;
                role.RoleIconSolid = customRole.IconSolid;
                role.RoleIconWhite = customRole.IconWhite;
            }
        }
        [HarmonyPatch("AssignRoleOnDeath")]
        [HarmonyPrefix]
        public static bool AssignRoleOnDeathPrefix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] bool specialRolesAllowed)
        {
            if (player == null || !player.Data.IsDead)
            {
                return false;
            }
            if (specialRolesAllowed)
            {
                TryAssignSpecialGhostRoles(player);
            }
            if (!RoleManager.IsGhostRole(player.Data.Role.Role))
            {
                player.RpcSetRole(player.Data.Role.DefaultGhostRole, false);
            }
            return false;
        }
        [HarmonyPatch("SelectRoles")]
        [HarmonyPrefix]
        public static bool SelectRolesPrefix(RoleManager __instance)
        {
            LogicRoleSelection logicRoleSelection = GameManager.Instance.LogicRoleSelection;
            if (logicRoleSelection is LogicRoleSelectionNormal logicRoleSelectionNormal)
            {
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
                IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
                List<ModdedTeam> teams = ModdedTeam.Teams.ToArray().ToList();
                teams.Sort((a, b) => b.CountAndPriority.GetCount().CompareTo(a.CountAndPriority.GetCount()));
                foreach (ModdedTeam team in teams)
                {
                    AssignRolesForTeam(logicRoleSelectionNormal, list2.ToIl2CppList(), currentGameOptions, team);
                }
                return false;
            }
            return true;
        }
        public static void AssignRolesForTeam(LogicRoleSelectionNormal logicRoleSelectionNormal, Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players, IGameOptions opts, ModdedTeam team)
        {
            IRoleOptionsCollection roleOptions = opts.RoleOptions;
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
        public static void TryAssignSpecialGhostRoles(PlayerControl player)
        {
            List<RoleTypes> validRoleTypes = new List<RoleTypes>();
            foreach (RoleTypes type in RoleManager.GhostRoles)
            {
                if (RoleManager.Instance.GetRole(type).GetTeam() == player.Data.Role.GetTeam())
                {
                    validRoleTypes.Add(type);
                }
            }
            RoleTypes roleTypes = validRoleTypes[new System.Random().Next(0, validRoleTypes.Count - 1)];
            int num = PlayerControl.AllPlayerControls.ToArray().Count((PlayerControl pc) => pc.Data.IsDead && pc.Data.RoleType == roleTypes);
            IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
            {
                player.RpcSetRole(roleTypes, false);
                return;
            }
            if (num > roleOptions.GetNumPerGame(roleTypes))
            {
                return;
            }
            int chancePerGame = roleOptions.GetChancePerGame(roleTypes);
            if (HashRandom.Next(101) < chancePerGame)
            {
                player.RpcSetRole(roleTypes, false);
            }
        }
    }
}
