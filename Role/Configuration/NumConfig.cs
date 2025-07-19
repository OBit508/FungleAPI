using BepInEx.Configuration;
using FungleAPI.Patches;
using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Configuration
{
    public class NumConfig : CustomConfig
    {
        public NumConfig(ICustomRole role, string configName, float defaultValue, float reduceValue = 1, float increceValue = 1, int Id = 0)
        {
            ReduceValue = reduceValue;
            IncreceValue = increceValue;
            ModPlugin plugin = ModPlugin.GetModPlugin(role.GetType().Assembly);
            ConfigName = configName;
            Role = role;
            localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + role.GetType().FullName + " - " + Id, configName, defaultValue.ToString());
            onlineValue = defaultValue.ToString();
        }
        public float ReduceValue;
        public float IncreceValue;
    }
}
