using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Api;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOptions.Lobby;
using FungleAPI.Role.Utilities;
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
        string ModName => PluginInfo?.Metadata?.Name ?? GetType().Assembly.GetName().Name ?? "Unknown Mod";
        string ModVersion => PluginInfo?.Metadata?.Version.Clean() ?? GetType().Assembly.GetName().Version?.ToString() ?? "0.0.0";
        bool UseAutoRegistration => true;
        bool RequiredOnAllClients => true;
        PluginCredits? Credits => new PluginCredits()
        {
            Name = ModName,
            Version = ModVersion
        };

        void ShowCreditsScreen() { }
        void AlmostLoaded() { }
        void FullyLoaded() { }
        List<LobbyTab> LoadTabs(ModPlugin modPlugin) 
        {
            List<LobbyTab> lobbyTabs = new List<LobbyTab>() { new GamemodeSettingsTab() { TabAssembly = modPlugin.ModAssembly } };
            if (modPlugin.Settings.Groups.Count > 0)
            {
                lobbyTabs.Add(new RoomSettingsTab() { TabAssembly = modPlugin.ModAssembly });
            }
            if (modPlugin.Teams.Count > 0)
            {
                lobbyTabs.Add(new TeamTab() { TabAssembly = modPlugin.ModAssembly });
            }
            if (modPlugin.Modifiers.Count > 0)
            {
                lobbyTabs.Add(new ModifierTab() { TabAssembly = modPlugin.ModAssembly });
            }
            if (modPlugin.Roles.FindAll(r => !r.CustomRole().Configuration.HideInLobby).Count > 0)
            {
                lobbyTabs.Add(new RoleTab() { TabAssembly = modPlugin.ModAssembly });
            }
            return lobbyTabs;
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
