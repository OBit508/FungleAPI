using AmongUs.Data;
using AsmResolver.PE.DotNet.ReadyToRun;
using FungleAPI.GameOver.Ends;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOver.Patches
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), "CheckEndCriteria")]
    internal static class LogicGameFlowNormalPatch
    {
        public static bool Prefix()
        {
            CheckEndCriteria();
            return false;
        }
        public static void CheckEndCriteria()
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
                        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorsBySabotage, !DataManager.Player.Ads.HasPurchasedAdRemoval);
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
                        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorsBySabotage, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                        return;
                    }
                    HudManager.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverSabotage));
                    criticalSabotage.ClearSabotage();
                }
            }
            bool onlyCrewmates = true;
            Dictionary<ModdedTeam, ChangeableValue<int>> independentTeams = new Dictionary<ModdedTeam, ChangeableValue<int>>();
            List<PlayerControl> neutralKillerCount = new List<PlayerControl>();
            int crewmateCount = 0;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    ModdedTeam team = player.Data.Role.GetTeam();
                    if (team != ModdedTeam.Crewmates)
                    {
                        if (team != ModdedTeam.Neutrals)
                        {
                            if (independentTeams.ContainsKey(team))
                            {
                                independentTeams[team].Value++;
                            }
                            else
                            {
                                independentTeams.Add(team, new ChangeableValue<int>(1));
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
            GameManager gameManager = GameManager.Instance;
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
                        if (customRole != null && customRole.NeutralGameOver != null)
                        {
                            gameManager.RpcEndGame(customRole.NeutralGameOver);
                            return;
                        }
                        gameManager.RpcEndGame(customRole.NeutralGameOver);
                    }
                }
                else if (independentTeams.Count == 1)
                {
                    KeyValuePair<ModdedTeam, ChangeableValue<int>> pair = independentTeams.First();
                    if (neutralKillerCount.Count <= 0 && pair.Value.Value >= crewmateCount)
                    {
                        if (pair.Key == ModdedTeam.Impostors)
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
            return;
        }
    }
}
