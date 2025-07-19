using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;
using FungleAPI.Patches;

namespace FungleAPI.Role.Configuration
{
    public class BoolConfig : CustomConfig
    {
        public BoolConfig(ICustomRole role, string configName, bool defaultValue, int Id = 0)
        {
            ModPlugin plugin = ModPlugin.GetModPlugin(role.GetType().Assembly);
            ConfigName = configName;
            Role = role;
            localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + role.GetType().FullName + " - " + Id, configName, defaultValue.ToString());
            onlineValue = defaultValue.ToString();
        }
    }
}
