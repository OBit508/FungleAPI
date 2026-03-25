using BepInEx.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Teams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Helpers
{
    /// <summary>
    /// Helper class to teams
    /// </summary>
    public class TeamOptions : ConfigHelper
    {
        internal ConfigEntry<int> localPriority;
        internal int onlinePriority;
        internal ConfigEntry<int> localCount;
        internal int onlineCount;

        public List<ModdedOption> ExtraOptions = new List<ModdedOption>();
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
        public override string Compact()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(localCount.Value);
                    bw.Write(localPriority.Value);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        public override void Decompact(string str, bool local)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    if (local)
                    {
                        localCount.Value = br.ReadInt32();
                        localPriority.Value = br.ReadInt32();
                        return;
                    }
                    onlineCount = br.ReadInt32();
                    onlinePriority = br.ReadInt32();
                }
            }
        }
        public void Initialize(ConfigFile configFile, ModdedTeam team, string name)
        {
            int count = team.DefaultCount > int.MaxValue ? int.MaxValue : (int)team.DefaultCount;
            int priority = team.DefaultPriority > 500 ? 500 : (int)team.DefaultPriority;
            localCount = configFile.Bind(name, "Count", count);
            onlineCount = localCount.Value;
            localPriority = configFile.Bind(name, "Priority", team.GetType() != typeof(CrewmateTeam) ? priority : -1);
            onlinePriority = localPriority.Value;
            Name = name;
        }
    }
}
