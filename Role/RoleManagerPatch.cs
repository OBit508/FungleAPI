using AmongUs.GameOptions;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using HarmonyLib;
using InnerNet;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role
{
    [HarmonyPatch(typeof(RoleManager))]
    public class RoleManagerPatch
    {
        [HarmonyPatch("SetRole")]
        public static void Postfix(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer, [HarmonyArgument(1)] RoleTypes roleType)
        {
            LoadButtons(__instance.GetRole(roleType));
        }
        [HarmonyPatch("SelectRoles")]
        public static bool Prefix()
        {
            Il2CppSystem.Collections.Generic.List<ClientData> list = new Il2CppSystem.Collections.Generic.List<ClientData>();
            AmongUsClient.Instance.GetAllClients(list);
            List<PlayerControl> players = (from c in list.ToArray().ToList()
                                               where c.Character != null
                                               where c.Character.Data != null
                                               where !c.Character.Data.Disconnected && !c.Character.Data.IsDead
                                               orderby c.Id
                                               select c.Character).ToList();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Object != null && networkedPlayerInfo.Object.isDummy)
                {
                    players.Add(networkedPlayerInfo.Object);
                }
            }
            IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            List<RoleTypes> crewmateRoles = new List<RoleTypes>();
            List<RoleTypes> impostorRoles = new List<RoleTypes>();
            List<RoleTypes> neutralRoles = new List<RoleTypes>();
            Dictionary<List<RoleTypes>, ModdedTeam> other = new Dictionary<List<RoleTypes>, ModdedTeam>();
            foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
            {
                if (role.GetRolePlugin() == FungleAPIPlugin.Plugin)
                {
                    int count = role.CaculeCountByChance(currentGameOptions.RoleOptions);
                    if (count > 0)
                    {
                        if (role.GetTeam() == ModdedTeam.Crewmates)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                crewmateRoles.Add(role.Role);
                            }
                        }
                        else if (role.GetTeam() == ModdedTeam.Impostors)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                impostorRoles.Add(role.Role);
                            }
                        }
                    }
                }
            }
            foreach (ModdedTeam team in ModdedTeam.Teams)
            {
                if (team.TeamPlugin != FungleAPIPlugin.Plugin || team == ModdedTeam.Neutrals)
                {
                    List<RoleTypes> teamRoles = new List<RoleTypes>();
                    foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
                    {
                        if (role.GetTeam() == team)
                        {
                            teamRoles.Add(role.Role);
                        }
                    }
                    other.Add(teamRoles, team);
                }
            }
            for (int i = 0; i < currentGameOptions.GetInt(Int32OptionNames.NumImpostors); i++)
            {
                if (players.Count > 0)
                {

                    PlayerControl player = players[UnityEngine.Random.Range(0, players.Count)];
                    if (impostorRoles.Count > 0)
                    {
                        RoleTypes role = impostorRoles[UnityEngine.Random.Range(0, impostorRoles.Count)];
                        player.RpcSetRole(role);
                        impostorRoles.Remove(role);
                    }
                    else
                    {
                        player.RpcSetRole(RoleTypes.Impostor);
                    }
                    players.Remove(player);
                }
            }
            for (int i = 0; i < other.Count; i++)
            {
                List<RoleTypes> roles = other.Keys.ToArray()[UnityEngine.Random.Range(0, other.Count)];
                ModdedTeam team = other[roles];
                int t = 0;
                while (t < team.GetCount())
                {
                    t++;
                    if (players.Count > 0 && roles.Count > 0)
                    {
                        PlayerControl player = players[UnityEngine.Random.Range(0, players.Count)];
                        RoleTypes role = roles[UnityEngine.Random.Range(0, roles.Count)];
                        player.RpcSetRole(role);
                        roles.Remove(role);
                        players.Remove(player);
                    }
                }
                other.Remove(roles);
            }
            for (int i = 0; i < players.Count; i++)
            {
                if (players.Count > 0)
                {
                    PlayerControl player = players[UnityEngine.Random.Range(0, players.Count)];
                    if (crewmateRoles.Count > 0)
                    {
                        RoleTypes role = crewmateRoles[UnityEngine.Random.Range(0, crewmateRoles.Count)];
                        player.RpcSetRole(role);
                        crewmateRoles.Remove(role);
                    }
                    else
                    {
                        player.RpcSetRole(RoleTypes.Crewmate);
                    }
                    players.Remove(player);
                }
            }
            return false;
        }
        public static void LoadButtons(RoleBehaviour role)
        {
            PlayerHelper helper = PlayerControl.LocalPlayer.GetComponent<PlayerHelper>();
            RoleBehaviour oldRole = helper.OldRole;
            helper.OldRole = role;
            if (oldRole != null && oldRole.CustomRole() != null)
            {
                foreach (CustomAbilityButton button in oldRole.CustomRole().CachedConfiguration.Buttons)
                {
                    button.Destroy();
                }
            }
            ICustomRole customRole = role.CustomRole();
            if (customRole != null)
            {
                
                foreach (CustomAbilityButton button in customRole.CachedConfiguration.Buttons)
                {
                    button.CreateButton();
                }
            }
        }
    }
}
