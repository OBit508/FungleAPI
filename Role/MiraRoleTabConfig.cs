using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
{
    public class MiraRoleTabConfig
    {
        internal MiraRoleTabConfig() { }
        public MiraRoleTabConfig(ICustomRole role)
        {
            __stringName = role.RoleName;
            TabNameColor = role.RoleColor;
        }
        public string TabNameText => __stringName == StringNames.None ? __text : __stringName.GetString();
        public string __text;
        public StringNames __stringName;
        public Color TabNameColor;
    }
}
