using AmongUs.GameOptions;
using FungleAPI.Extensions;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GModes
{
    public class NormalGameMode : BaseGameMode
    {
        public override StringNames GameModeName => StringNames.GameTypeClassic;
        public override bool CanUse(IUsable usable, PlayerControl player) => true;
        public override void OnPlayerDeath(PlayerControl player, bool assignGhostRole)
        {
            if (AmongUsClient.Instance.AmHost && assignGhostRole)
            {
                RoleManager.Instance.AssignRoleOnDeath(player, true);
            }
        }
        public override int GetVotingTime()
        {
            if (GameOptions.TryGetInt(Int32OptionNames.VotingTime, out int votingTime))
            {
                return votingTime;
            }
            return 120;
        }
        public override int GetDiscussionTime()
        {
            if (GameOptions.TryGetInt(Int32OptionNames.DiscussionTime, out int discussionTime))
            {
                return discussionTime;
            }
            return 15;
        }
        public override bool GetGhostsDoTasks()
        {
            bool flag;
            return GameOptions.TryGetBool(BoolOptionNames.GhostsDoTasks, out flag) && flag;
        }
        public override float GetKillCooldown()
        {
            float num;
            if (!GameOptions.TryGetFloat(FloatOptionNames.KillCooldown, out num))
            {
                return 1f;
            }
            return num;
        }
        public override float GetKillDistance()
        {
            float[] floatArray = GameOptions.GetFloatArray(FloatArrayOptionNames.KillDistances);
            int @int = GameOptions.GetInt(Int32OptionNames.KillDistance);
            return floatArray[Mathf.Clamp(@int, 0, floatArray.Length - 1)];
        }
        public override float GetPlayerSpeedMod(PlayerControl pc)
        {
            float num;
            if (!GameOptions.TryGetFloat(FloatOptionNames.PlayerSpeedMod, out num))
            {
                return 1f;
            }
            return num;
        }
        public override bool GetConfirmImpostor()
        {
            bool flag;
            return GameOptions.TryGetBool(BoolOptionNames.ConfirmImpostor, out flag) && flag;
        }
        public override int GetEmergencyCooldown()
        {
            int num;
            if (!GameOptions.TryGetInt(Int32OptionNames.EmergencyCooldown, out num))
            {
                return 1;
            }
            return num;
        }
        public override int GetNumEmergencyMeetings()
        {
            int num;
            if (!GameOptions.TryGetInt(Int32OptionNames.NumEmergencyMeetings, out num))
            {
                return 0;
            }
            return num;
        }
        public override bool GetVisualTasks()
        {
            bool flag;
            return GameOptions.TryGetBool(BoolOptionNames.VisualTasks, out flag) && flag;
        }
        public override bool GetAnonymousVotes()
        {
            bool flag;
            return GameOptions.TryGetBool(BoolOptionNames.AnonymousVotes, out flag) && flag;
        }
        public override TaskBarMode GetTaskBarMode()
        {
            int num;
            if (!GameOptions.TryGetInt(Int32OptionNames.TaskBarMode, out num))
            {
                return TaskBarMode.Normal;
            }
            return (TaskBarMode)num;
        }
        public override float GetEngineerCooldown()
        {
            return Manager.LogicOptions.GetRoleFloat(FloatOptionNames.EngineerCooldown);
        }
        public override float GetEngineerInVentTime()
        {
            return Manager.LogicOptions.GetRoleFloat(FloatOptionNames.EngineerInVentMaxTime);
        }
        public override MapOptions GetMapOptions()
        {
            return new MapOptions
            {
                Mode = (PlayerControl.LocalPlayer.Data.Role.CanSabotage() && !MeetingHud.Instance) ? MapOptions.Modes.Sabotage : MapOptions.Modes.Normal
            };
        }
        public override DeadBody GetDeadBody(GameManager gameManager, RoleBehaviour impostorRole)
        {
            return gameManager.deadBodyPrefab[impostorRole.GetCreatedDeadBody() == DeadBodyType.Viper ? 1 : 0];
        }
        public override float CanUseVent(Vent vent, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
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
        public override void SelectRoles(RoleManager roleManager)
        {
            if (Manager.LogicRoleSelection.Is(out LogicRoleSelectionNormal logicRoleSelectionNormal))
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
                return;
            }
        }
        public void AssignRolesForTeam(LogicRoleSelectionNormal logicRoleSelectionNormal, Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players, ModdedTeam team)
        {
            int rolesAssigned = 0;
            IEnumerable<RoleBehaviour> availableRoles = RoleManager.Instance.AllRoles.ToSystemList().Where(role => role.GetTeam() == team && !RoleManager.IsGhostRole(role.Role));

            IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;

            Il2CppSystem.Collections.Generic.List<RoleTypes> roleList = new Il2CppSystem.Collections.Generic.List<RoleTypes>();

            IEnumerable<RoleManager.RoleAssignmentData> guaranteedRoles = availableRoles.Where(role => roleOptions.GetChancePerGame(role.Role) == 100).Select(role => new RoleManager.RoleAssignmentData(role, roleOptions.GetNumPerGame(role.Role), 100));

            foreach (RoleManager.RoleAssignmentData assignment in guaranteedRoles)
            {
                for (int i = 0; i < assignment.Count; i++)
                {
                    roleList.Add(assignment.Role.Role);
                }
            }

            logicRoleSelectionNormal.AssignRolesFromList(players, (int)team.MaxCount, roleList, ref rolesAssigned);

            List<RoleManager.RoleAssignmentData> randomRoles = availableRoles.Select(role => new { Role = role, Chance = roleOptions.GetChancePerGame(role.Role) }).Where(x => x.Chance > 0 && x.Chance < 100).Select(x => new RoleManager.RoleAssignmentData(x.Role, roleOptions.GetNumPerGame(x.Role.Role), x.Chance)).ToList();

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
        public override void AssignTasks(ShipStatus shipStatus)
        {
            shipStatus.numScans = 0;
            shipStatus.AssignTaskIndexes();

            IGameOptions options = GameOptionsManager.Instance.CurrentGameOptions;
            Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players = GameData.Instance.AllPlayers;
            Il2CppSystem.Collections.Generic.HashSet<TaskTypes> used = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();
            Il2CppSystem.Collections.Generic.List<byte> tasks = new Il2CppSystem.Collections.Generic.List<byte>(10);
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> common = shipStatus.CommonTasks.ToList().ToIl2CppList();
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> longTasks = shipStatus.LongTasks.ToList().ToIl2CppList();
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> shortTasks = shipStatus.ShortTasks.ToList().ToIl2CppList();

            common.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle();
            longTasks.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle(0);
            shortTasks.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle(0);
            common.ToSystemList().ForEach(t => t.Length = NormalPlayerTask.TaskLength.Common);
            longTasks.ToSystemList().ForEach(t => t.Length = NormalPlayerTask.TaskLength.Long);
            shortTasks.ToSystemList().ForEach(t => t.Length = NormalPlayerTask.TaskLength.Short);

            int commonCount = options.GetInt(Int32OptionNames.NumCommonTasks);
            int longCount = options.GetInt(Int32OptionNames.NumLongTasks);
            int shortCount = options.GetInt(Int32OptionNames.NumShortTasks);

            if (commonCount + longCount + shortCount == 0)
            {
                shortCount = 1;
            }

            int commonIdx = 0;
            int longIdx = 0;
            int shortIdx = 0;

            shipStatus.AddTasksFromList(ref commonIdx, commonCount, tasks, used, common);

            for (int i = 0; i < commonCount && common.Count > 0; i++)
            {
                int idx = common.ToArray().RandomIdx();
                tasks.Add((byte)common[idx].Index);
                common.RemoveAt(idx);
            }

            for (byte i = 0; i < players.Count; i++)
            {
                used.Clear();
                tasks.RemoveRange(commonCount, tasks.Count - commonCount);

                shipStatus.AddTasksFromList(ref longIdx, longCount, tasks, used, longTasks);
                shipStatus.AddTasksFromList(ref shortIdx, shortCount, tasks, used, shortTasks);

                NetworkedPlayerInfo player = players[i];
                if (player.Object && !player.Object.GetComponent<DummyBehaviour>().enabled)
                {
                    player.RpcSetTasks((byte[])tasks.ToArray());
                }
            }
        }
        public override void CheckEndCriteria()
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
                        Manager.RpcEndGame<ImpostorsBySabotage>();
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
                        Manager.RpcEndGame<ImpostorsBySabotage>();
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
                                Manager.ReviveEveryoneFreeplay();
                            }
                            else
                            {
                                Manager.RpcEndGame<ImpostorDisconnect>();
                            }
                            return;
                        }
                        if (TutorialManager.InstanceExists)
                        {
                            DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorDead));
                            Manager.ReviveEveryoneFreeplay();
                        }
                        else
                        {
                            Manager.RpcEndGame<CrewmatesByVote>();
                        }
                    }
                    else if (neutralKillerCount.Count == 1 && crewmateCount <= 1)
                    {
                        PlayerControl winner = neutralKillerCount[0];
                        ICustomRole customRole = winner.Data.Role.CustomRole();
                        if (customRole != null && customRole.Configuration.CallGameOverAsNeutral != null)
                        {
                            customRole.Configuration.CallGameOverAsNeutral(winner);
                            return;
                        }

                        GameManager.Instance?.RpcEndGame<NeutralGameOver, PlayerControl>(winner);
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
                                Manager.ReviveEveryoneFreeplay();
                            }
                            else
                            {
                                switch (GameData.LastDeathReason)
                                {
                                    case DeathReason.Exile:
                                        Manager.RpcEndGame<ImpostorsByVote>();
                                        break;
                                    case DeathReason.Kill:
                                        Manager.RpcEndGame<ImpostorsByKill>();
                                        break;
                                    default:
                                        Manager.RpcEndGame<CrewmateDisconnect>();
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
                            Manager.RpcEndGame(pair.Key.DefaultGameOver);
                        }
                    }
                }
            }

            if (TutorialManager.InstanceExists)
            {
                GameData.Instance.RecomputeTaskCounts();
                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks && PlayerControl.LocalPlayer.Data.Role.TasksCountTowardProgress)
                {
                    HudManager.Instance.ShowPopUp(TranslationController.Instance.GetString(StringNames.GameOverTaskWin));
                    ShipStatus.Instance.Begin();
                }
            }
            else
            {
                Manager.CheckEndGameViaTasks();
            }
        }
    }
}
