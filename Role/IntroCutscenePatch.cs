using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Role.Teams;
using HarmonyLib;
using LibCpp2IL.Elf;
using Steamworks;
using Unity.IL2CPP.Metadata;
using UnityEngine;
using UnityEngine.UIElements.UIR;
using BepInEx.Unity.IL2CPP.Utils;

namespace FungleAPI.Roles
{
    [HarmonyPatch(typeof(IntroCutscene))]
    internal class IntroCutscenePatch
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
            ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
            if (customRole != null && customRole.Team != ModdedTeam.Crewmates)
            {
                Transform transform = __instance.BackgroundBar.transform;
                Vector3 position = transform.position;
                position.y -= 0.25f;
                transform.position = position;
                __instance.BackgroundBar.material.color = customRole.Team.TeamColor;
                __instance.TeamTitle.text = customRole.Team.TeamName.GetString();
                __instance.impostorScale = 1f;
                __instance.ImpostorText.text = null;
                __instance.TeamTitle.color = customRole.Team.TeamColor;
            }
        }
        public static System.Collections.IEnumerator ShowCustomRole(IntroCutscene intro)
        {
            while (true)
            {
                ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
                if (customRole != null && !customRole.RoleB.ShowTeamColorOnIntro)
                {
                    intro.YouAreText.color = customRole.RoleColor;
                    intro.RoleBlurbText.color = customRole.RoleColor;
                    intro.RoleText.color = customRole.RoleColor;
                }
            }
        }
        [HarmonyPatch("ShowRole")]
        [HarmonyPostfix]
        public static void OnShowRole(IntroCutscene __instance)
        {
            
        }
    }
}
