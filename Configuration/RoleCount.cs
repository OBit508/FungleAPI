using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public class RoleCount
    {
        public string Name;
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
        public void Initialize(ConfigFile configFile, string name)
        {
            localCount = configFile.Bind<int>(name, "Count", 1);
            onlineCount = localCount.Value;
            Name = name;
        }
    }
}
