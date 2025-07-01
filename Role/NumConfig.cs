using BepInEx.Configuration;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role
{
    public class NumConfig : Config
    {
        public NumConfig(ICustomRole role, string configName, float value, float reduceValue = 1, float increceValue = 1, int Id = 0)
        {
            ConfigName = configName;
            Role = role;
            ReduceValue = reduceValue;
            IncreceValue = increceValue;
            ConfigId = Id;
            ConfigEntry = role.RolePlugin.BasePlugin.Config.Bind(role.RolePlugin.ModName + " - " + role.GetType().Name + " - " + Id, configName, value);
        }
        public float ReduceValue;
        public float IncreceValue;
        public ConfigEntry<float> ConfigEntry;
    }
}
