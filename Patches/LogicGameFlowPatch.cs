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
    public static class LogicGameFlowPatch
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
            if (!GameData.Instance)
            {
                return false;
            }
            Dictionary<ModdedTeam, ChangeableValue<int>> pair = new Dictionary<ModdedTeam, ChangeableValue<int>>();
            int crewmates = 0;
            List<PlayerControl> neutralKillers = new List<PlayerControl>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    ModdedTeam team = player.Data.Role.GetTeam();
                    if (team == ModdedTeam.Crewmates)
                    {
                        crewmates++;
                    }
                    else if (team == ModdedTeam.Neutrals && player.Data.Role.CanKill())
                    {
                        neutralKillers.Add(player);
                    }
                    else
                    {
                        if (!pair.Keys.Contains(team))
                        {
                            pair.Add(team, new ChangeableValue<int>(1));
                        }
                        else
                        {
                            foreach (KeyValuePair<ModdedTeam, ChangeableValue<int>> pair2 in pair)
                            {
                                if (pair2.Key == team)
                                {
                                    pair2.Value.Value++;
                                }
                            }
                        }
                    }
                }
            }
            bool freeplay = AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
            if (pair.Count == 1 && pair.Values.ToArray()[0].Value >= crewmates && neutralKillers.Count == 0)
            {
                if (!freeplay)
                {
                    __instance.Manager.RpcCustomEndGame(pair.Keys.ToArray()[0]);
                }
                __instance.Manager.ReviveEveryoneFreeplay();
            }
            else if (pair.Count == 0 && neutralKillers.Count == 1 && neutralKillers.Count >= crewmates)
            {
                if (!freeplay)
                {
                    __instance.Manager.RpcCustomEndGame(ModdedTeam.Neutrals);
                }
                __instance.Manager.ReviveEveryoneFreeplay();
            }
            else if (pair.Count == 0 && neutralKillers.Count == 0)
            {
                if (!freeplay)
                {
                    __instance.Manager.RpcCustomEndGame(ModdedTeam.Crewmates);
                }
                __instance.Manager.ReviveEveryoneFreeplay();
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
