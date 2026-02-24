using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// Configuration class for a Mira API role tab
    /// </summary>
    public class MiraRoleTabConfig
    {
        internal MiraRoleTabConfig() { }
        public MiraRoleTabConfig(ICustomRole role)
        {
            __stringName = role.RoleName;
            TabNameColor = role.RoleColor;
        }
        /// <summary>
        /// Gets the tab name text based on the string source
        /// </summary>
        public string TabNameText => __stringName == StringNames.None ? __text : __stringName.GetString();
        /// <summary>
        /// Custom text used when no StringNames value is provided
        /// </summary>
        public string __text;
        /// <summary>
        /// StringNames identifier for localized tab name
        /// </summary>
        public StringNames __stringName;
        /// <summary>
        /// Color used for the tab name
        /// </summary>
        public Color TabNameColor;
    }
}