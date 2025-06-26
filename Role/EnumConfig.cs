using FungleAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using BepInEx.Configuration;
using FungleAPI.LoadMod;
using Il2CppSystem.Web;

namespace FungleAPI.Role
{
    public class EnumConfig : Config
    {
        public EnumConfig(ICustomRole role, string configName, string[] value, int Id = 0)
        {
            ConfigName = configName;
            DefaultValue = value;
            Role = role;
            ConfigId = Id;
            ConfigEntry = role.RolePlugin.BasePlugin.Config.Bind(role.RolePlugin.ModName + " - " + role.RoleName.Default + " - " + Id, configName, value[0]);
        }
        internal int currentIndex;
        internal string[] DefaultValue;
        public void BackValue()
        {
            if ((currentIndex - 1) <= 0)
            {
                currentIndex = ((string[])DefaultValue).Count() - 1;
            }
            else
            {
                currentIndex--;
            }
            ConfigEntry.Value = ((string[])DefaultValue)[currentIndex];
        }
        public void NextValue()
        {
            if ((currentIndex + 1) >= ((string[])DefaultValue).Count())
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            ConfigEntry.Value = ((string[])DefaultValue)[currentIndex];
        }
        public ConfigEntry<string> ConfigEntry;
    }
}
