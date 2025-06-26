using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using HarmonyLib;
using LibCpp2IL.Elf;
using Steamworks;
using UnityEngine;
using UnityEngine.UIElements.UIR;

namespace FungleAPI.Roles
{
    [HarmonyPatch(typeof(IntroCutscene))]
    internal class Intro
    {
        [HarmonyPrefix]
        [HarmonyPatch("BeginCrewmate")]
        public static bool BeginCrewmatePatch(IntroCutscene __instance)
        {
            ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
            if (customRole != null && customRole.Team != ModdedTeam.Crewmates)
            {
                Transform transform = __instance.BackgroundBar.transform;
                Vector3 position = transform.position;
                position.y -= 0.25f;
                transform.position = position;
                __instance.BackgroundBar.material.color = customRole.Team.TeamColor;
                __instance.TeamTitle.text = customRole.Team.TeamName;
                __instance.impostorScale = 1f;
                __instance.ImpostorText.text = null;
                __instance.TeamTitle.color = customRole.Team.TeamColor;
                __instance.ourCrewmate = __instance.CreatePlayer(0, Mathf.CeilToInt(7.5f), PlayerControl.LocalPlayer.Data, false);
                int i = 1;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer && player.Data.Role.GetTeam() == customRole.Team)
                    {
                        __instance.CreatePlayer(i, Mathf.CeilToInt(7.5f), player.Data, false);
                        i++;
                    }
                }
            }
            return true;
        }
    }
}
