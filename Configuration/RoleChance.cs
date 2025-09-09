using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public class RoleChance
    {
        public string Name;
        private ConfigEntry<int> localChance;
        private int onlineChance;
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
            localChance = configFile.Bind<int>(name, "Chance", 100);
            onlineChance = localChance.Value;
            Name = name;
        }
    }
}
