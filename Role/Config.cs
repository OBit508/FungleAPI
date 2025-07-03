using BepInEx.Configuration;
using FungleAPI.LoadMod;
using FungleAPI.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Roles
{
    public class Config
    {
        internal Config()
        {
        }
        public string ConfigName;
        public int ConfigId;
        public RoleBehaviour Role;
    }
}
