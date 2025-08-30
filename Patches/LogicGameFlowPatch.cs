using AmongUs.Data;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FungleAPI.Patches
{
    [HarmonyPatch]
    internal static class LogicGameFlowPatch
    {
        internal static List<Type> LogicGameFlowTypes { get; } = (from x in typeof(LogicGameFlow).Assembly.GetTypes() where x.IsSubclassOf(typeof(LogicGameFlow)) select x).ToList<Type>();
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return from x in LogicGameFlowTypes
                   select x.GetMethod("CheckEndCriteria", AccessTools.allDeclared) into m
                   where m != null
                   select m;
        }
        public static bool Prefix(LogicGameFlow __instance)
        {
            if (__instance.Manager.IsHideAndSeek())
            {
                if (!GameData.Instance)
                {
                    return false;
                }
                Il2CppSystem.ValueTuple<int, int, int> playerCounts = __instance.GetPlayerCounts();
                int item = playerCounts.Item1;
                int item2 = playerCounts.Item2;
                if (item2 <= 0 && !DestroyableSingleton<TutorialManager>.InstanceExists)
                {
                    __instance.Manager.RpcEndGame(GameOverReason.ImpostorDisconnect, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                }
                if (item > 0)
                {
                    if (!DestroyableSingleton<TutorialManager>.InstanceExists && __instance.TryCast<LogicGameFlowHnS>() != null && __instance.TryCast<LogicGameFlowHnS>().AllTimersExpired())
                    {
                        __instance.Manager.RpcEndGame(GameOverReason.HideAndSeek_CrewmatesByTimer, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                    }
                    return false;
                }
                if (!DestroyableSingleton<TutorialManager>.InstanceExists)
                {
                    __instance.Manager.RpcEndGame(GameOverReason.HideAndSeek_ImpostorsByKills, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                    return false;
                }
                DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorKills));
                __instance.Manager.ReviveEveryoneFreeplay();
                return false;
            }
            Dictionary<ModdedTeam, ChangeableValue<int>> teamsCount = new Dictionary<ModdedTeam, ChangeableValue<int>>();
            int neutralKillerCount = 0;
            int crewmateCount = 0;
            foreach (NetworkedPlayerInfo player in GameData.Instance.AllPlayers)
            {
                if (!player.IsDead)
                {
                    ModdedTeam team = player.Role.GetTeam();
                    if (team == ModdedTeam.Crewmates)
                    {
                        crewmateCount++;
                    }
                    else if (team == ModdedTeam.Neutrals && player.Role.CanKill())
                    {
                        neutralKillerCount++;
                    }
                    else
                    {
                        if (!teamsCount.ContainsKey(team))
                        {
                            teamsCount.Add(team, new ChangeableValue<int>(1));
                        }
                        else
                        {
                            teamsCount[team].Value++;
                        }
                    }
                }
            }
            if (teamsCount.Count <= 0)
            {
                if (neutralKillerCount == 1 && neutralKillerCount == crewmateCount)
                {
                    __instance.Manager.RpcCustomEndGame(ModdedTeam.Neutrals);
                }
                if (neutralKillerCount <= 0 && !TutorialManager.InstanceExists)
                {
                    __instance.Manager.RpcCustomEndGame(ModdedTeam.Crewmates);
                }
            }
            else if (teamsCount.Count == 1 && neutralKillerCount == 0)
            {
                ModdedTeam team = teamsCount.Keys.ToArray()[0];
                if (teamsCount[team].Value >= crewmateCount)
                {
                    __instance.Manager.RpcCustomEndGame(team);
                }
            }
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.LifeSupp, out systemType))
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.SafeCast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    if (!DestroyableSingleton<TutorialManager>.InstanceExists)
                    {
                        __instance.Manager.RpcCustomEndGame(ModdedTeam.Impostors);
                        return false;
                    }
                    DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverSabotage));
                    lifeSuppSystemType.Countdown = 10000f;
                }
            }
            foreach (ISystemType systemType2 in ShipStatus.Instance.Systems.Values)
            {
                ICriticalSabotage criticalSabotage = systemType2.SafeCast<ICriticalSabotage>();
                if (criticalSabotage != null && criticalSabotage.Countdown < 0f)
                {
                    if (!DestroyableSingleton<TutorialManager>.InstanceExists)
                    {
                        __instance.Manager.RpcCustomEndGame(ModdedTeam.Impostors);
                        return false;
                    }
                    DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverSabotage));
                    criticalSabotage.ClearSabotage();
                }
            }
            return false;
        }
    }
}
