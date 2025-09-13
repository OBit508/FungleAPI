using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public class RoleCountAndChance
    {
        public string Name;
        private ConfigEntry<int> localChance;
        private int onlineChance;
        private ConfigEntry<int> localCount;
        private int onlineCount;
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
        public void Initialize(ConfigFile configFile, string name)
        {
            localCount = configFile.Bind<int>(name, "Count", 1);
            onlineCount = localCount.Value;
            localChance = configFile.Bind<int>(name, "Chance", 100);
            onlineChance = localChance.Value;
            Name = name;
        }
    }
}
