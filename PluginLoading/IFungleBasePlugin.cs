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
        bool ApperOnCredits => false;
        void ShowCreditsScreen() { }
        void AlmostLoaded() { }
        void FullyLoaded() { }
        void LoadTabs(ModPlugin modPlugin) 
        {
            if (modPlugin.Settings != null)
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
                optionCollection.SetAsDefault(amHost);
            }

            if (amHost)
            {
                modPlugin.RulePreset.Value = (byte)preset;
            }
        }
        System.Collections.IEnumerator CoLoadAssets(TextMeshPro loadingText)
        {
            yield return null;
        }
    }
}
