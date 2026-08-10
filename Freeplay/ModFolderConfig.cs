using AmongUs.Matchmaking;
using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.Components;
using FungleAPI.Freeplay.Helpers;
using FungleAPI.Modifiers;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Freeplay
{
    /// <summary>
    /// Class used to create the mod folder in freeplay
    /// </summary>
    [FungleIgnore]
    public class ModFolderConfig
    {
        private bool Initialized;
        public virtual string FolderName { get; set; }
        public virtual Color FolderColor { get; set; } = new Color(0.937f, 0.811f, 0.592f);
        public List<Folder> SubFolders = new List<Folder>();
        public List<FolderItem> Items = new List<FolderItem>();
        public virtual void Initialize(ModPlugin modPlugin)
        {
            if (Initialized)
            {
                return;
            }
            FolderName = modPlugin.FunglePlugin.ModName;
            foreach (KeyValuePair<ModdedTeam, List<RoleBehaviour>> teams in modPlugin.GetTeamsAndRoles())
            {
                teams.Value.RemoveAll(r => r.CustomRole() == null && RoleManager.IsGhostRole(r.Role) || r.CustomRole() != null && r.CustomRole().Configuration.HideInFreeplay);
                if (teams.Value.Count > 0)
                {
                    if (teams.Key.HideInFreeplay) continue;

                    Folder teamFolder = new Folder() { FolderName = teams.Key.TeamName.GetString(), FolderColor = teams.Key.TeamColor };
                    foreach (RoleBehaviour roleBehaviour in teams.Value)
                    {
                        teamFolder.Items.Add(new FolderItem()
                        {
                            Name = $"Be_{roleBehaviour.NiceName}.exe",
                            Color = roleBehaviour.TeamColor,
                            OnClick = delegate { PlayerControl.LocalPlayer?.RpcSetRole(roleBehaviour.Role);  },
                            Overlay = () => PlayerControl.LocalPlayer.Data.RoleType == roleBehaviour.Role
                        });
                    }
                    SubFolders.Add(teamFolder);
                }
            }
            if (modPlugin.Modifiers.Count > 0)
            {
                Folder modifierFolder = new Folder() { FolderName = FungleTranslation.ModifiersText.GetString(), FolderColor = Color.blue };
                foreach (BaseModifier baseModifier in modPlugin.Modifiers)
                {
                    Func<bool> factory = () => ModifierHolder.LocalPlayer != null && ModifierHolder.LocalPlayer.Modifiers.ContainsKey(baseModifier.ModifierId);
                    modifierFolder.Items.Add(new FolderItem()
                    {
                        Name = baseModifier.ModifierName.GetString(),
                        Color = baseModifier.ModifierColor,
                        OnClick = delegate 
                        {
                            if (factory())
                            {
                                PlayerControl.LocalPlayer?.RpcRemoveModifier(baseModifier.ModifierId);
                                return;
                            }
                            PlayerControl.LocalPlayer?.RpcAddModifier(baseModifier.ModifierId);
                        },
                        Overlay = factory
                    });
                }
                if (modifierFolder.Items.Count > 0)
                {
                    SubFolders.Add(modifierFolder);
                }
            }
            Initialized = true;
        }
    }
}
