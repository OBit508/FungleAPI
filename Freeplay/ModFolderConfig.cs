using AmongUs.Matchmaking;
using FungleAPI.Attributes;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Freeplay.Helpers;
using FungleAPI.PluginLoading;
using FungleAPI.Teams;
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
            FolderName = modPlugin.ModName;
            foreach (KeyValuePair<ModdedTeam, List<RoleBehaviour>> teams in modPlugin.GetTeamsAndRoles())
            {
                Folder teamFolder = new Folder() { FolderName = teams.Key.TeamName.GetString(), FolderColor = teams.Key.TeamColor };
                foreach (RoleBehaviour roleBehaviour in teams.Value)
                {
                    teamFolder.Items.Add(new FolderItem()
                    {
                        Name = $"Be_{roleBehaviour.NiceName}.exe",
                        Color = roleBehaviour.TeamColor,
                        OnClick = delegate { PlayerControl.LocalPlayer?.RpcSetRole(roleBehaviour.Role); }
                    });
                }
                SubFolders.Add(teamFolder);
            }
            Initialized = true;
        }
    }
}
