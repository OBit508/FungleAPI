using BepInEx.Configuration;
using FungleAPI.Patches;
using FungleAPI.Roles;
using Il2CppSystem.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Role.Configuration
{
    public class EnumConfig : CustomConfig
    {
        public EnumConfig(ICustomRole role, string configName, string[] defaultValue, int Id = 0)
        {
            ModPlugin plugin = ModPlugin.GetModPlugin(role.GetType().Assembly);
            ConfigName = configName;
            Role = role;
            Enum = defaultValue;
            localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + role.GetType().FullName + " - " + Id, configName, defaultValue[0]);
            onlineValue = defaultValue[0];
        }
        internal int currentIndex;
        internal string[] Enum;
        public void BackValue()
        {
            if (currentIndex - 1 <= 0)
            {
                currentIndex = Enum.Count() - 1;
            }
            else
            {
                currentIndex--;
            }
            SetValue(Enum[currentIndex]);
        }
        public void NextValue()
        {
            if (currentIndex + 1 >= Enum.Count())
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            SetValue(Enum[currentIndex]);
        }
    }
}
