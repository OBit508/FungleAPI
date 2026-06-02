using BepInEx.Core.Logging.Interpolation;
using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Patches;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using LibCpp2IL.Elf;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.IL2CPP.Metadata;
using UnityEngine;
using UnityEngine.UIElements.UIR;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(IntroCutscene))]
    internal static class IntroCutscenePatch
    {
        [HarmonyPatch("BeginCrewmate")]
        [HarmonyPrefix]
        public static bool BeginCrewmatePatch(IntroCutscene __instance)
        {
            ModdedTeam team = PlayerControl.LocalPlayer.Data.Role.GetTeam();
            if (team != ModdedTeamManager.Crewmates)
            {
                Il2CppSystem.Collections.Generic.List<PlayerControl> list = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                list.Add(PlayerControl.LocalPlayer);
                if (team.KnowMembers)
                {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        if (player != PlayerControl.LocalPlayer && player.Data.Role.GetTeam() == team)
                        {
                            list.Add(player);
                        }
                    }
                }
                __instance.BeginImpostor(list);
                return false;
            }
            return true;
        }
        [HarmonyPatch("BeginImpostor")]
        [HarmonyPatch("BeginCrewmate")]
        [HarmonyPostfix]
        public static void BeginCustom(IntroCutscene __instance)
        {
            if (EventManager.CallEvent(new BeforeIntroBeginEvent(__instance)).Cancelled) return; 

            ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (customRole != null && customRole.Team != ModdedTeamManager.Crewmates)
            {
                Transform transform = __instance.BackgroundBar.transform;
                Vector3 position = transform.position;
                position.y -= 0.25f;
                transform.position = position;
                __instance.BackgroundBar.material.color = customRole.Team.TeamColor;
                __instance.TeamTitle.text = customRole.Team.TeamName.GetString();
                __instance.impostorScale = 1f;
                __instance.TeamTitle.color = customRole.Team.TeamColor;
            }
            __instance.ImpostorText.gameObject.SetActive(true);

            string teamsText = "";

            Dictionary<ModdedTeam, int> teams = new Dictionary<ModdedTeam, int>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    ModdedTeam team = player.Data.Role.GetTeam();
                    if (teams.TryGetValue(team, out int value))
                    {
                        teams[team] = value + 1;
                    }
                    else
                    {
                        teams[team] = 0;
                    }
                }
            }

            ModdedTeam last = teams.Last().Key;

            foreach (KeyValuePair<ModdedTeam, int> pair in teams)
            {
                if (pair.Value > 1)
                {
                    teamsText += $"{pair.Value} {pair.Key.TeamColor.ToTextColor()}{pair.Key.PluralName.GetString()}</color>";
                }
                else
                {
                    teamsText += $"1 {pair.Key.TeamColor.ToTextColor()}{pair.Key.TeamName.GetString()}</color>";
                }

                if (pair.Key != last)
                {
                    teamsText += ", ";
                }
                else
                {
                    teamsText += ".";
                }
            }

            __instance.ImpostorText.text = string.Format(FungleTranslation.TeamsRemainText.GetString(), teamsText);

            EventManager.CallEvent(new AfterIntroBeginEvent(__instance));
        }
    }
}
