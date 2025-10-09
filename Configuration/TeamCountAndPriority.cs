using BepInEx.Configuration;
using FungleAPI.Role.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    public class TeamCountAndPriority
    {
        public ModdedTeam Team;
        public string Name;
        private ConfigEntry<int> localPriority;
        private int onlinePriority;
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
        public int GetPriority()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localPriority.Value;
            }
            return onlinePriority;
        }
        public void SetPriority(int chance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                localPriority.Value = chance;
                return;
            }
            onlinePriority = chance;
        }
        public void Initialize(ConfigFile configFile, ModdedTeam team, string name)
        {
            int count = team.DefaultCount > (uint)int.MaxValue ? int.MaxValue : (int)team.DefaultCount;
            int priority = team.DefaultPriority > 500 ? 500 : (int)team.DefaultPriority;
            localCount = configFile.Bind<int>(name, "Count", team.GetType() != typeof(CrewmateTeam) ? count : 1);
            onlineCount = localCount.Value;
            localPriority = configFile.Bind<int>(name, "Priority", team.GetType() != typeof(CrewmateTeam) ? priority : -1);
            onlinePriority = localPriority.Value;
            Name = name;
            Team = team;
        }
    }
}
