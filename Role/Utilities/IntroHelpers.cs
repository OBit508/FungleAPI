using FungleAPI.Api;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Sentry.MeasurementUnit;

namespace FungleAPI.Role.Utilities
{
    public static class IntroHelpers
    {
        public static System.Collections.IEnumerator ShowTeam(IntroCutscene introCutscene, float duration)
        {
            if (introCutscene.overlayHandle == null)
            {
                introCutscene.overlayHandle = DestroyableSingleton<DualshockLightManager>.Instance.AllocateLight();
            }
            yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();

            ModdedTeam moddedTeam = PlayerControl.LocalPlayer.Data.Role.GetTeam();

            moddedTeam.BeginIntro(introCutscene);
            introCutscene.overlayHandle.color = moddedTeam.TeamColor;

            Color c = introCutscene.TeamTitle.color;
            Color fade = Color.black;
            Color impColor = Color.white;
            Vector3 titlePos = introCutscene.TeamTitle.transform.localPosition;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float num = Mathf.Min(1f, timer / duration);
                introCutscene.Foreground.material.SetFloat("_Rad", introCutscene.ForegroundRadius.ExpOutLerp(num * 2f));
                fade.a = Mathf.Lerp(1f, 0f, num * 3f);
                introCutscene.FrontMost.color = fade;
                c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
                introCutscene.TeamTitle.color = c;
                introCutscene.RoleText.color = c;
                impColor.a = Mathf.Lerp(0f, 1f, (num - 0.3f) * 3f);
                introCutscene.ImpostorText.color = impColor;
                titlePos.y = 2.7f - num * 0.3f;
                introCutscene.TeamTitle.transform.localPosition = titlePos;

                Color o = introCutscene.overlayHandle.color;
                o.a = Mathf.Min(1f, timer * 2f);
                introCutscene.overlayHandle.color = o;
                yield return null;
            }
            timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                float num2 = timer / 1f;
                fade.a = Mathf.Lerp(0f, 1f, num2 * 3f);
                introCutscene.FrontMost.color = fade;
                Color o = introCutscene.overlayHandle.color;
                o.a = 1f - fade.a;
                introCutscene.overlayHandle.color = o;
                yield return null;
            }
        }
        public static string GetDefaultTeamsText()
        {
            string teamsText = "";

            Dictionary<ModdedTeam, int> teams = new Dictionary<ModdedTeam, int>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                ModdedTeam team = player.Data.Role.GetTeam();
                if (teams.TryGetValue(team, out int value))
                {
                    teams[team] = value + 1;
                }
                else
                {
                    teams[team] = 1;
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

            return string.Format(FungleTranslation.TeamsRemainText.GetString(), teamsText);
        }
        public static string GetTeamsTextAsCrewmate()
        {
            string teamsText = "";

            Dictionary<ModdedTeam, int> teams = new Dictionary<ModdedTeam, int>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                ModdedTeam team = player.Data.Role.GetTeam();

                if (team == ModdedTeamManager.Crewmates) continue;

                if (teams.TryGetValue(team, out int value))
                {
                    teams[team] = value + 1;
                }
                else
                {
                    teams[team] = 1;
                }
            }

            if (teams.Count <= 0)
            {
                return "No impostors among us";
            }

            ModdedTeam last = teams.Last().Key;

            for (int i = 0; i < teams.Count; i++)
            {
                KeyValuePair<ModdedTeam, int> pair = teams.ElementAt(i);

                if (pair.Value > 1)
                {
                    teamsText += $"{pair.Key.TeamColor.ToTextColor()}{pair.Value} {pair.Key.PluralName.GetString()}</color>";
                }
                else
                {
                    teamsText += $"{pair.Key.TeamColor.ToTextColor()}1 {pair.Key.TeamName.GetString()}</color>";
                }

                if (pair.Key != last)
                {
                    if (teams.Count > 1 && i == teams.Count - 2)
                    {
                        teamsText += FungleTranslation.AndText.GetString();

                        continue;
                    }

                    teamsText += ", ";
                }
            }

            return string.Format(FungleTranslation.TeamsAmongUs.GetString(), teamsText);
        }
    }
}
