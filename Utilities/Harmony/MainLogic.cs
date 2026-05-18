using AmongUs.Data;
using AmongUs.GameOptions;
using FungleAPI.Extensions;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Harmony
{
    /// <summary>
    /// Patch this class methods to change the API main logic easily
    /// </summary>
    public static class MainLogic
    {
        public static DeadBody GetDeadBody(GameManager gameManager, RoleBehaviour impostorRole)
        {
            return gameManager.deadBodyPrefab[impostorRole.GetCreatedDeadBody() == DeadBodyType.Viper ? 1 : 0];
        }
        public static MapOptions GetMapOptions()
        {
            return new MapOptions
            {
                Mode = (PlayerControl.LocalPlayer.Data.Role.CanSabotage() && !MeetingHud.Instance) ? MapOptions.Modes.Sabotage : MapOptions.Modes.Normal
            };
        }
        public static float CanUseVent(Vent vent, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
        {
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;
            IUsable usable = vent.SafeCast<IUsable>();
            couldUse = pc.Role.CanUseVent() && GameManager.Instance.LogicUsables.CanUse(usable, @object) && pc.Role.CanUse(usable) && (!@object.MustCleanVent(vent.Id) || (@object.inVent && Vent.currentVent == vent)) && !pc.IsDead && (@object.CanMove || @object.inVent);
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Ventilation, out systemType))
            {
                VentilationSystem ventilationSystem = systemType.SafeCast<VentilationSystem>();
                if (ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(vent.Id))
                {
                    couldUse = false;
                }
            }
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = vent.transform.position;
                num = Vector2.Distance(center, position);
                canUse &= (num <= vent.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false));
            }
            return num;
        }
        public static void SelectRoles(RoleManager roleManager)
        {
            if (GameManager.Instance != null && GameManager.Instance.LogicRoleSelection is LogicRoleSelectionNormal logicRoleSelectionNormal)
            {
                IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
                Il2CppSystem.Collections.Generic.List<ClientData> list = new Il2CppSystem.Collections.Generic.List<ClientData>();
                AmongUsClient.Instance.GetAllClients(list);
                List<NetworkedPlayerInfo> list2 = Enumerable.ToList(Enumerable.Select(Enumerable.OrderBy(Enumerable.Where(Enumerable.Where(Enumerable.Where(list.ToSystemList(), c => c.Character != null), c => c.Character.Data != null), c => !c.Character.Data.Disconnected && !c.Character.Data.IsDead), c => c.Id), c => c.Character.Data));
                foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
                {
                    if (networkedPlayerInfo.Object != null && networkedPlayerInfo.Object.isDummy)
                    {
                        list2.Add(networkedPlayerInfo);
                    }
                }
                List<ModdedTeam> teams = ModdedTeamManager.Teams.Values.ToList();
                teams.Sort((a, b) => b.TeamOptions.TeamCount.CompareTo(a.TeamOptions.TeamCount));
                Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players = list2.ToIl2CppList();
                foreach (ModdedTeam team in teams)
                {
                    AssignRolesForTeam(logicRoleSelectionNormal, players, team);
                }
            }
        }
        public static void AssignRolesForTeam(LogicRoleSelectionNormal logicRoleSelectionNormal, Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players, ModdedTeam team)
        {
            int rolesAssigned = 0;
            IEnumerable<RoleBehaviour> availableRoles = RoleManager.Instance.AllRoles.ToSystemList().Where(role => role.GetTeam() == team && !RoleManager.IsGhostRole(role.Role));

            IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;

            Il2CppSystem.Collections.Generic.List<RoleTypes> roleList = new Il2CppSystem.Collections.Generic.List<RoleTypes>();

            IEnumerable<RoleManager.RoleAssignmentData> guaranteedRoles = availableRoles.Where(role => roleOptions.GetChancePerGame(role.Role) == 100).Select(role => new RoleManager.RoleAssignmentData( role, roleOptions.GetNumPerGame(role.Role), 100));

            foreach (RoleManager.RoleAssignmentData assignment in guaranteedRoles)
            {
                for (int i = 0; i < assignment.Count; i++)
                {
                    roleList.Add(assignment.Role.Role);
                }
            }

            logicRoleSelectionNormal.AssignRolesFromList(players, (int)team.MaxCount, roleList, ref rolesAssigned);

            List<RoleManager.RoleAssignmentData> randomRoles = availableRoles.Select(role => new { Role = role, Chance = roleOptions.GetChancePerGame(role.Role)})
                    .Where(x => x.Chance > 0 && x.Chance < 100).Select(x => new RoleManager.RoleAssignmentData(x.Role, roleOptions.GetNumPerGame(x.Role.Role), x.Chance)).ToList();

            roleList.Clear();

            foreach (RoleManager.RoleAssignmentData assignment in randomRoles)
            {
                for (int i = 0; i < assignment.Count; i++)
                {
                    if (HashRandom.Next(101) < assignment.Chance)
                    {
                        roleList.Add(assignment.Role.Role);
                    }
                }
            }

            logicRoleSelectionNormal.AssignRolesFromList(players, (int)team.MaxCount, roleList, ref rolesAssigned);

            while (roleList.Count < players.Count && roleList.Count + rolesAssigned < team.MaxCount)
            {
                roleList.Add(team.DefaultRole);
            }
            logicRoleSelectionNormal.AssignRolesFromList(players, (int)team.MaxCount, roleList, ref rolesAssigned);
        }
        public static void AssignTasks(ShipStatus shipStatus)
        {
            shipStatus.numScans = 0;
            shipStatus.AssignTaskIndexes();
            IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            Il2CppSystem.Collections.Generic.HashSet<TaskTypes> hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();
            Il2CppSystem.Collections.Generic.List<byte> list = new Il2CppSystem.Collections.Generic.List<byte>(10);
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> list2 = shipStatus.CommonTasks.ToList().ToIl2CppList();
            list2.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle();
            list2.ToSystemList().ForEach(delegate (NormalPlayerTask t)
            {
                t.Length = NormalPlayerTask.TaskLength.Common;
            });
            int num = 0;
            int @int = currentGameOptions.GetInt(Int32OptionNames.NumCommonTasks);
            shipStatus.AddTasksFromList(ref num, @int, list, hashSet, list2);
            for (int i = 0; i < @int; i++)
            {
                if (list2.Count == 0)
                {
                    Debug.LogWarning("Not enough common tasks");
                    break;
                }
                int index = list2.ToArray().RandomIdx<NormalPlayerTask>();
                list.Add((byte)list2[index].Index);
                list2.RemoveAt(index);
            }
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> list3 = shipStatus.LongTasks.ToList<NormalPlayerTask>().ToIl2CppList();
            list3.ToSystemList().ForEach(delegate (NormalPlayerTask t)
            {
                t.Length = NormalPlayerTask.TaskLength.Long;
            });
            list3.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle(0);
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> list4 = shipStatus.ShortTasks.ToList<NormalPlayerTask>().ToIl2CppList();
            list4.ToSystemList().ForEach(delegate (NormalPlayerTask t)
            {
                t.Length = NormalPlayerTask.TaskLength.Short;
            });
            list4.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle(0);
            int num2 = 0;
            int num3 = 0;
            int num4 = currentGameOptions.GetInt(Int32OptionNames.NumShortTasks);
            int int2 = currentGameOptions.GetInt(Int32OptionNames.NumCommonTasks);
            int int3 = currentGameOptions.GetInt(Int32OptionNames.NumLongTasks);
            if (int2 + int3 + num4 == 0)
            {
                num4 = 1;
            }
            byte b = 0;
            while ((int)b < allPlayers.Count)
            {
                hashSet.Clear();
                list.RemoveRange(int2, list.Count - int2);
                shipStatus.AddTasksFromList(ref num2, int3, list, hashSet, list3);
                shipStatus.AddTasksFromList(ref num3, num4, list, hashSet, list4);
                NetworkedPlayerInfo networkedPlayerInfo = allPlayers[(int)b];
                if (networkedPlayerInfo.Object && !networkedPlayerInfo.Object.GetComponent<DummyBehaviour>().enabled)
                {
                    byte[] taskTypeIds = list.ToArray();
                    networkedPlayerInfo.RpcSetTasks(taskTypeIds);
                }
                b += 1;
            }
        }
        public static void CheckEndCriteria(GameManager gameManager)
        {
            if (!GameData.Instance)
            {
                return;
            }
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.LifeSupp, out systemType))
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.SafeCast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    if (!TutorialManager.InstanceExists)
                    {
                        gameManager.RpcEndGame(GameOverReason.ImpostorsBySabotage, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                        return;
                    }
                    HudManager.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverSabotage));
                    lifeSuppSystemType.Countdown = 10000f;
                }
            }
            foreach (ISystemType systemType2 in ShipStatus.Instance.Systems.Values)
            {
                ICriticalSabotage criticalSabotage = systemType2.SafeCast<ICriticalSabotage>();
                if (criticalSabotage != null && criticalSabotage.Countdown < 0f)
                {
                    if (!TutorialManager.InstanceExists)
                    {
                        gameManager.RpcEndGame(GameOverReason.ImpostorsBySabotage, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                        return;
                    }
                    HudManager.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverSabotage));
                    criticalSabotage.ClearSabotage();
                }
            }
            bool onlyCrewmates = true;
            Dictionary<ModdedTeam, int> independentTeams = new Dictionary<ModdedTeam, int>();
            List<PlayerControl> neutralKillerCount = new List<PlayerControl>();
            int crewmateCount = 0;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    ModdedTeam team = player.Data.Role.GetTeam();
                    if (team != ModdedTeamManager.Crewmates)
                    {
                        if (team != ModdedTeamManager.Neutrals)
                        {
                            if (independentTeams.TryGetValue(team, out int num))
                            {
                                independentTeams[team] = num + 1;
                            }
                            else
                            {
                                independentTeams[team] = 1;
                            }
                            if (player.Data.Role.CanKill())
                            {
                                onlyCrewmates = false;
                            }
                        }
                        else if (player.Data.Role.CanKill())
                        {
                            neutralKillerCount.Add(player);
                            onlyCrewmates = false;
                        }
                    }
                    else
                    {
                        crewmateCount++;
                    }
                }
            }
            if (!onlyCrewmates && TutorialManager.InstanceExists || !TutorialManager.InstanceExists)
            {
                if (independentTeams.Count <= 0)
                {
                    if (neutralKillerCount.Count <= 0)
                    {
                        if (crewmateCount <= 0)
                        {
                            if (TutorialManager.InstanceExists)
                            {
                                DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorDead));
                                gameManager.ReviveEveryoneFreeplay();
                            }
                            else
                            {
                                gameManager.RpcEndGame<ImpostorDisconnect>();
                            }
                            return;
                        }
                        if (TutorialManager.InstanceExists)
                        {
                            DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorDead));
                            gameManager.ReviveEveryoneFreeplay();
                        }
                        else
                        {
                            gameManager.RpcEndGame<CrewmatesByVote>();
                        }
                    }
                    else if (neutralKillerCount.Count == 1 && crewmateCount <= 1)
                    {
                        NetworkedPlayerInfo data = neutralKillerCount[0].Data;
                        ICustomRole customRole = data.Role.CustomRole();
                        if (customRole != null && customRole.Configuration.NeutralGameOver != null)
                        {
                            gameManager.RpcEndGame(customRole.Configuration.NeutralGameOver);
                            return;
                        }
                        gameManager.RpcEndGame(customRole.Configuration.NeutralGameOver);
                    }
                }
                else if (independentTeams.Count == 1)
                {
                    KeyValuePair<ModdedTeam, int> pair = independentTeams.First();
                    if (neutralKillerCount.Count <= 0 && pair.Value >= crewmateCount)
                    {
                        if (pair.Key == ModdedTeamManager.Impostors)
                        {
                            if (TutorialManager.InstanceExists)
                            {
                                DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorKills));
                                gameManager.ReviveEveryoneFreeplay();
                            }
                            else
                            {
                                switch (GameData.LastDeathReason)
                                {
                                    case DeathReason.Exile:
                                        gameManager.RpcEndGame<ImpostorsByVote>();
                                        break;
                                    case DeathReason.Kill:
                                        gameManager.RpcEndGame<ImpostorsByKill>();
                                        break;
                                    default:
                                        gameManager.RpcEndGame<CrewmateDisconnect>();
                                        break;
                                }
                            }
                            return;
                        }
                        if (TutorialManager.InstanceExists)
                        {
                            HudManager.Instance.ShowPopUp(pair.Key.VictoryText);
                        }
                        else
                        {
                            gameManager.RpcEndGame(pair.Key.DefaultGameOver);
                        }
                    }
                }
            }
            if (onlyCrewmates && TutorialManager.InstanceExists || !TutorialManager.InstanceExists)
            {
                if (TutorialManager.InstanceExists)
                {
                    GameData.Instance.RecomputeTaskCounts();
                    if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks && PlayerControl.LocalPlayer.Data.Role.TasksCountTowardProgress)
                    {
                        DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverTaskWin));
                        ShipStatus.Instance.Begin();
                    }
                }
                else
                {
                    gameManager.CheckEndGameViaTasks();
                }
            }
        }
    }
}
