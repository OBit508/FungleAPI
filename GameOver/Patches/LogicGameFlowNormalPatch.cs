using AmongUs.Data;
using FungleAPI.GameOver.Ends;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
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
        public static bool Prefix(LogicGameFlowNormal __instance)
        {
            if (!GameData.Instance)
            {
                return false;
            }
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.LifeSupp, out systemType))
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.SafeCast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    __instance.EndGameForSabotage();
                    lifeSuppSystemType.Countdown = 10000f;
                }
            }
            foreach (ISystemType systemType2 in ShipStatus.Instance.Systems.Values)
            {
                ICriticalSabotage criticalSabotage = systemType2.SafeCast<ICriticalSabotage>();
                if (criticalSabotage != null && criticalSabotage.Countdown < 0f)
                {
                    __instance.EndGameForSabotage();
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
                            onlyCrewmates = false;
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
                            return false;
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
                        string winText = "Victory of the " + data.Role.NiceName;
                        if (data.Role.CustomRole() != null)
                        {
                            winText = data.Role.CustomRole().NeutralWinText;
                        }
                        if (TutorialManager.InstanceExists)
                        {
                            DestroyableSingleton<HudManager>.Instance.ShowPopUp(winText);
                            gameManager.ReviveEveryoneFreeplay();
                        }
                        else
                        {
                            gameManager.RpcEndGame(new List<NetworkedPlayerInfo>() { data }, winText, data.Role.NameColor, data.Role.NameColor);
                        }
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
                            return false;
                        }
                        if (TutorialManager.InstanceExists)
                        {
                            HudManager.Instance.ShowPopUp(pair.Key.VictoryText);
                        }
                        else
                        {
                            gameManager.RpcEndGame(pair.Key);
                        }
                    }
                }
            }
            if (onlyCrewmates && TutorialManager.InstanceExists || !TutorialManager.InstanceExists)
            {
                if (TutorialManager.InstanceExists)
                {
                    GameData.Instance.RecomputeTaskCounts();
                    if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
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
            return false;
        }
    }
}
