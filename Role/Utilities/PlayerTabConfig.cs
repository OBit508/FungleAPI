using FungleAPI.Utilities;
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
        /// <summary>
        /// Returns the tab name
        /// </summary>
        public virtual string TabName => PlayerControl.LocalPlayer.Data.PlayerName;
        /// <summary>
        /// Appends the showed text on the tab
        /// </summary>
        public virtual void AppendTabText(Il2CppSystem.Text.StringBuilder stringBuilder)
        {
            RoleExtensions.AppendHint(PlayerControl.LocalPlayer.Data.Role, stringBuilder);
        }
        /// <summary>
        /// Color used for the tab name
        /// </summary>
        public virtual Color TabNameColor => Color.white;
    }
}