using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using LibCpp2IL.Elf;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (team != ModdedTeam.Crewmates)
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
            ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role.CustomRole();
            if (customRole != null && customRole.Team != ModdedTeam.Crewmates)
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
            __instance.ImpostorText.text = ExileControllerPatch.TeamsRemainText.GetString();
            Dictionary<ModdedTeam, ChangeableValue<int>> teams = new Dictionary<ModdedTeam, ChangeableValue<int>>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead)
                {
                    ModdedTeam team = player.Data.Role.GetTeam();
                    if (teams.ContainsKey(team))
                    {
                        teams[team].Value++;
                    }
                    else
                    {
                        teams.Add(team, new ChangeableValue<int>(1));
                    }
                }
            }
            foreach (KeyValuePair<ModdedTeam, ChangeableValue<int>> pair in teams)
            {
                __instance.ImpostorText.text += pair.Value.Value.ToString() + " " + pair.Key.TeamColor.ToTextColor() + (pair.Value.Value == 1 ? pair.Key.TeamName.GetString() : pair.Key.PluralName.GetString()) + "</color>" + (pair.Key == teams.Last().Key ? "" : ", ");
            }
        }
    }
}
