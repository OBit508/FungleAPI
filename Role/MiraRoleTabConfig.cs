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
    /// 
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
        /// 
        /// </summary>
        public string TabNameText => __stringName == StringNames.None ? __text : __stringName.GetString();
        /// <summary>
        /// 
        /// </summary>
        public string __text;
        /// <summary>
        /// 
        /// </summary>
        public StringNames __stringName;
        /// <summary>
        /// 
        /// </summary>
        public Color TabNameColor;
    }
}
