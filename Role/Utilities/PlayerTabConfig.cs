using FungleAPI.Utilities;
using Il2CppSystem.Reflection.Metadata.Ecma335;
using Il2CppSystem.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Utilities
{
    /// <summary>
    /// Configuration class for a Mira API role tab
    /// </summary>
    public class PlayerTabConfig
    {
        public static PlayerTabConfig Default { get; } = new PlayerTabConfig();
        public PlayerTabConfig()
            : this(out _) { }
        public PlayerTabConfig(out PlayerTabConfig playerTabConfig)
        {
            playerTabConfig = this;
            TabName = () => $"{Color.white.ToTextColor()}{PlayerControl.LocalPlayer.Data.PlayerName}</color>";
            AppendTabText = delegate (Il2CppSystem.Text.StringBuilder stringBuilder) { RoleExtensions.AppendHint(PlayerControl.LocalPlayer.Data.Role, stringBuilder); };
        }
        /// <summary>
        /// Returns the tab name
        /// </summary>
        public Func<string> TabName;
        /// <summary>
        /// Appends the showed text on the tab
        /// </summary>
        public Action<Il2CppSystem.Text.StringBuilder> AppendTabText;
    }
}