using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;
using FungleAPI.LoadMod;

namespace FungleAPI.Role
{
    public class BoolConfig : Config
    {
        public BoolConfig(RoleBehaviour role, string configName, bool value, int Id = 0)
        {
            ConfigName = configName;
            Role = role;
            ConfigId = Id;
            ConfigEntry = role.GetRolePlugin().BasePlugin.Config.Bind(role.GetRolePlugin().ModName + " - " + role.GetType().Name + " - " + Id, configName, value);
        }
        public ConfigEntry<bool> ConfigEntry;
    }
}
