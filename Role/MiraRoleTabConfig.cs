using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Utilities;
using UnityEngine;

namespace FungleAPI.Role
{
    public class MiraRoleTabConfig
    {
        public MiraRoleTabConfig(ICustomRole role)
        {
            __stringName = role.RoleName;
            TabNameColor = role.RoleColor;
        }
        internal MiraRoleTabConfig() { }
        public string __text;
        public StringNames __stringName;
        public Color TabNameColor;
        public string TabNameText
        {
            get
            {
                if (__stringName == StringNames.None)
                {
                    return __text;
                }
                return __stringName.GetString();
            }
        }
    }
}
