using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Helpers
{
    public class RoleCountAndChance
    {
        public string Name;
        internal ConfigEntry<int> localChance;
        internal int onlineChance;
        internal ConfigEntry<int> localCount;
        internal int onlineCount;
        internal Type roleType;
        public int GetCount()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localCount.Value;
            }
            return onlineCount;
        }
        public void SetCount(int count)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                localCount.Value = count;
                return;
            }
            onlineCount = count;
        }
        public int GetChance()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localChance.Value;
            }
            return onlineChance;
        }
        public void SetChance(int chance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                localChance.Value = chance;
                return;
            }
            onlineChance = chance;
        }
        public void Initialize(ConfigFile configFile, string name, Type type)
        {
            roleType = type;
            localCount = configFile.Bind(name, "Count", 0);
            onlineCount = localCount.Value;
            localChance = configFile.Bind(name, "Chance", 0);
            onlineChance = localChance.Value;
            Name = name;
        }
    }
}
