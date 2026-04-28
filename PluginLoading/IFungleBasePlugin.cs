using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.PluginLoading
{
    public interface IFungleBasePlugin
    {
        PluginInfo PluginInfo => ModPluginManager.TryGetPluginInfo(this as BasePlugin);
        string ModName => PluginInfo.Metadata.Name;
        string ModVersion => PluginInfo.Metadata.Version.Clean();
        string ModCredits => $"[{ModName} v{ModVersion}]";
        void ClickOnModName() { }
        void OnRegisterInFungleAPI() { }

        void LoadTabs(ModPlugin modPlugin) 
        {
            if (modPlugin.Settings.Groups.Count > 0)
            {
                modPlugin.LobbyTabs.Add(new GameSettingsTab() { Plugin = modPlugin });
            }
            if (modPlugin.Teams.Count > 0)
            {
                modPlugin.LobbyTabs.Add(new TeamTab() { Plugin = modPlugin });
            }
            if (modPlugin.HasRoles)
            {
                modPlugin.LobbyTabs.Add(new RoleTab() { Plugin = modPlugin });
            }
        }

        void SetPreset(RulesPresets preset, ModPlugin modPlugin)
        {
            bool amHost = AmongUsClient.Instance.AmHost;

            foreach (OptionCollection optionCollection in modPlugin.OptionCollections)
            {
                foreach (IModdedOption moddedOption in optionCollection.Options.Values)
                {
                    moddedOption.SetValue(moddedOption.DefaultValue, amHost);
                }

                if (optionCollection is TeamOptionCollection teamOptionCollection)
                {
                    int count = Mathf.Clamp(teamOptionCollection.Team.DefaultCount, 0, 1000);
                    int priority = teamOptionCollection.Team.GetType() == typeof(CrewmateTeam) ? -1 : Mathf.Clamp(teamOptionCollection.Team.DefaultPriority, 0, 1000);

                    if (amHost)
                    {
                        teamOptionCollection.LocalTeamCount = count;
                        teamOptionCollection.LocalTeamPriority = priority;
                    }
                    else
                    {
                        teamOptionCollection.NonHostTeamCount = count;
                        teamOptionCollection.NonHostTeamPriority = priority;
                    }
                }

                if (preset == RulesPresets.Standard && optionCollection is RoleOptionCollection roleOptionCollection)
                {
                    if (amHost)
                    {
                        roleOptionCollection.SetLocal(0, 0);
                        continue;
                    }
                    roleOptionCollection.NonHostRoleCount = 0;
                    roleOptionCollection.NonHostRoleChance = 0;
                }
            }

            if (amHost)
            {
                modPlugin.RulePreset.Value = (byte)preset;
            }
        }

        System.Collections.IEnumerator CoLoadOnMainScreen(TextMeshPro loadingText)
        {
            yield return null;
        }
    }
}
