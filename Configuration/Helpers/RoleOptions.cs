using BepInEx.Configuration;
using FungleAPI.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Helpers
{
    /// <summary>
    /// Helper class to roles
    /// </summary>
    public class RoleOptions : ConfigHelper
    {
        internal ConfigEntry<int> localChance;
        internal int onlineChance;

        internal ConfigEntry<int> localCount;
        internal int onlineCount;

        public List<ModdedOption> Options = new List<ModdedOption>();
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
        public override string Compact()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(localCount.Value);
                    bw.Write(localChance.Value);
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
                        localChance.Value = br.ReadInt32();
                        return;
                    }
                    onlineCount = br.ReadInt32();
                    onlineChance = br.ReadInt32();
                }
            }
        }
        public void Initialize(ConfigFile configFile, string name)
        {
            localCount = configFile.Bind(name, "Count", 0);
            onlineCount = localCount.Value;
            localChance = configFile.Bind(name, "Chance", 0);
            onlineChance = localChance.Value;
            Name = name;
        }
    }
}
